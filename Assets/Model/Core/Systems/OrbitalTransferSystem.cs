using Bserg.Model.Space;
using Bserg.Model.Units;

namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Handle logic that makes it possible to calculate when it is possible to fly between celestial objects
    /// </summary>
    public class OrbitalTransferSystem : GameSystem
    {
        public float[] OrbitalPeriodsInTicks;
        public HohmannTransfer<float>[,] HohmannTransfers;
        public float[,] HohmannDeltaV;

        
        public OrbitalTransferSystem(Game game) : base(game)
        {
            // Orbits
            StandardGravitationalParameterOld mu = new StandardGravitationalParameterOld(Game.Planets[0].Mass);
            OrbitalPeriodsInTicks = new float[Game.N];
            HohmannDeltaV = new float[Game.N,Game.N];
            for (int i = 1; i < Game.N; i++) 
                OrbitalPeriodsInTicks[i] = GameTick.ToTickF(OrbitalMechanics.GetOrbitalPeriod(new StandardGravitationalParameterOld(Game.Planets[Game.Planets[i].OrbitObject].Mass), Game.Planets[i].OrbitRadius));
            for (int planetDeparture = 0; planetDeparture < Game.N; planetDeparture++)
            for (int planetDestination = 0; planetDestination < Game.N; planetDestination++)
            {
                if (planetDeparture == 0 || planetDestination == 0 || planetDeparture == planetDestination)
                {
                    HohmannDeltaV[planetDeparture, planetDestination] = 0;
                    continue;
                }
                // Function doesnt work for satellites
                HohmannDeltaV[planetDeparture, planetDestination] = (float)OrbitalMechanics.GetHohmannDeltaV(mu, Game.Planets[planetDeparture].OrbitRadius, Game.Planets[planetDestination].OrbitRadius);
            }
            HohmannTransfers = CalculateHohmannTransfers(Game.Planets);
        }
        
        /// <summary>
        /// Calculates all the hohmann transfers windows
        /// </summary>
        /// <param name="planets"> Planets to calculate </param>
        /// <returns>Returns n*n array, first coordiante is departure, second is arrival</returns>
        HohmannTransfer<float>[,] CalculateHohmannTransfers(PlanetOld[] planets)
        {
            // Planets
            int n = planets.Length;
            HohmannTransfer<float>[,] transfers = new HohmannTransfer<float>[n, n];

            for (int i = 0; i < n; i++)
            {
                int orbitID = Game.Planets[i].OrbitObject;
                StandardGravitationalParameterOld muDeparture = new StandardGravitationalParameterOld(Game.Planets[orbitID == -1 ? 0 : orbitID].Mass);
                
                PlanetOld departure = planets[i];
                Time departureOrbitalPeriod = OrbitalMechanics.GetOrbitalPeriod(muDeparture, departure.OrbitRadius);

                for (int j = 0; j < n; j++)
                {
                    // No time between itself
                    if (i == j)
                    {
                        transfers[i, j] = new HohmannTransfer<float>(0, 0, 0);
                        continue;
                    }

                    // Calculate
                    PlanetOld destination = planets[j];
                    int departureOrbitID = Game.Planets[j].OrbitObject;
                    StandardGravitationalParameterOld muDestination = new StandardGravitationalParameterOld(Game.Planets[departureOrbitID == -1 ? 0 : departureOrbitID].Mass);
                    Time destinationOrbitalPeriod = OrbitalMechanics.GetOrbitalPeriod(muDestination, destination.OrbitRadius);
                    Time window = 
                        OrbitalMechanics.GetSynodicPeriod(departureOrbitalPeriod, destinationOrbitalPeriod);
                    Time duration =
                        OrbitalMechanics.HohmannTransferDuration(muDeparture, departure.OrbitRadius, destination.OrbitRadius);
                    double deltaAngleAtLaunch =
                        OrbitalMechanics.HohmannAngleDifferenceAtLaunch(destinationOrbitalPeriod, duration);
                    Time timeUntilFirstWindow = OrbitalMechanics.GetTimeUntilAngleDifference(deltaAngleAtLaunch, departureOrbitalPeriod, destinationOrbitalPeriod); 
                    transfers[i, j] = new HohmannTransfer<float>(GameTick.ToTickF(timeUntilFirstWindow), GameTick.ToTickF(window), GameTick.ToTickF(duration));
                }
            }

            // Return
            return transfers;
        }
        
    }
    
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
}