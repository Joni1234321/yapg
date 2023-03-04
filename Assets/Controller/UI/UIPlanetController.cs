using System.Collections.Generic;
using Bserg.Controller.Material;
using Bserg.Controller.Tools;
using Bserg.Controller.UI.Planet;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Custom.Counter;
using Bserg.View.Custom.Field;
using Bserg.View.Custom.Level;
using Bserg.View.Custom.Transfer;
using Bserg.View.Space;
using Model.Utilities;
using UnityEngine;
using UnityEngine.UIElements;
using Time = Bserg.Model.Units.Time;

namespace Bserg.Controller.UI
{
    public class UIPlanetController
    {
        private UIDocument uiDocument;
        
        
        // UI
        public MigrationUI MigrationUI;
        public TransferUI TransferUI;
        public BuildUI BuildUI;
        public PlanetUI PlanetUI;
        public LevelUI LevelUI;

        private PlanetLevels planetLevels;
        private int selectedPlanetID;
        private int previousPopulation;
        
        public UIPlanetController(UIDocument uiDocument, PlanetLevels planetLevels)
        {
            this.uiDocument = uiDocument;
            this.planetLevels = planetLevels;
            
            LevelStyle.Load();
            ElementStyle.Load();
            
            MigrationUI = new MigrationUI(GetUI("migration-view"));
            TransferUI = new TransferUI(GetUI("transfer-view"));
            BuildUI = new BuildUI(GetUI("build-view"), planetLevels);
            LevelUI = new LevelUI(GetUI("level-view"));
            
            PlanetUI = new PlanetUI(GetUI("planet-view"), BuildUI);
        }

        public VisualElement GetUI(string name) => uiDocument.rootVisualElement.Q<VisualElement>(name);

        /// <summary>
        /// Called whenever the selected or visualised planet has changed
        /// </summary>
        /// <param name="planetID"></param>
        public void SetPlanet(int planetID)
        {
            if (planetID == selectedPlanetID)
                return;

            selectedPlanetID = planetID;
            
            if (planetID < 0)
            {
                UIClass.HideAll();
                UIClass.ClearSelectedPlanet();
            }
            else
            {
                UIClass.ShowAll();
                UIClass.SetSelectedPlanet(planetID);
                previousPopulation = planetLevels.Get("Population")[planetID];
            }
        }
        
        
        public void SetPlanet(string name, long spacecraftPoolCount, int planetID, float populationProgress)
        {
            PlanetUI.Update(name, spacecraftPoolCount, planetLevels, planetID, populationProgress);
            
            int pop = planetLevels.Get("Population")[planetID];
            if (previousPopulation != pop)
            {
                previousPopulation = pop;
                BuildUI.UpdateBuild();
            }
        }








    }
}
