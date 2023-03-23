using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Units;
using UnityEngine;

namespace Bserg.Controller.World
{
    /// <summary>
    /// Draws the ui of planets
    /// </summary>
    public class PlanetRenderer : WorldRenderer
    {
        public readonly PlanetUIDrawer PlanetUIDrawer;
        private readonly PlanetOld[] planets;
        private readonly OrbitalTransferSystem orbitalTransferSystem;
        
        public readonly SystemGenerator SystemGenerator;

        private List<int> visiblePlanets;
        private Vector3[] planetPositions;
        public PlanetRenderer(PlanetOld[] planets, OrbitalTransferSystem orbitalTransferSystem, SystemGenerator systemGenerator)
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
            for (int i = 0; i < planetPositions.Length; i++)
                planetPositions[i] = GetLocalPlanetPositionAtTickF(visiblePlanets[i], ticks + dt);

            PlanetUIDrawer.Draw(planetPositions, planets, visiblePlanets);
        }


        // ReSharper disable Unity.PerformanceAnalysis
        private void DrawGameObjects(int ticks, float dt)
        {
            for (int planetID = 0; planetID < planets.Length; planetID++)
            {
                // Planet GO
                SystemGenerator.planetTransforms[planetID].position = GetLocalPlanetPositionAtTickF( planetID, ticks + dt);

                // Orbit GO
                if (planetID != 0)
                {
                    Transform orbitTransform = SystemGenerator.orbitParent.GetChild(planetID - 1).transform;
                    orbitTransform.position =
                        SystemGenerator.planetTransforms[planets[planetID].OrbitObject].position;
                    orbitTransform.localEulerAngles = new Vector3(0,0,Mathf.Rad2Deg * GetPlanetAngleAtTicksF(planetID, ticks + dt));
                }
            }
        }        

        
        /// <summary>
        /// Returns the position of the planet at a given tick and delta
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public Vector3 GetLocalPlanetPositionAtTickF (int planetID, float ticks)
        {
            // Position in orbit
            Vector3 localPosition = SystemGenerator.GetPlanetPosition(
                (float)planets[planetID].OrbitRadius.To(Length.UnitType.AstronomicalUnits),
                GetPlanetAngleAtTicksF(planetID, ticks));
            
            int orbitID = planets[planetID].OrbitObject;
            if (orbitID == -1)
                return localPosition;

            Vector3 relativePosition =
                SystemGenerator.GetPlanetPosition((float)planets[orbitID].OrbitRadius
                    .To(Length.UnitType.AstronomicalUnits),
                    GetPlanetAngleAtTicksF(orbitID, ticks));
            
            return relativePosition + localPosition;
        }


        /// <summary>
        /// Return angles in radians
        /// </summary>
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
        
        
        /// <summary>
        /// Return angles in radians
        /// </summary>
        /// <param name="orbitPeriodTicksF"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static float GetPlanetAngleAtTicksF(float orbitPeriodTicksF, float ticks)
        {
            if (orbitPeriodTicksF == 0)
                return 0;
            return 2 * Mathf.PI * ticks / orbitPeriodTicksF;
        }
        
        /// <summary>
        /// Returns the position of the planet at a given tick and delta
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static Vector3 GetLocalPlanetPositionAtTickF (float orbitRadiusAU, float orbitalPeriodTicksF, float ticks)
        {
            // Position in orbit
            Vector3 localPosition = SystemGenerator.GetPlanetPosition(
                orbitRadiusAU,
                GetPlanetAngleAtTicksF(orbitalPeriodTicksF, ticks));
            return localPosition;
        }
    }
}