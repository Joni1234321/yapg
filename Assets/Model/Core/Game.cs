using System.Collections.Generic;
using Bserg.Model.Core.Operators;
using Bserg.Model.Core.Systems;
using Bserg.Model.Political;
using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Time = Bserg.Model.Units.Time;

namespace Bserg.Model.Core
{
    public class Game
    {
        public int Ticks { get; private set; }

        public readonly int N;

        public delegate void TickAction();

        public static event TickAction OnTick, OnTickMonth, OnTickYear, OnTickQuarter;
        
        //TODO: MAKE HOHMANN TRANSFERS IN FLOATS, AND EACH WINDOW HAS TO CALCULATE IT IN TICKS
        // Planet
        public EntityManager EntityManager;
        public readonly Entity[] Entities;
        public PlanetOld[] Planets { get; }
        public string[] PlanetNames;
        public bool[] Inhabited;
        
        // Population
        public PlanetLevels PlanetLevels;
        public PlanetLevelsGeneric<float> LevelProgress;
        public float[] PlanetPopulationProgress;

        // Political
        public PoliticalBody[] PlanetPoliticalBodies;
        public Player Player;

        public OrbitalTransferSystem OrbitalTransferSystem;
            
        public PopulationGrowthSystem PopulationGrowthSystem;
        public BuildSystem BuildSystem;
        public BuildOperator BuildOperator;
        
        public MigrationSystem MigrationSystem;
        public SettleSystem SettleSystem;
        public SpaceflightSystem SpaceflightSystem;
        
