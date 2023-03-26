using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Unity.Mathematics;
using UnityEngine;

namespace Bserg.Controller.WorldRenderer
{
    /// <summary>
    /// Draws the ui of planets
    /// </summary>
    public class PlanetRenderer : WorldRenderer
    {
        private readonly PlanetOld[] planets;
        private readonly OrbitalTransferSystem orbitalTransferSystem;
        
        private List<int> visiblePlanets;
        private Vector3[] planetPositions;
        public PlanetRenderer(PlanetOld[] planets, OrbitalTransferSystem orbitalTransferSystem, SystemGenerator systemGenerator)
        {

            this.planets = planets;
            this.orbitalTransferSystem = orbitalTransferSystem;
        }

        public void SetVisiblePlanets(List<int> visible)
        {
            visiblePlanets = visible;
            planetPositions = new Vector3[visiblePlanets.Count];
        }

        


        /// <summary>
        /// Returns the position of the planet at a given tick and delta
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="ticks"></param>
        /// <returns></returns>
        public static Vector3 GetLocalPlanetPositionAtTickF (PlanetOld[]  planets, int planetID, float ticks)
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
        /// <param name="ticksF"></param>
        /// <returns></returns>
        public float GetPlanetAngleAtTicksF(int planetID, float ticksF)
        {
            float orbitalPeriodsInTick = orbitalTransferSystem.OrbitalPeriodsInTicks[planetID];
            if (orbitalPeriodsInTick == 0)
                return 0;
            return 2 * math.PI * ticksF / orbitalPeriodsInTick;
        }
        
        
        /// <summary>
        /// Return angles in radians
        /// </summary>
        /// <param name="orbitPeriodTicksF"></param>
        /// <param name="ticksF"></param>
        /// <returns></returns>
        public static float GetPlanetAngleAtTicksF(float orbitPeriodTicksF, float ticksF)
        {
            if (orbitPeriodTicksF == 0)
                return 0;
            return 2 * math.PI * ticksF / orbitPeriodTicksF;
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

        public override void OnUpdate(int ticks, float dt)
        {
            
        }
    }
}