using System.Collections.Generic;
using Bserg.Controller.Tools;
using Bserg.Model.Space;
using Bserg.View.Custom.Level;
using Bserg.View.Custom.Progress;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class BuildUI : UIClass
    {
        private readonly VisualElement inputList, outputList, inputRemainingList, inputSection;
        public readonly VisualElement Upgrade, Downgrade;
        private List<LevelGroupControl> inputs, outputs;
        private List<LevelControl> inputRemaining;
        private ProgressControl outputProgress;

        public bool IsProgressBar;


        public BuildUI(VisualElement ui) : base(ui)
        {
            inputList = ui.Q<VisualElement>("input-list");
            inputSection = ui.Q<VisualElement>("input");
            outputList = ui.Q<VisualElement>("output-list");
            inputRemainingList = ui.Q<VisualElement>("input-remaining");
            
            
            Upgrade = ui.Q<VisualElement>("upgrade");
            Downgrade = ui.Q<VisualElement>("downgrade");
            
            
            inputs = inputList.Query<LevelGroupControl>().ToList();
            outputs = outputList.Query<LevelGroupControl>().ToList();
            inputRemaining = inputRemainingList.Query<LevelControl>().ToList();

            outputProgress = ui.Q<ProgressControl>("output-progress");
            
        }

        public void ChangeProgressValue(float val)
        {
            outputProgress.Value = val;
        }

        /// <summary>
        /// Redraws the values of the levels
        /// </summary>
        /// <param name="inputLevels"></param>
        /// <param name="inputRemainingLevels"></param>
        /// <param name="outputProduction"></param>
        public void RedrawLevelValues(int[] inputLevels, int[] inputRemainingLevels, int[] outputProduction)
        {
            for (int i = 0; i < inputLevels.Length; i++)
            {
                inputs[i].Level = inputLevels[i].ToString();
                inputRemaining[i].Level = inputRemainingLevels[i].ToString();
            }

            for (int i = 0; i < outputProduction.Length; i++)
                outputs[i].Level = outputProduction[i].ToString();
        }


        /// <summary>
        /// Redraws recipe items
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="inputCallbacks">callback when click on input</param>
        /// <param name="outputCallbacks">callback when click on output</param>
        public void DrawRecipe(Recipe recipe, EventCallback<ClickEvent>[] inputCallbacks, EventCallback<ClickEvent>[] outputCallbacks)
        {
            bool isFixed = recipe.Input.Length == 0;
            IsProgressBar = isFixed && recipe.Output[0].Name == "Population";
            
            inputList.Clear();
            outputList.Clear();
            inputRemainingList.Clear();
            
            inputs.Clear();
            outputs.Clear();
            inputRemaining.Clear();
            
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Input[i].Name, inputCallbacks[i]);
                inputList.Add(group);
                inputs.Add(group);

                LevelControl remainingLevel = CreateLevel(recipe.Input[i].Name);
                remainingLevel.AddToClassList("fake-group");
                inputRemainingList.Add(remainingLevel);
                inputRemaining.Add(remainingLevel);
            }
            
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                LevelGroupControl group = CreateLevelGroup(recipe.Output[i].Name, outputCallbacks[i]);
                outputList.Add(group);
                outputs.Add(group);
                outputProgress.Fill = group.BackgroundColor;
            }

            
            outputProgress.style.display = IsProgressBar ? DisplayStyle.Flex : DisplayStyle.None;
            
            Upgrade.style.display = isFixed ? DisplayStyle.None : DisplayStyle.Flex;
            Downgrade.style.display = isFixed ? DisplayStyle.None : DisplayStyle.Flex;
            inputSection.style.display = isFixed ? DisplayStyle.None : DisplayStyle.Flex;
            inputRemainingList.style.display = isFixed ? DisplayStyle.None : DisplayStyle.Flex;

        }

        
        
        /// <summary>
        /// Creates a group given the name, by looking up its levelstyle
        /// </summary>
        public LevelGroupControl CreateLevelGroup(string name, EventCallback<ClickEvent> clickCallback, bool reverse = false, bool progressEnabled = false, string slang = null)
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
            
            group.RegisterCallback(clickCallback);
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