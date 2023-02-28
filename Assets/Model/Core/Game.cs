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
        public float[] OrbitalPeriodsInTicks;
        public HohmannTransfer<float>[,] HohmannTransfers;
        public float[,] HohmannDeltaV;
        
        // Population
        public long[] PlanetPopulations;

        // Political
        public PoliticalBody[] PlanetPoliticalBodies;
        public Player Player;

        public PopulationGrowthSystem PopulationGrowthSystem;
        public MigrationSystem MigrationSystem;
        public SettleSystem SettleSystem;
        public SpaceflightSystem SpaceflightSystem;

        public Game(string[] planetNames, long[] planetPopulations, PoliticalBody[] planetPoliticalBodies, Planet[] planets)
        {
            N = planetNames.Length;
            PlanetNames = planetNames;
            Planets = planets;
            
            // Orbits
            StandardGravitationalParameter mu = new StandardGravitationalParameter(planets[0].Mass);
            OrbitalPeriodsInTicks = new float[N];
            HohmannDeltaV = new float[N, N];
            for (int i = 0; i < N; i++) 
                OrbitalPeriodsInTicks[i] = GameTick.ToTickF(OrbitalMechanics.GetOrbitalPeriod(mu, planets[i].OrbitRadius));
            for (int i = 0; i < N; i++)
            for (int j = 0; j < N; j++)
            {
                if (i == 0 || j == 0 || i == j)
                {
                    HohmannDeltaV[i, j] = 0;
                    continue;
                }
                HohmannDeltaV[i, j] = (float)OrbitalMechanics.GetHohmannDeltaV(mu, planets[i].OrbitRadius, planets[j].OrbitRadius);
            }
            HohmannTransfers = CalculateHohmannTransfers(planets[0], planets);

                // Population
            PlanetPopulations = planetPopulations;

            // Political
            PlanetPoliticalBodies = planetPoliticalBodies;
            Player = new Player("Player1", "Democracy", "UN");

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
        /// Hohmann transfer has a launch window and a duration to destination
        /// </summary>
        public struct HohmannTransfer<T>
        {
            public readonly T Offset, Window, Duration;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="offset">first launch window</param>
            /// <param name="window">delta time till next launch windows</param>
            /// <param name="duration">time it takes from launch at departure till destination</param>
            public HohmannTransfer(T offset, T window, T duration)
            {
                Offset = offset;
                Window = window;
                Duration = duration;
            }
        }
        
        /// <summary>
        /// Calculates all the hohmann transfers windows
        /// </summary>
        /// <param name="orbit">The object they orbit</param>
        /// <param name="planets"> Planets to calculate </param>
        /// <returns>Returns n*n array, first coordiante is departure, second is arrival</returns>
        HohmannTransfer<float>[,] CalculateHohmannTransfers(Planet orbit, Planet[] planets)
        {
            // Orbit values
            StandardGravitationalParameter mu = new StandardGravitationalParameter(orbit.Mass);
            
            // Planets
            int n = planets.Length;
            HohmannTransfer<float>[,] transfers = new HohmannTransfer<float>[n, n];

            for (int i = 0; i < n; i++)
            {
                Planet departure = planets[i];
                Time departureOrbitalPeriod = OrbitalMechanics.GetOrbitalPeriod(mu, departure.OrbitRadius);

                for (int j = 0; j < n; j++)
                {
                    // No time between itself
                    if (i == j)
                    {
                        transfers[i, j] = new HohmannTransfer<float>(0, 0, 0);
                        continue;
                    }

                    // Calculate
                    Planet destination = planets[j];
                    Time destinationOrbitalPeriod = OrbitalMechanics.GetOrbitalPeriod(mu, destination.OrbitRadius);
                    Time window = 
                        OrbitalMechanics.GetSynodicPeriod(departureOrbitalPeriod, destinationOrbitalPeriod);
                    Time duration =
                        OrbitalMechanics.HohmannTransferDuration(mu, departure.OrbitRadius, destination.OrbitRadius);
                    double deltaAngleAtLaunch =
                        OrbitalMechanics.HohmannAngleDifferenceAtLaunch(destinationOrbitalPeriod, duration);
                    Time timeUntilFirstWindow = OrbitalMechanics.GetTimeUntilAngleDifference(deltaAngleAtLaunch, departureOrbitalPeriod, destinationOrbitalPeriod); 
                    transfers[i, j] = new HohmannTransfer<float>(GameTick.ToTickF(timeUntilFirstWindow), GameTick.ToTickF(window), GameTick.ToTickF(duration));
                }
            }

            // Return
            return transfers;
        }
        
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
        
        /// <summary>
        /// Handles build orders
        /// </summary>
        public void BuildSystem()
        {
            foreach (Planet planet in Planets)
            {
                // Tick
                for (int i = 0; i < planet.BuildOrders.Count; i++)
                {
                    BuildOrder buildOrder = planet.BuildOrders[i];
                    buildOrder.Progress++;
                    if (buildOrder.Progress >= 100)
                        toDelete.Push(i);
                }

                // Delete
                while (toDelete.Count > 0)
                    planet.BuildOrders.RemoveAt(toDelete.Pop());
            }
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