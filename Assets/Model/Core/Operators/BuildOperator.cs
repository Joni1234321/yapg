using Bserg.Model.Core.Systems;
using Bserg.Model.Space;

namespace Bserg.Model.Core.Operators
{
    public class BuildOperator : GameOperator
    {
        private readonly BuildSystem buildSystem;

        public BuildOperator(Game game) : base(game)
        {
            buildSystem = game.BuildSystem;
        }

        
        public void SetRecipeLevel(Recipe recipe, int recipeLevel, int planetID)
        {
            // Set input effect
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                uint nextConsumptionMask = buildSystem.GenerateConsumptionMask(
                    Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID], recipeLevel + recipe.Input[i].OffsetLevel);
                buildSystem.ConsumptionMasks.Get(recipe.Input[i].Name)[planetID] += nextConsumptionMask;

            }

            // Set output
            for (int i = 0; i < recipe.Output.Length; i++)
                Game.PlanetLevels.Get(recipe.Output[i].Name)[planetID] = recipeLevel + recipe.Output[i].OffsetLevel;
        }
        

        public bool Downgrade(Recipe recipe, int decreaseInLevels, int planetID)
        {
            int planetLevel = Game.PlanetLevels.Get(recipe.Output[0].Name)[planetID];
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                // Set new mask
                int inputLevel = Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID];
                uint consumptionMask = buildSystem.ConsumptionMasks.Get(recipe.Input[i].Name)[planetID];
                uint currentLevelMask = buildSystem.GenerateConsumptionMask(inputLevel, planetLevel + recipe.Input[i].OffsetLevel);
                uint nextLevelMask = buildSystem.GenerateConsumptionMask(inputLevel, planetLevel + recipe.Input[i].OffsetLevel - decreaseInLevels);
                uint nextConsumptionMask = consumptionMask + (nextLevelMask - currentLevelMask);
                buildSystem.ConsumptionMasks.Get(recipe.Input[i].Name)[planetID] = nextConsumptionMask;
            }
            
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                Game.PlanetLevels.Get(recipe.Output[i].Name)[planetID] -= decreaseInLevels;
            }
            
            return true;
        }
        public bool Upgrade(Recipe recipe, int increaseInLevels, int planetID)
        {
            int productionLevel = Game.PlanetLevels.Get(recipe.Output[0].Name)[planetID];
            if (recipe.Input.Length == 0)
                return false;
            
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                if (!buildSystem.CanIncreaseConsumption(recipe.Input[i].Name, planetID, productionLevel + recipe.Input[i].OffsetLevel, productionLevel + recipe.Input[i].OffsetLevel + increaseInLevels))
                    return false;
            }
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                // Set new mask
                int inputProductionLevel = Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID]; // 10
                int consumption = productionLevel + recipe.Input[i].OffsetLevel;
                uint usedMask = buildSystem.ConsumptionMasks.Get(recipe.Input[i].Name)[planetID];  // 0
                uint currentLevelMask = buildSystem.GenerateConsumptionMask(inputProductionLevel, consumption);
                uint nextLevelMask = buildSystem.GenerateConsumptionMask(inputProductionLevel, consumption + increaseInLevels);
                uint nextUsedMask = usedMask + (nextLevelMask - currentLevelMask);
                buildSystem.ConsumptionMasks.Get(recipe.Input[i].Name)[planetID] = nextUsedMask;
            }

            // Set new levels
            for (int i = 0; i < recipe.Output.Length; i++)
            {
                Game.PlanetLevels.Get(recipe.Output[i].Name)[planetID] += increaseInLevels;
            }
            
            return true;
        }


        /// <summary>
        /// Called whenever a production is increased, then reduce the consumption mask
        /// </summary>
        /// <param name="name"></param>
        /// <param name="planetID"></param>
        /// <param name="increase"></param>
        public void OnProductionIncreased(string name, int planetID, int increase)
        {
            buildSystem.ConsumptionMasks.Get(name)[planetID] >>= increase;
        }
    }
}