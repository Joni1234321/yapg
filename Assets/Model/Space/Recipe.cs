using UnityEngine;

namespace Bserg.Model.Space
{
    /// <summary>
    /// What it takes to make another Level (ie. 1 pop + 3 land gives 3 food)
    /// </summary>
    public struct Recipe
    {
        public RecipeItem[] Input, Output;

        /// <summary>
        /// Give levels as ratios, then it will adjust so that the first output item is 1 at level 1 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public Recipe(RecipeItem[] input, RecipeItem[] output)
        {
            Input = input;
            Output = output;

            if (output.Length == 0)
            {
                Debug.LogError("BuildRecipe");
                return;
            }


            // Adjust so that output 0 is level 0 at level 0
            int offset = Output[0].Level;

            for (int i = 0; i < Input.Length; i++)
                Input[i].Level -= offset;
            
            for (int i = 0; i < Output.Length; i++)
                Output[i].Level -= offset;
        }
    }

    /// <summary>
    /// item in a build recipe
    /// </summary>
    public struct RecipeItem
    {
        public readonly string Name;
        public int Level;

        public RecipeItem(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}