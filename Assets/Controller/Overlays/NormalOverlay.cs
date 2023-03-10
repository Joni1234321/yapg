﻿using Bserg.Controller.Core;
using Bserg.Controller.UI;
using Bserg.Model.Core;
using Bserg.Model.Space;
using UnityEngine;

namespace Bserg.Controller.Overlays
{
    public class NormalOverlay : Overlay
    {
        private GameObject selector;
        private UIController uiController;
        private PlanetController planetController;
        public NormalOverlay(UIController uiController, PlanetController planetController)
        {
            selector = GameObject.Find("Selector");
            this.uiController = uiController;
            this.planetController = planetController;

            Disable();
        }
        
        public override void Enable(Game game)
        {
            selector.SetActive(true);
        }

        public override void Disable()
        {
            HideSelector();
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
            ShowSelectorAt(game, selectedPlanetID);
        }

        public override void UpdateHover(Game game, int hoverPlanetID)
        {
            ShowSelectorAt(game, hoverPlanetID);
        }

        public override void UpdateHoverAndSelected(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            UpdateHover(game, hoverPlanetID);
        }


        public override void PlanetHoverEnter(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            ShowSelectorAt(game, hoverPlanetID);
        }
        
        public override void PlanetHoverExit(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            if (selectedPlanetID == -1)
            {
                HideSelector();
                return;
            }
            
            ShowSelectorAt(game, selectedPlanetID);
        }
        
        public override void PlanetSelectedEnter(Game game, int selectedPlanetID)
        {
            ShowSelectorAt(game, selectedPlanetID);
        }
        public override void PlanetSelectedExit(Game game, int selectedPlanetID)
        {
            HideSelector();
        }


        

        /// <summary>
        /// Moves the selector to outerring at the selected planet
        /// </summary>
        /// <param name="planetID"></param>
        private void ShowSelectorAt(Game game, int planetID)
        {
            selector.gameObject.SetActive(true);
            UpdatePlanetData(game, planetID);
            
            Planet planet = game.GetPlanet(planetID);
            
            selector.transform.position = planetController.GetPlanetPositionAtTickF(game, planetID, game.Ticks + DeltaTick);;
            selector.transform.localScale = planetController.SystemGenerator.GetIconPlanetSize(planet.Size);
        }

        /// <summary>
        /// Called whenever no planet is selected
        /// </summary>
        private void HideSelector()
        {
            selector.gameObject.SetActive(false);
            uiController.UIPlanetController.SetPlanet(-1);
        }

        private void UpdatePlanetData(Game game, int planetID)
        {
            uiController.UIPlanetController.SetPlanet(planetID);

            uiController.UIPlanetController.SetPlanet(
                game.PlanetNames[planetID],
                game.SpaceflightSystem.SpacecraftPools[planetID].Count,
                planetID,
                game.PlanetPopulationProgress[planetID] % 1f
                );
            
            
            //float totalMigration = 0;
            //for (int i = 0; i < game.N; i++) totalMigration += game.MigrationSystem.PlanetImmigration[i, planetID] - game.MigrationSystem.PlanetImmigration[planetID, i];

            // Levels (HACK)
            //List<ElementCount> elements = new List<ElementCount> 
            //    { new("Fe", 100_000), new("Si", 30), new("MJ", 200_000)};
            //List<LevelCount> levels = new List<LevelCount> 
            //    { new("Population", 2), new("Social", 3), new("Infrastructure", 4) };
                
            // Update UIS
            //uiController.UIPlanetController.UpdateMaterials(elements);
            //uiController.UIPlanetController.UpdateLevels(levels);
            //uiController.UIPlanetController.UpdateTransfers(planetID, game.PlanetNames, game.OrbitalTransferSystem.HohmannTransfers, game.OrbitalTransferSystem.HohmannDeltaV);
            //uiController.UIPlanetController.UpdateMigration(planetID, game.PlanetNames, game.MigrationSystem.PlanetImmigration);
        }

    }


}