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
        private readonly VisualElement inputList, outputList;
        private List<LevelGroupControl> inputs, outputs;
        private CounterControl buildLevel;

        private Recipe currentRecipe;

        public BuildUI(VisualElement ui) : base(ui)
        {            
            inputList = ui.Q<VisualElement>("input-list");
            outputList = ui.Q<VisualElement>("output-list");
            buildLevel = ui.Q<CounterControl>();
            inputs = inputList.Query<LevelGroupControl>().ToList();
            outputs = outputList.Query<LevelGroupControl>().ToList();
        }

        /// <summary>
        /// Changes the look of the buildmenu
        /// </summary>
        /// <param name="recipe"></param>
        public void ChangeRecipe(Recipe recipe, int currentLevel)
        {
            currentRecipe = recipe;
            
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
        
                
        /// <summary>
        /// Updates the UI's level count for items in recipe
        /// </summary>
        /// <param name="level"></param>
        public void UpdateBuild(int level)
        {
            for (int i = 0; i < currentRecipe.Input.Length; i++)
                inputs[i].Level = (currentRecipe.Input[i].Level + level).ToString();
            
            for (int i = 0; i < currentRecipe.Output.Length; i++)
                outputs[i].Level = (currentRecipe.Output[i].Level + level).ToString();
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