        EntityQuery gameTicksQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameTicks));

        
        public Game(string[] planetNames,
            float[] givenPopulationLevels,
            PoliticalBody[] planetPoliticalBodies,
            PlanetOld[] planets,
            OrbitData orbits)
        {
            N = planetNames.Length;
            PlanetNames = planetNames;
            Inhabited = new bool[N];
            Planets = planets;
            Entities = new Entity[N];
            
            Recipe.Load();

            // Spawn objects
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Queue<(OrbitData, Entity, int)> queue = new ();
            queue.Enqueue((orbits, Entity.Null, 0));
            
            // Builds children recursively // BUILD CHILDREN BECAUSE FIRST ELEMENT HAS ID -1 for WORLD
            while (queue.Count != 0)
            {
                (OrbitData orbit, Entity parent, int layer) = queue.Dequeue();
                
                // Build orbit for each child
                foreach (OrbitData child in orbit.Children)
                {
                    PlanetOld childPlanet = planets[child.PlanetID];
                    Planet.PrefabData childData = new Planet.PrefabData
                    {
                        Name = childPlanet.Name,
                        Population = (long)givenPopulationLevels[child.PlanetID],
                        Color = new float4(childPlanet.Color.r, childPlanet.Color.g, childPlanet.Color.b, childPlanet.Color.a),
                        Size = childPlanet.Size,
                        WeightEarthMass = childPlanet.Mass.To(Mass.UnitType.EarthMass),
                        RadiusAU = childPlanet.OrbitRadius.To(Length.UnitType.AstronomicalUnits),
                        OrbitObject = childPlanet.OrbitObject
                    };

                    Entity childEntity = CreatePlanetEntity(EntityManager, parent, layer, childData);
                    child.Entity = childEntity;
                    child.OrbitRadius = childPlanet.OrbitRadius;
                    Entities[child.PlanetID] = childEntity;
                    queue.Enqueue((child, childEntity, layer + 1));
                }
            }

            
            // Generate transfer map
            NativeHashMap<EntityPair, HohmannTransfer> map =
                new NativeHashMap<EntityPair, HohmannTransfer>(N * 10, Allocator.Persistent);

           orbits.GenerateTransfersForChildren(EntityManager, ref map);

           // Initial settle
           EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
           ecb.AppendToBuffer(Entities[3], new Settle.Order
           {
               Destination = Entities[4],
               Parallel = 6,
               Serial = 50,
           });
           ecb.AppendToBuffer(Entities[3], new Settle.Order
           {
               Destination = Entities[2],
               Parallel = 6,
               Serial = 50,           
           });
           ecb.AppendToBuffer(Entities[3], new Settle.Order
           {
               Destination = Entities[1],
               Parallel = 6,
               Serial = 50,
           });
           ecb.Playback(EntityManager);
           ecb.Dispose();
           
           
            // Population
            PlanetLevels = new PlanetLevels(N);
            LevelProgress = new PlanetLevelsGeneric<float>(N);
            PlanetPopulationProgress = LevelProgress.Get("Population");

            int[] planetLandLevels = PlanetLevels.Get("Land");
            int[] planetPopulationLevels = PlanetLevels.Get("Population");



            // Political
            PlanetPoliticalBodies = planetPoliticalBodies;
            Player = new Player("Player1", "Democracy", "UN");

            
            OrbitalTransferSystem = new OrbitalTransferSystem(this);
            
            PopulationGrowthSystem = new PopulationGrowthSystem(this);
            BuildSystem = new BuildSystem(this);
            
            MigrationSystem = new MigrationSystem(this);
            SettleSystem = new SettleSystem(this);
            SpaceflightSystem = new SpaceflightSystem(this);
            
            
            BuildOperator = new BuildOperator(this);

            
            for (int i = 0; i < N; i++)
            {
                planetLandLevels[i] = 50;
                planetPopulationLevels[i] = (int)givenPopulationLevels[i];
                LevelProgress.Get("Population")[i] = givenPopulationLevels[i] - planetPopulationLevels[i];
                if (planetPopulationLevels[i] > 0)
                    Inhabited[i] = true;
                BuildOperator.SetRecipeLevel(Recipe.Get("Housing"), planetPopulationLevels[i] + (planetPopulationLevels[i] > 15 ? 1 : 0), i);
                if (planetPopulationLevels[i] > 1)
                    BuildOperator.SetRecipeLevel(Recipe.Get("Food"), planetPopulationLevels[i] + 1, i);
            }
            
            
            
            // Dude
            EntityManager.CreateSingleton(new GameTicks { Ticks = Ticks });
            EntityManager.CreateSingleton(new HohmannTransferMap { Map = map.AsReadOnly() });
            OnTickMonth += TickMonth;
            OnTickYear += TickYear;
            OnTickQuarter += TickQuarter;

        }


        
        /// <summary>
        /// Game progresses through ticks
        /// </summary>
        public void DoTick()
        {
            if (!TickSystemGroup.CanTick())
                return;
            
            // Increment time
            Ticks++;
            gameTicksQuery.GetSingletonRW<GameTicks>().ValueRW.Ticks = Ticks;
            bool r = TickSystemGroup.TryTick();
            Assert.IsTrue(r);
            
            OnTick?.Invoke();
            if (Ticks % GameTick.TICKS_PER_MONTH == 0)
            {
                TickMonthSystemGroup.Tick();
                OnTickMonth?.Invoke();
                if (Ticks % GameTick.TICKS_PER_QUARTER == 0)
                {
                    TickQuarterSystemGroup.Tick();
                    OnTickQuarter?.Invoke();
                    if (Ticks % GameTick.TICKS_PER_YEAR == 0)
                    {
                        TickYearSystemGroup.Tick();
                        OnTickYear?.Invoke();
                    }
                }
            }
            
            //SpaceflightSystem.System();
            
        }

        
        /// <summary>
        /// Called every in game month
        /// </summary>
        private void TickMonth()
        {
            
        }

        /// <summary>
        /// Called every in game quarter
        /// </summary>
        private void TickQuarter()
        { 
            //SettleSystem.System();
            MigrationSystem.System();
        }

        /// <summary>
        /// Called every in game year
        /// </summary>
        private void TickYear()
        {
            //PopulationGrowthSystem.System();
        }
        
        public PlanetOld GetPlanet(int planetID)
        {
            if (planetID == -1)
                return null;
            
            return Planets[planetID];
        }
        
        /// <summary>
        /// Returns the tick count until next event will happen
        /// </summary>
        /// <param name="tickPeriod">The event happens every n ticks</param>
        /// <param name="tickOffset">The event is offset by n ticks from the start of the game</param>
        /// <returns>The tick</returns>
        public float TicksUntilNextEventF(float tickPeriod, float tickOffset = 0) =>
            TicksUntilNextEventF(Ticks, tickPeriod, tickOffset);
        public float TicksUntilNextEventF(Time time, float tickOffset = 0) => 
            TicksUntilNextEventF(GameTick.ToTickF(time), tickOffset);

        public static float TicksUntilNextEventF(int ticks, float tickPeriod, float tickOffset = 0)
        {
            if (tickPeriod == 0) 
                return 0;

            float ticksSinceLastEvent = ((ticks + 1) - tickOffset) % tickPeriod;
            return tickPeriod - ticksSinceLastEvent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickPeriod">Every n ticks it happens</param>
        /// <param name="tickOffset">First event starts at n tick</param>
        /// <returns></returns>
        public int TickAtNextEvent(int tickPeriod, int tickOffset = 0)
        {
            if (tickPeriod == 0) 
                return Ticks + 1;

            int eventNTimesBefore = ((Ticks + 1) - tickOffset) / tickPeriod;
            int tickAtLastEvent =  eventNTimesBefore * tickPeriod + tickOffset;
            return tickPeriod + tickAtLastEvent;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickPeriod">Every n ticks it happens</param>
        /// <param name="tickOffset">First event starts at n tick</param>
        /// <returns></returns>
        public float TickAtNextEventF(float tickPeriod, float tickOffset = 0) =>
            GameTick.TickAtNextEventF(Ticks, tickPeriod, tickOffset);
        





        Entity CreatePlanetEntity(EntityManager entityManager, Entity parent, int layer, Planet.PrefabData data)
        {
            Entity e = entityManager.CreateEntity();

            #if UNITY_EDITOR
            entityManager.SetName(e, data.Name);
            #endif
            
            // Tag
            entityManager.AddComponentData(e, new Planet.Tag());
            
            // Data
            Mass mass = new Mass(data.WeightEarthMass, Mass.UnitType.EarthMass);
            Length radius = new Length(data.RadiusAU, Length.UnitType.AstronomicalUnits);
            entityManager.AddComponentData(e, new Planet.Data
            {
                Color = data.Color,
                Size = data.Size,
                Mass = mass,
                OrbitRadius = radius,
            });
            entityManager.AddComponentData(e, new Planet.Name { Text = data.Name });
            
            // Population
            if (data.Population > 0)
                entityManager.AddComponentData(e, new Planet.ActiveTag());
            entityManager.AddComponentData(e, new PopulationLevel { Level = (int)data.Population });
            entityManager.AddComponentData(e, new PopulationProgress { Progress = data.Population - (int)data.Population });
            
            // Levels
            entityManager.AddComponentData(e, new HousingLevel { Level = 50 });
            entityManager.AddComponentData(e, new FoodLevel { Level = 0 });
            entityManager.AddComponentData(e, new HousingLevel { Level = 0 });
            
            // Growth System
            entityManager.AddComponentData(e, new PopulationGrowth { BirthRate = 0.02f, DeathRate = 0.005f } );
            
            // OrbitSystem
            entityManager.AddComponentData(e,
                new OrbitRadius() { RadiusAU = (float)radius.To(Length.UnitType.AstronomicalUnits) });
            entityManager.AddComponentData(e, new StandardGravitationalParameter
            {
                Value = StandardGravitationalParameterOld.GRAVITATIONAL_CONSTANT *
                        mass.To(Mass.UnitType.KiloGrams) });
            entityManager.AddComponentData(e, new PlanetOrbit { OrbitEntity = parent });

            Time orbitPeriod = Time.Zero();
            if (parent != Entity.Null)
            {
                StandardGravitationalParameter parentGravitation =
                    entityManager.GetComponentData<StandardGravitationalParameter>(parent);
                orbitPeriod = OrbitalMechanics.GetOrbitalPeriod(parentGravitation, radius);
            }
            entityManager.AddComponentData(e, new OrbitPeriod { TicksF = GameTick.ToTickF(orbitPeriod) });


            // Spacecraft System
            entityManager.AddComponentData(e, new SpacecraftPool { Available = data.Population > 30 ? 4000 : 0});
            
            // Settle system
            entityManager.AddBuffer<Settle.Order>(e);
            
            return e;
        }
    }


}