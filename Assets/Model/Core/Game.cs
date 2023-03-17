using System.Collections.Generic;
using Bserg.Model.Core.Operators;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Political;
using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
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
        public Space.Planet[] Planets { get; }
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
        
        EntityQuery gameticksQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameTicks));

        
        public Game(string[] planetNames, float[] givenPopulationLevels, PoliticalBody[] planetPoliticalBodies, Space.Planet[] planets)
        {
            N = planetNames.Length;
            PlanetNames = planetNames;
            Inhabited = new bool[N];
            Planets = planets;
            
            Recipe.Load();


            // Spawn objects
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            Entities = new Entity[N];
            for (int i = 0; i < N; i++)
            {
                Space.Planet p = planets[i];
                Entities[i] = EntityManager.CreateEntity();
                EntityManager.AddComponentData(Entities[i], new Shared.Components.Planet.PrefabData
                {
                    Name = p.Name,
                    Population = (long)givenPopulationLevels[i],
                    Color = new float4(p.Color.r, p.Color.g, p.Color.b, p.Color.a),
                    Size = p.Size,
                    WeightEarthMass = p.Mass.To(Mass.UnitType.EarthMass),
                    RadiusAU = p.OrbitRadius.To(Length.UnitType.AstronomicalUnits),
                    OrbitObject = p.OrbitObject
                });
            }

            
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
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton(new GameTicks { Ticks = Ticks });
            OnTickMonth += TickMonth;
            OnTickYear += TickYear;
            OnTickQuarter += TickQuarter;
        }


        
        /// <summary>
        /// Game progresses through ticks
        /// </summary>
        public void DoTick()
        {
            
            if (!TickSystemGroup.TryTick())
                return;
            
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
            
            SpaceflightSystem.System();
            
            // Increment time
            Ticks++;
            gameticksQuery.GetSingletonRW<GameTicks>().ValueRW.Ticks = Ticks;
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
            SettleSystem.System();
            MigrationSystem.System();
        }

        /// <summary>
        /// Called every in game year
        /// </summary>
        private void TickYear()
        {
            //PopulationGrowthSystem.System();
        }
        
        public Space.Planet GetPlanet(int planetID)
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
        public float TicksUntilNextEventF(float tickPeriod, float tickOffset = 0)
        {
            if (tickPeriod == 0) 
                return 0;

            float ticksSinceLastEvent = ((Ticks + 1) - tickOffset) % tickPeriod;
            return tickPeriod - ticksSinceLastEvent;
        }
        public float TicksUntilNextEventF(Time time, float tickOffset = 0) => TicksUntilNextEventF(GameTick.ToTickF(time), tickOffset);


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
        public float TickAtNextEventF(float tickPeriod, float tickOffset = 0)
        {
            if (tickPeriod == 0) 
                return Ticks + 1;

            int eventNTimesBefore = (int)((Ticks + 1 - tickOffset) / tickPeriod);
            float tickAtLastEvent =  eventNTimesBefore * tickPeriod + tickOffset;
            return tickPeriod + tickAtLastEvent;
        }
    }


}