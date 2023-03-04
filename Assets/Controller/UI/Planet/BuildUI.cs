using System.Collections.Generic;
using Bserg.Controller.Tools;
using Bserg.Model.Space;
using Bserg.View.Custom.Counter;
using Bserg.View.Custom.Level;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class BuildUI : UIClass
    {
        private readonly VisualElement inputList, outputList, upgrade, downgrade;
        private List<LevelGroupControl> inputs, outputs;

        public Recipe CurrentRecipe;

        private readonly PlanetLevels planetLevels;
        private int currentPlanetID = -1;

        public BuildUI(VisualElement ui, PlanetLevels planetLevels) : base(ui)
        {
            this.planetLevels = planetLevels;
            
            inputList = ui.Q<VisualElement>("input-list");
            outputList = ui.Q<VisualElement>("output-list");
            upgrade = ui.Q<VisualElement>("upgrade");
            downgrade = ui.Q<VisualElement>("downgrade");
            inputs = inputList.Query<LevelGroupControl>().ToList();
            outputs = outputList.Query<LevelGroupControl>().ToList();
            
            upgrade.RegisterCallback<ClickEvent>(_ => Upgrade());
            downgrade.RegisterCallback<ClickEvent>(_ => Downgrade());
        }

        protected override void OnNewSelectedPlanet(int planetID)
        {
            currentPlanetID = planetID;
            UpdateBuild();
        }

        protected override void OnDeselectPlanet()
        {
            UpdateBuild(0);
        }

        /// <summary>
        /// Changes the look of the buildmenu
        /// </summary>
        /// <param name="recipe"></param>
        public void ChangeRecipe(Recipe recipe, int currentLevel)
        {
            CurrentRecipe = recipe;
            
            inputList.Clear();
            outputList.Clear();
            inputs.Clear();
            outputs.Clear();

            for (int i = 0; i < recipe.Input.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Input[i].Name);
                inputList.Add(group);
                inputs.Add(group);
            }
            
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Output[i].Name);
                outputList.Add(group);
                outputs.Add(group);
            }
            UpdateBuild(currentLevel);
        }

        public void UpdateBuild() => UpdateBuild(planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]);
                
        /// <summary>
        /// Updates the UI's level count for items in recipe, but doesnt change the recipe
        /// </summary>
        /// <param name="level"></param>
        public void UpdateBuild(int level)
        {
            for (int i = 0; i < CurrentRecipe.Input.Length; i++)
                inputs[i].Level = (CurrentRecipe.Input[i].Level + level).ToString();
            
            for (int i = 0; i < CurrentRecipe.Output.Length; i++)
                outputs[i].Level = (CurrentRecipe.Output[i].Level + level).ToString();
        }

        /// <summary>
        /// Upgrades the level count on the current recipe
        /// </summary>
        void Upgrade()
        {
            planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]++;
            UpdateBuild();
        }

        /// <summary>
        /// Downgrades the current level count on the current recipe
        /// </summary>
        void Downgrade()
        {
            planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]--;
            UpdateBuild();
        }
        
        
        /// <summary>
        /// Creates a group given the name, by looking up its levelstyle
        /// </summary>
        public LevelGroupControl CreateLevelGroup(string name, bool reverse = false, bool progressEnabled = false, string slang = null)
        {
            LevelGroupControl group = new LevelGroupControl();

            LevelStyle style = LevelStyle.Get(name);
            group.Level = "X";
            group.ProgressEnabled = progressEnabled;
            group.Reverse = reverse;
            group.Text = style.Name;
            group.BackgroundColor = style.Color;

            if (slang != null)
                group.Text = slang;
            
            group.RegisterCallback<ClickEvent>(_ => ChangeRecipe(Recipe.Get(style.Name), 0));
            return group;
        }

    }
}