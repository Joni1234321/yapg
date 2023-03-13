using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.World
{
    /// <summary>
    /// Draws the ui of planets
    /// </summary>
    public class PlanetRenderer : WorldRenderer
    {
        public readonly PlanetUIDrawer PlanetUIDrawer;
        private readonly Planet[] planets;
        private readonly OrbitalTransferSystem orbitalTransferSystem;
        
        public readonly SystemGenerator SystemGenerator;

        private List<int> visiblePlanets;
        private Vector3[] planetPositions;
        public PlanetRenderer(Planet[] planets, OrbitalTransferSystem orbitalTransferSystem, SystemGenerator systemGenerator)
        {
            PlanetUIDrawer = new PlanetUIDrawer();

            this.planets = planets;
            this.orbitalTransferSystem = orbitalTransferSystem;
            SystemGenerator = systemGenerator;
        }

        public void SetVisiblePlanets(List<int> visible)
        {
            visiblePlanets = visible;
            planetPositions = new Vector3[visiblePlanets.Count];
        }

        
        public override void OnUpdate(int ticks, float dt)
        {
            // Draw all planets
            DrawGameObjects(ticks, dt);
            
            // Only draw visible uis
            planetPositions[0] = GetPlanetPositionAtTickF(0, ticks + dt);
            for (int i = 1; i < planetPositions.Length; i++)
                planetPositions[i] = GetPlanetPositionAtTickF(visiblePlanets[i], ticks + dt);

            // Add orbit offset
            for (int i = 0; i < visiblePlanets.Count; i++)
            {
                Planet planet = planets[visiblePlanets[i]];
                if (planet.OrbitObject != -1)
                    planetPositions[i] += planetPositions[visiblePlanets.IndexOf(planet.OrbitObject)];
            }
            
            PlanetUIDrawer.Draw(planetPositions, planets, visiblePlanets);
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private void DrawGameObjects(int ticks, float dt)
        {
            if (!SystemGenerator.Orbits.Get(SelectionHelper.SelectedPlanetID, out OrbitData orbitData))
                Debug.LogError("Couldnt Find Selected Planet ID");

            for (int planetID = 0; planetID < planets.Length; planetID++)
            {
                // Planet GO
                SystemGenerator.planetTransforms[planetID].position = GetPlanetPositionAtTickF( planetID, ticks + dt);
                int orbitID = planets[planetID].OrbitObject;
                
                if (orbitID != -1)
                    SystemGenerator.planetTransforms[planetID].position += SystemGenerator.planetTransforms[orbitID].transform.position;

                // Orbit GO
                if (planetID != 0)
                    SystemGenerator.orbitParent.GetChild(planetID - 1).transform.localEulerAngles = new Vector3(0,0,Mathf.Rad2Deg * GetPlanetAngleAtTicksF(planetID, ticks + dt));
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
        public Vector3 GetPlanetPositionAtTickF (int planetID, float ticks)
        {
            return SystemGenerator.GetPlanetPosition((float)planets[planetID].OrbitRadius.To(Length.UnitType.AstronomicalUnits), GetPlanetAngleAtTicksF(planetID, ticks));
        }


        /// <summary>
        /// Return angles in radians
        /// </summary>
        /// <param name="orbitalTransferSystem"></param>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public float GetPlanetAngleAtTicksF(int planetID, float ticks)
        {
            float orbitalPeriodsInTick = orbitalTransferSystem.OrbitalPeriodsInTicks[planetID];
            if (orbitalPeriodsInTick == 0)
                return 0;
            return 2 * Mathf.PI * ticks / orbitalPeriodsInTick;
        }
    }
}