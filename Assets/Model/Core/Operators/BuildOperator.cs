using Bserg.Model.Core.Systems;
using Bserg.Model.Space;
using UnityEngine;

namespace Bserg.Model.Core.Operators
{
    public class BuildOperator : GameOperator
    {

        private readonly PlanetLevelsGeneric<uint> consumptionMasks;
        
        private const int BIT_SIZE = sizeof(uint) * 8;
        private const uint MAX_USED = 1U << (BIT_SIZE - 1);
        public BuildOperator(Game game) : base(game)
        {
            consumptionMasks = new PlanetLevelsGeneric<uint>(game.N);
        }


        /// <summary>
        /// Returns a uint with one bit set based on how close it is to the planetlevel
        /// 1 << 31 is when there is no level differnce
        /// 0 means that it is less than 31 levels
        /// </summary>
        /// <param name="production"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        private uint GetConsumptionMask(int production, int consumption)
        {
            int diff = production - consumption;
            if (diff >= BIT_SIZE)
                return 0;
            
            return MAX_USED >> diff;
        }

        public int GetHighestLevel(string name, int planetID)
        {
            int production = Game.PlanetLevels.Get(name)[planetID];
            return GetHighestLevel(consumptionMasks.Get(name)[planetID], production);
        }

        public int GetHighestLevel(string name, int planetID, int currentConsumption)
        {
            int production = Game.PlanetLevels.Get(name)[planetID];
            uint currentConsumptionMask = GetConsumptionMask(production, currentConsumption);
            
            return GetHighestLevel(consumptionMasks.Get(name)[planetID] - currentConsumptionMask, production);
        }
        public int GetHighestLevel(uint consumptionMask, int production)
        {
            // Edge case
            if (consumptionMask == 0)
                return production;
            
            // Max level
            if (consumptionMask == MAX_USED)
                return production - BIT_SIZE;

            // Start at 2nd bit
            int i = 1;
            // Masks
            uint mask = MAX_USED >> i;

            for (; i < BIT_SIZE; i++)
            {
                // if just one other consumption thing is on the stuff
                if (consumptionMask == mask)
                    return production - i;
                
                // if there is multiple 
                if ((consumptionMask & mask) == 0)
                    return production - i;

                mask >>= 1;
            }

            
            Debug.LogError("HIGHEST LEVEL METHOD HAS FAILED, ITERATED THROUGH ALL");
            return production - BIT_SIZE;
        }

        public void SetRecipeLevel(Recipe recipe, int planetID, int recipeLevel)
        {
            for (int i = 0; i < recipe.Input.Length; i++)
                consumptionMasks.Get(recipe.Input[i].Name)[planetID] += GetConsumptionMask(
                    Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID], recipeLevel + recipe.Input[i].OffsetLevel);

            for (int i = 0; i < recipe.Output.Length; i++)
                Game.PlanetLevels.Get(recipe.Output[i].Name)[planetID] = recipeLevel + recipe.Output[i].OffsetLevel;
        }
        
        /// <summary>
        /// Checks whether or not the planetlevel will be high enough
        /// </summary>
        /// <param name="name"></param>
        /// <param name="planetID"></param>
        /// <param name="currentConsumption"></param>
        /// <param name="newConsumption"></param>
        /// <returns></returns>
        public bool CanIncreaseConsumption(string name, int planetID, int currentConsumption, int newConsumption)
        { 
            int production = Game.PlanetLevels.Get(name)[planetID];
            uint consumptionMask = consumptionMasks.Get(name)[planetID];

            // Cases 
            // if next lvel is higher than planet level
            if (newConsumption > production)
                return false;
            
            uint currentMask = GetConsumptionMask(production, currentConsumption);
            uint nextMask = GetConsumptionMask(production, newConsumption);
            uint nextConsumptionMask = consumptionMask - currentMask + nextMask;
            
            // Check for overflow
            if (nextConsumptionMask < consumptionMask || nextConsumptionMask > MAX_USED)
                return false;

            return true;
        }

        public bool DownGrade(Recipe recipe, int decreaseInLevels, int planetID)
        {
            int planetLevel = Game.PlanetLevels.Get(recipe.Output[0].Name)[planetID];
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                // Set new mask
                int inputLevel = Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID];
                uint consumptionMask = consumptionMasks.Get(recipe.Input[i].Name)[planetID];
                uint currentLevelMask = GetConsumptionMask(inputLevel, planetLevel + recipe.Input[i].OffsetLevel);
                uint nextLevelMask = GetConsumptionMask(inputLevel, planetLevel + recipe.Input[i].OffsetLevel - decreaseInLevels);
                uint nextConsumptionMask = consumptionMask + (nextLevelMask - currentLevelMask);
                consumptionMasks.Get(recipe.Input[i].Name)[planetID] = nextConsumptionMask;
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
                if (!CanIncreaseConsumption(recipe.Input[i].Name, planetID, productionLevel + recipe.Input[i].OffsetLevel, productionLevel + recipe.Input[i].OffsetLevel + increaseInLevels))
                    return false;
            }
            for (int i = 0; i < recipe.Input.Length; i++)
            {
                // Set new mask
                int inputProductionLevel = Game.PlanetLevels.Get(recipe.Input[i].Name)[planetID]; // 10
                int consumption = productionLevel + recipe.Input[i].OffsetLevel;
                uint usedMask = consumptionMasks.Get(recipe.Input[i].Name)[planetID];  // 0
                uint currentLevelMask = GetConsumptionMask(inputProductionLevel, consumption);
                uint nextLevelMask = GetConsumptionMask(inputProductionLevel, consumption + increaseInLevels);
                uint nextUsedMask = usedMask + (nextLevelMask - currentLevelMask);
                consumptionMasks.Get(recipe.Input[i].Name)[planetID] = nextUsedMask;
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
            consumptionMasks.Get(name)[planetID] >>= increase;
        }
    }
}