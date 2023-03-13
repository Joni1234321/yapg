using Bserg.Model.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Draw the planets and their orbits
    /// </summary>
    public class PlanetHelper
    {

        public SystemGenerator SystemGenerator;

        public PlanetHelper(SystemGenerator systemGenerator)
        {
            SystemGenerator = systemGenerator;
        }


        // Show all the planets
        // ReSharper disable Unity.PerformanceAnalysis
        public void Update(Game game, float dt)
        {
            if (!SystemGenerator.Orbits.Get(SelectionController.SelectedPlanetID, out OrbitData orbitData))
                Debug.LogError("Couldnt Find Selected Planet ID");
            
            
            for (int planetID = 0; planetID < game.N; planetID++)
            {
                // Planet GO
                SystemGenerator.planetTransforms[planetID].position = GetPlanetPositionAtTickF(game.Planets, game.OrbitalTransferSystem, planetID, game.Ticks + dt);
                int orbitID = game.Planets[planetID].OrbitObject;
                
                if (orbitID != -1)
                    SystemGenerator.planetTransforms[planetID].position += SystemGenerator.planetTransforms[orbitID].transform.position;

                // Orbit GO
                if (planetID != 0)
                {
                    SystemGenerator.orbitParent.GetChild(planetID - 1).transform.localEulerAngles = new Vector3(0,0,Mathf.Rad2Deg * GetPlanetAngleAtTicksF(game.OrbitalTransferSystem, planetID, game.Ticks + dt));
                }
            }
        }

        /// <summary>
        /// Returns the position of the planet at a given tick and delta
        /// </summary>
        /// <param name="planets"></param>
        /// <param name="orbitalTransferSystem"></param>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public Vector3 GetPlanetPositionAtTickF (Planet[] planets, OrbitalTransferSystem orbitalTransferSystem, int planetID, float ticks)
        {
            return SystemGenerator.GetPlanetPosition((float)planets[planetID].OrbitRadius.To(Length.UnitType.AstronomicalUnits), GetPlanetAngleAtTicksF(orbitalTransferSystem, planetID, ticks));
        }


        /// <summary>
        /// Return angles in radians
        /// </summary>
        /// <param name="orbitalTransferSystem"></param>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public float GetPlanetAngleAtTicksF(OrbitalTransferSystem orbitalTransferSystem, int planetID, float ticks)
        {
            float orbitalPeriodsInTick = orbitalTransferSystem.OrbitalPeriodsInTicks[planetID];
            if (orbitalPeriodsInTick == 0)
                return 0;
            return 2 * Mathf.PI * ticks / orbitalPeriodsInTick;
        }

    }
}