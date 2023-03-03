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
            StandardGravitationalParameter mu = new StandardGravitationalParameter(Game.Planets[0].Mass);
            OrbitalPeriodsInTicks = new float[Game.N];
            HohmannDeltaV = new float[Game.N,Game.N];
            for (int i = 0; i < Game.N; i++) 
                OrbitalPeriodsInTicks[i] = GameTick.ToTickF(OrbitalMechanics.GetOrbitalPeriod(mu, Game.Planets[i].OrbitRadius));
            for (int i = 0; i < Game.N; i++)
            for (int j = 0; j < Game.N; j++)
            {
                if (i == 0 || j == 0 || i == j)
                {
                    HohmannDeltaV[i, j] = 0;
                    continue;
                }
                HohmannDeltaV[i, j] = (float)OrbitalMechanics.GetHohmannDeltaV(mu, Game.Planets[i].OrbitRadius, Game.Planets[j].OrbitRadius);
            }
            HohmannTransfers = CalculateHohmannTransfers(Game.Planets[0], Game.Planets);
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