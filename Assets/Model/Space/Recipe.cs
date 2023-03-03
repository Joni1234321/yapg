using System.Collections.Generic;
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
        private Recipe(RecipeItem[] input, RecipeItem[] output)
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

        private static readonly Dictionary<string, Recipe> Recipes = new();
        public static Recipe Get(string name) => Recipes[name];

        public static void Load()
        {
            if (Recipes.Count != 0)
                return;

            // 1 Land is approx 5 Acres (that is what one person needs)
            // Recipe is 1 pop + 4 land = 4 food
            // Meaning that 1 pop feeds 2^3 = 8 pops
            Add("Food",
                new RecipeItem[] { new("Population", 1), new("Land", 4) },
                new RecipeItem[] { new("Food", 4) });

            /* https://www.ti.org/vaupdate17.html
 *Perhaps in response to Cox's comments, on June 20 the Sierra Club modified the web page to compare four different densities:
* Dense urban, which is 400 households per acre or slightly less than the "efficient urban" of the day before;
* Efficient urban, which is "only" 100 households per acre;
* Efficient suburban, which is 10 households per acre; and
* Sprawl, which the Sierra Club defines as one household per acre.
 * 
 */
            // 1 housing per population
            // With low density
            // 5 homes per acre -> 25 homes per land -> 100 people per land
            // 1 person per 64 (2^6) people, for maintainance -> 2 people for 1 land

            Add("Housing",
                new RecipeItem[] { new("Population", 1), new("Land", 0) },
                new RecipeItem[] { new("Housing", 7) });


            Add("Population");
            Add("Land");
        }

        static void Add(string name, RecipeItem[] inputs, RecipeItem[] outputs)
        {
            Recipes.Add(name, new Recipe(inputs, outputs));
        }

        // Add empty 
        static void Add(string name)
        {
            Recipes.Add(name, new Recipe(new RecipeItem[]{ }, new RecipeItem[] {new(name, 1)}));
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