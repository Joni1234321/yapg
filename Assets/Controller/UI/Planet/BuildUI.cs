﻿using System.Collections.Generic;
using Bserg.Controller.Tools;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using Bserg.View.Custom.Counter;
using Bserg.View.Custom.Level;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class BuildUI : UIClass
    {
        private readonly VisualElement inputList, outputList, inputRemainingList, upgrade, downgrade;
        private List<LevelGroupControl> inputs, outputs;
        private List<LevelControl> inputRemaining;

        public Recipe CurrentRecipe;

        private readonly PlanetLevels planetLevels;
        private readonly BuildSystem buildSystem;
        
        private int currentPlanetID = -1;

        public BuildUI(VisualElement ui, PlanetLevels planetLevels, BuildSystem buildSystem) : base(ui)
        {
            this.planetLevels = planetLevels;
            this.buildSystem = buildSystem;
            
            inputList = ui.Q<VisualElement>("input-list");
            outputList = ui.Q<VisualElement>("output-list");
            inputRemainingList = ui.Q<VisualElement>("input-remaining");
            
            
            upgrade = ui.Q<VisualElement>("upgrade");
            downgrade = ui.Q<VisualElement>("downgrade");
            inputs = inputList.Query<LevelGroupControl>().ToList();
            outputs = outputList.Query<LevelGroupControl>().ToList();
            inputRemaining = inputRemainingList.Query<LevelControl>().ToList();
            
            upgrade.RegisterCallback<ClickEvent>(_ => Upgrade());
            downgrade.RegisterCallback<ClickEvent>(_ => Downgrade());
            ChangeRecipe(Recipe.Get("Food"), false);
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
        public void ChangeRecipe(Recipe recipe, bool update = true)
        {
            CurrentRecipe = recipe;
            
            inputList.Clear();
            outputList.Clear();
            inputRemainingList.Clear();
            
            inputs.Clear();
            outputs.Clear();
            inputRemaining.Clear();
            
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Input[i].Name);
                inputList.Add(group);
                inputs.Add(group);

                LevelControl remainingLevel = CreateLevel(recipe.Input[i].Name);
                remainingLevel.AddToClassList("fake-group");
                inputRemainingList.Add(remainingLevel);
                inputRemaining.Add(remainingLevel);
            }
            
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Output[i].Name);
                outputList.Add(group);
                outputs.Add(group);
            }
            
            if (update) UpdateBuild();
        }

        public void UpdateBuild() => UpdateBuild(planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]);
                
        /// <summary>
        /// Updates the UI's level count for items in recipe, but doesnt change the recipe
        /// </summary>
        /// <param name="level"></param>
        public void UpdateBuild(int level)
        {
            for (int i = 0; i < CurrentRecipe.Input.Length; i++)
            {
                int consumption = CurrentRecipe.Input[i].OffsetLevel + level;
                inputs[i].Level = consumption.ToString();
                inputRemaining[i].Level = buildSystem.GetHighestLevel(CurrentRecipe.Input[i].Name, currentPlanetID, consumption).ToString();
            }
            
            for (int i = 0; i < CurrentRecipe.Output.Length; i++)
                outputs[i].Level = (CurrentRecipe.Output[i].OffsetLevel + level).ToString();
        }

        /// <summary>
        /// Upgrades the level count on the current recipe
        /// </summary>
        void Upgrade()
        {
            //planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]
            if (!buildSystem.Upgrade(CurrentRecipe, 1, currentPlanetID))
                Debug.Log("No Upgrade");
            //planetLevels.Get(CurrentRecipe.Output[0].Name)[currentPlanetID]++;
            UpdateBuild();
        }

        /// <summary>
        /// Downgrades the current level count on the current recipe
        /// </summary>
        void Downgrade()
        {
            if (!buildSystem.DownGrade(CurrentRecipe, 1, currentPlanetID))
                Debug.Log("No Downgrade");
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
            
            group.RegisterCallback<ClickEvent>(_ => ChangeRecipe(Recipe.Get(style.Name)));
            return group;
        }

        public LevelControl CreateLevel(string name, LevelControl.LevelSizeEnum size = LevelControl.LevelSizeEnum.Medium)
        {
            LevelControl level = new LevelControl();
            LevelStyle style = LevelStyle.Get(name);
            level.Level = "X";
            level.BackgroundColor = style.Color;
            return level;
        }
    }
}