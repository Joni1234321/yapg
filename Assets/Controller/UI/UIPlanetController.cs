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
        
        public UIPlanetController(UIDocument uiDocument)
        {
            this.uiDocument = uiDocument;
            
            LevelStyle.Load();
            ElementStyle.Load();
            
            MigrationUI = new MigrationUI(GetUI("migration-view"));
            TransferUI = new TransferUI(GetUI("transfer-view"));
            BuildUI = new BuildUI(GetUI("build-view"));
            LevelUI = new LevelUI(GetUI("level-view"));
            
            PlanetUI = new PlanetUI(GetUI("planet-view"), BuildUI);
        }

        public VisualElement GetUI(string name) => uiDocument.rootVisualElement.Q<VisualElement>(name); 



        public void SetPlanet(string name, long spacecraftPoolCount, PlanetLevels planetLevels, int planetID, float populationProgress)
        {
            PlanetUI.Update(name, spacecraftPoolCount, planetLevels, planetID, populationProgress);
            BuildUI.UpdateBuild(planetLevels.Get("Housing")[planetID]);
        }








    }
}
