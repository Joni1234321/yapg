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
        }
        
        



    }

    

}