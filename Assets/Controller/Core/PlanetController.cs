using Bserg.Model.Core;
using Bserg.Model.Units;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Draw the planets and their orbits
    /// </summary>
    public class PlanetController
    {

        public SystemGenerator SystemGenerator;

        public PlanetController(SystemGenerator systemGenerator)
        {
            SystemGenerator = systemGenerator;
        }


        // Show all the planets
        public void Update(Game game, float dt)
        {
            if (!SystemGenerator.Orbits.Get(SelectionController.SelectedPlanetID, out OrbitData orbitData))
                Debug.LogError("Couldnt Find Selected Planet ID");
            
            
            
            for (int planetID = 0; planetID < game.N; planetID++)
            {
                // Planet GO
                SystemGenerator.planetTransforms[planetID].position = GetPlanetPositionAtTickF(game, planetID, game.Ticks + dt);
                int orbitID = game.Planets[planetID].OrbitObject;
                
                if (orbitID != -1)
                    SystemGenerator.planetTransforms[planetID].position += SystemGenerator.planetTransforms[orbitID].transform.position;

                // Orbit GO
                if (planetID != 0)
                {
                    SystemGenerator.orbitParent.GetChild(planetID - 1).transform.localEulerAngles = new Vector3(0,0,Mathf.Rad2Deg * GetPlanetAngleAtTicksF(game, planetID, game.Ticks + dt));
                }
            }
        }
        
        /// <summary>
        /// Returns the position of the planet at a given tick and delta
        /// </summary>
        /// <param name="game"></param>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
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