using Bserg.Controller.Material;
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
        private int selectedPlanetID;
        private int previousPopulation;
        
        public UIPlanetController(UIDocument uiDocument, Game game)
        {
            this.uiDocument = uiDocument;
            planetLevels = game.PlanetLevels;
            
            LevelStyle.Load();
            ElementStyle.Load();
            
            MigrationUI = new MigrationUI(GetUI("migration-view"));
            TransferUI = new TransferUI(GetUI("transfer-view"));
            BuildUI = new BuildUI(GetUI("build-view"), game.PlanetLevels, game.LevelProgress, game.BuildSystem);
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
            BuildUI.OnTick();
            int pop = planetLevels.Get("Population")[planetID];
            if (previousPopulation != pop)
            {
                previousPopulation = pop;
                BuildUI.UpdateBuild();
            }
        }








    }
}
