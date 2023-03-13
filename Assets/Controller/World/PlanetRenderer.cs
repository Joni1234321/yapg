using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using UnityEngine;

namespace Bserg.Controller.World
{
    /// <summary>
    /// 
    /// </summary>
    public class PlanetRenderer : WorldRenderer
    {
        public PlanetUIDrawer PlanetUIDrawer;
        private Planet[] planets;
        private OrbitalTransferSystem orbitalTransferSystem;
        private PlanetHelper planetHelper;

        private List<int> visiblePlanets;
        private Vector3[] planetPositions;
        public PlanetRenderer(Planet[] planets, OrbitalTransferSystem orbitalTransferSystem, PlanetHelper planetHelper)
        {
            PlanetUIDrawer = new PlanetUIDrawer();

            this.planets = planets;
            this.orbitalTransferSystem = orbitalTransferSystem;
            this.planetHelper = planetHelper;
        }

        public void SetVisiblePlanets(List<int> visible)
        {
            visiblePlanets = visible;
            planetPositions = new Vector3[visiblePlanets.Count];
        }

        
        public override void OnUpdate(int ticks, float dt)
        {
            planetPositions[0] = planetHelper.GetPlanetPositionAtTickF(planets, orbitalTransferSystem, 0, ticks + dt);
            for (int i = 1; i < planetPositions.Length; i++)
                planetPositions[i] = planetHelper.GetPlanetPositionAtTickF(planets, orbitalTransferSystem, visiblePlanets[i], ticks + dt);

            PlanetUIDrawer.Draw(planetPositions, planets, visiblePlanets);
        }
    }
}