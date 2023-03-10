using Bserg.Controller.UI.Planet;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using UnityEngine.UIElements;

namespace Bserg.Controller.Sensors
{ 
    /// <summary>
    /// Sensor for build system
    /// </summary>
    public class BuildSensor : GameSensor
    {

        public readonly BuildUI UI;
        public readonly BuildSystem BuildSystem;
        public readonly PlanetLevels PlanetLevels;
        
        public Recipe CurrentRecipe { get; private set; }
        public int CurrentPlanetID = -1;

        
        private readonly PlanetLevelsGeneric<float> planetProgress;

        public BuildSensor(BuildUI buildUI, BuildSystem buildSystem, PlanetLevels planetLevels, PlanetLevelsGeneric<float> planetProgress)
        { 
            
            PlanetLevels = planetLevels;
            this.planetProgress = planetProgress;

            UI = buildUI;
            BuildSystem = buildSystem;

            ChangeRecipe(Recipe.Get("Food"), false);
        }

        public override void OnTick()
        {
            if (UI.IsProgressBar)
                UI.ChangeProgressValue(planetProgress.Get(CurrentRecipe.Output[0].Name)[CurrentPlanetID] * 100);
        }
        
        /// <summary>
        /// Changes the currently focused recipe
        /// </summary>
        /// <param name="newRecipe"></param>
        /// <param name="update"></param>
        public void ChangeRecipe(Recipe newRecipe, bool update = true)
        {
            CurrentRecipe = newRecipe;
            DrawRecipe(CurrentRecipe);
            if (update) RedrawBuildLevels();
        }

    
        protected override void OnNewFocusedPlanet(int planetID)
        {
            // Get the current recipe for the selected planet
            CurrentPlanetID = planetID;
            RedrawBuildLevels();
        }
        
        protected override void OnNoPlanetFocuesed()
        {
            // Just set current to 0, to indicate none is selected
            RedrawBuildLevels(CurrentRecipe, 0);
        }


        /// <summary>
        /// Draw the ui with the given recipe
        /// </summary>
        /// <param name="recipe"></param>
        public void DrawRecipe(Recipe recipe)
        {
            EventCallback<ClickEvent>[] inputCallbacks = new EventCallback<ClickEvent>[recipe.Input.Length];
            EventCallback<ClickEvent>[] outputCallbacks = new EventCallback<ClickEvent>[recipe.Output.Length];
            
            for (int i = 0; i < inputCallbacks.Length; i++)
            {
                string name = recipe.Input[i].Name;
                Recipe newRecipe = Recipe.Get(name);
                inputCallbacks[i] = _ => ChangeRecipe(newRecipe);
            }

            for (int i = 0; i < outputCallbacks.Length; i++)
            {
                string name = recipe.Output[i].Name;
                Recipe newRecipe = Recipe.Get(name);
                outputCallbacks[i] = _ => ChangeRecipe(newRecipe);
            }

            UI.DrawRecipe(recipe, inputCallbacks, outputCallbacks);
        }
        
        public void RedrawBuildLevels() => RedrawBuildLevels(CurrentRecipe, PlanetLevels.Get(CurrentRecipe.Output[0].Name)[CurrentPlanetID]);

        /// <summary>
        /// Updates the UI's level count for items in recipe, but doesnt change the recipe
        /// </summary>
        /// <param name="recipe"></param>
        /// <param name="level"></param>
        public void RedrawBuildLevels(Recipe recipe, int level)
        {
            int[] inputConsumption = new int[recipe.Input.Length];
            int[] inputRemainingLevels = new int[recipe.Input.Length];
            int[] outputProduction = new int[recipe.Output.Length];
            
            for (int i = 0; i < inputConsumption.Length; i++)
            {
                inputConsumption[i] = recipe.Input[i].OffsetLevel + level;
                inputRemainingLevels[i] = BuildSystem.GetHighestLevel(recipe.Input[i].Name, CurrentPlanetID, inputConsumption[i]);
            }

            for (int i = 0; i < outputProduction.Length; i++)
            {
                outputProduction[i] = recipe.Output[i].OffsetLevel + level;
            }
            
            UI.RedrawLevelValues(inputConsumption, inputRemainingLevels, outputProduction);
        }
        
    }
}