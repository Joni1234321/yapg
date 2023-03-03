using Bserg.Model.Core;
using Bserg.Model.Units;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    public class OrbitController
    {

        public SystemGenerator SystemGenerator;

        public OrbitController(SystemGenerator systemGenerator)
        {
            SystemGenerator = systemGenerator;
        }


        // Show all the planets
        public void Update(Game game, float dt)
        {
            
            for (int planetID = 0; planetID < game.N; planetID++)
            {
                SystemGenerator.transform.GetChild(planetID).transform.position = GetPlanetPositionAtTickF(game, planetID, game.Ticks + dt);
            
                // Orbit
                if (planetID != 0)
                {
                    SystemGenerator.orbitParent.GetChild(planetID - 1).transform.localEulerAngles = new Vector3(0,0,Mathf.Rad2Deg * GetPlanetAngleAtTicksF(game, planetID, game.Ticks + dt));
                }
            }
        }
        
        public Vector3 GetPlanetPositionAtTickF (Game game, int planetID, float ticks)
        {
            return SystemGenerator.GetPlanetPosition((float)game.Planets[planetID].OrbitRadius.To(Length.UnitType.AstronomicalUnits), GetPlanetAngleAtTicksF(game, planetID, ticks));
        }

        /// <summary>
        /// Return angles in radians
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public float GetPlanetAngleAtTicksF(Game game, int planetID, float ticks)
        {
            float orbitalPeriodsInTick = game.OrbitalTransferSystem.OrbitalPeriodsInTicks[planetID];
            if (orbitalPeriodsInTick == 0)
                return 0;
            return 2 * Mathf.PI * ticks / orbitalPeriodsInTick;
        }

    }
}