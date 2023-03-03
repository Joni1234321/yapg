using System.Collections.Generic;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Political;
using Bserg.Model.Units;
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
        public Planet[] Planets { get; }
        public string[] PlanetNames;
        
        // Population
        public PlanetLevels PlanetLevels;
        public float[] PlanetPopulationLevels;

        // Political
        public PoliticalBody[] PlanetPoliticalBodies;
        public Player Player;

        public OrbitalTransferSystem OrbitalTransferSystem;
        
        public PopulationGrowthSystem PopulationGrowthSystem;
        public MigrationSystem MigrationSystem;
        public SettleSystem SettleSystem;
        public SpaceflightSystem SpaceflightSystem;
        

        public Game(string[] planetNames, float[] planetPopulationLevels, PoliticalBody[] planetPoliticalBodies, Planet[] planets)
        {
            N = planetNames.Length;
            PlanetNames = planetNames;
            Planets = planets;
            
            Recipe.Load();
            
            // Population
            PlanetLevels = new PlanetLevels(N);
            PlanetPopulationLevels = planetPopulationLevels;

            int[] planetHousingLevels = PlanetLevels.Get("Housing");
            int[] planetFoodLevels = PlanetLevels.Get("Food");
            int[] planetLandLevels = PlanetLevels.Get("Land");

            for (int i = 0; i < N; i++)
            {
                planetHousingLevels[i] = (int)planetPopulationLevels[i] + (planetPopulationLevels[i] > 15 ? 1 : 0);
                planetLandLevels[i] = 50;
                if (PlanetPopulationLevels[i] > 1)
                    planetFoodLevels[i] = (int)PlanetPopulationLevels[i] + 1;
                
            }

            // Political
            PlanetPoliticalBodies = planetPoliticalBodies;
            Player = new Player("Player1", "Democracy", "UN");

            
            OrbitalTransferSystem = new OrbitalTransferSystem(this);
            
            PopulationGrowthSystem = new PopulationGrowthSystem(this);
            MigrationSystem = new MigrationSystem(this);
            SettleSystem = new SettleSystem(this);
            SpaceflightSystem = new SpaceflightSystem(this);
            
            // Dude
            OnTickMonth += TickMonth;
            OnTickYear += TickYear;
            OnTickQuarter += TickQuarter;
        }

        private Stack<int> toDelete = new(64);


        
        /// <summary>
        /// Game progresses through ticks
        /// </summary>
        public void DoTick()
        {

            OnTick?.Invoke();
            if (Ticks % GameTick.TICKS_PER_MONTH == 0)
            {
                OnTickMonth?.Invoke();
                if (Ticks % GameTick.TICKS_PER_QUARTER == 0)
                {
                    OnTickQuarter?.Invoke();
                    if (Ticks % GameTick.TICKS_PER_YEAR == 0)
                    {
                        OnTickYear?.Invoke();
                    }
                }
            }
            
            SpaceflightSystem.System();
            
            // Increment time
            Ticks++;
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
            PopulationGrowthSystem.System();
        }
        
        public Planet GetPlanet(int planetID)
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