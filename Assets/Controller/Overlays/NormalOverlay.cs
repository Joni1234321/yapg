using System.Collections.Generic;
using Bserg.Controller.Core;
using Bserg.Controller.Material;
using Bserg.Controller.UI;
using Bserg.Model.Core;
using Bserg.Model.Space;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Overlays
{
    public class NormalOverlay : Overlay
    {
        private GameObject selector;
        private UIController uiController;
        private OrbitController orbitController;
        public NormalOverlay(UIController uiController, OrbitController orbitController)
        {
            selector = GameObject.Find("Selector");
            this.uiController = uiController;
            this.orbitController = orbitController;

            Disable();
        }
        
        public override void Enable(Game game)
        {
            selector.SetActive(true);
        }

        public override void Disable()
        {
            selector.SetActive(false);
        }


        public override void OnTick(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            if (hoverPlanetID != -1)
                UpdatePlanetData(game, hoverPlanetID);
            else if (selectedPlanetID != -1)
                UpdatePlanetData(game, selectedPlanetID);
        }

        public override void UpdateSelected(Game game, int selectedPlanetID)
        {
            ShowSelectorAt(game, selectedPlanetID, 0);
        }

        public override void UpdateHover(Game game, int hoverPlanetID)
        {
            ShowSelectorAt(game, hoverPlanetID, 0);
        }

        public override void UpdateHoverAndSelected(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            UpdateHover(game, hoverPlanetID);
        }


        public override void PlanetHoverEnter(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            ShowSelectorAt(game, hoverPlanetID, 0);
        }
        
        public override void PlanetHoverExit(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            if (selectedPlanetID == -1)
            {
                selector.gameObject.SetActive(false);
                return;
            }
            
            ShowSelectorAt(game, selectedPlanetID, 0);
        }
        
        public override void PlanetSelectedEnter(Game game, int selectedPlanetID)
        {
            ShowSelectorAt(game, selectedPlanetID, 0);
        }
        public override void PlanetSelectedExit(Game game, int selectedPlanetID)
        {
            selector.gameObject.SetActive(false);
        }


        

        /// <summary>
        /// Moves the selector to outerring at the selected planet
        /// </summary>
        /// <param name="planetID"></param>
        private void ShowSelectorAt(Game game, int planetID, float dt)
        {
            selector.gameObject.SetActive(true);
            UpdatePlanetData(game, planetID);

            
            
            Planet planet = game.GetPlanet(planetID);
            selector.transform.position = orbitController.GetPlanetPositionAtTickF(game, planetID, game.Ticks + dt);;
            selector.transform.localScale = Vector3.one * (Mathf.Log(planet.Size * 2 + Mathf.Exp(1)) * 2);
        }

        private void UpdatePlanetData(Game game, int planetID)
        {
            
            long totalMigration = 0;
            for (int i = 0; i < game.N; i++) totalMigration += game.MigrationSystem.PlanetImmigration[i, planetID] - game.MigrationSystem.PlanetImmigration[planetID, i];

            uiController.UIPlanetController.SetPlanet(game.PlanetNames[planetID],
                game.PlanetPopulations[planetID],
                game.PopulationGrowthSystem.PlanetBirths[planetID] - game.PopulationGrowthSystem.PlanetDeaths[planetID],
                game.PopulationGrowthSystem.PlanetBirths[planetID],
                game.PopulationGrowthSystem.PlanetDeaths[planetID],
                totalMigration,
                game.MigrationSystem.PlanetAttraction[planetID],
                game.SpaceflightSystem.SpacecraftPools[planetID].Count);
            
            
            // Levels (HACK)
            List<ElementCount> elements = new List<ElementCount> 
                { new("Fe", 100_000), new("Si", 30), new("MJ", 200_000)};
            List<LevelCount> levels = new List<LevelCount> 
                { new("Population", 2), new("Social", 3), new("Infrastructure", 4) };
                
            // Update UIS
            uiController.UIPlanetController.UpdateMaterials(elements);
            uiController.UIPlanetController.UpdateLevels(levels);
            uiController.UIPlanetController.UpdateTransfers(planetID, game.PlanetNames, game.HohmannTransfers, game.HohmannDeltaV);
            uiController.UIPlanetController.UpdateMigration(planetID, game.PlanetNames, game.MigrationSystem.PlanetImmigration);
        }

    }


}