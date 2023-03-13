using Bserg.Controller.Drivers;
using Bserg.Controller.Material;
using Bserg.Controller.Sensors;
using Bserg.Controller.Tools;
using Bserg.Controller.UI.Planet;
using Bserg.Model.Core;
using Bserg.Model.Space;
using UnityEngine.UIElements;

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
        private int previousPopulation;
        private BuildSensor buildSensor;
        private BuildDriver buildDriver;
        
        public UIPlanetController(UIDocument uiDocument, Game game)
        {
            this.uiDocument = uiDocument;
            planetLevels = game.PlanetLevels;
            
            
            LevelStyle.Load();
            ElementStyle.Load();
            
            MigrationUI = new MigrationUI(GetUI("migration-view"));
            TransferUI = new TransferUI(GetUI("transfer-view"));
            BuildUI = new BuildUI(GetUI("build-view"));
            buildSensor = new BuildSensor(BuildUI, game.BuildSystem, game.PlanetLevels, game.LevelProgress);
            buildDriver = new BuildDriver(game.BuildOperator, buildSensor);
            
            LevelUI = new LevelUI(GetUI("level-view"));
            
            PlanetUI = new PlanetUI(GetUI("planet-view"), BuildUI, buildSensor);
        }

        public VisualElement GetUI(string name) => uiDocument.rootVisualElement.Q<VisualElement>(name);

        /// <summary>
        /// Called whenever the selected or visualised planet has changed
        /// </summary>
        /// <param name="planetID"></param>
        public void SetFocusedPlanet(int planetID)
        {
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
        
        
        public void SetFocusedPlanet(string name, long spacecraftPoolCount, int planetID, float populationProgress)
        {
            PlanetUI.Update(name, spacecraftPoolCount, planetLevels, planetID, populationProgress);
            buildSensor.OnTick();
            
            int pop = planetLevels.Get("Population")[planetID];
            if (previousPopulation != pop)
            {
                previousPopulation = pop;
                buildSensor.RedrawBuildLevels();
            }
        }








    }
}
