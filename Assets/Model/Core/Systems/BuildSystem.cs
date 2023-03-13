using Bserg.Model.Space;
using UnityEngine;

namespace Bserg.Model.Core.Systems
{
    public class BuildSystem : GameSystem
    {
        public readonly PlanetLevelsGeneric<uint> ConsumptionMasks;
        private const int BIT_SIZE = sizeof(uint) * 8;
        private const uint MAX_USED = 1U << (BIT_SIZE - 1);

        
        public BuildSystem(Game game) : base(game)
        {
            ConsumptionMasks = new PlanetLevelsGeneric<uint>(game.N);
        }
        
        public int GetHighestLevel(string name, int planetID)
        {
            int production = Game.PlanetLevels.Get(name)[planetID];
            return GetHighestLevel(ConsumptionMasks.Get(name)[planetID], production);
        }
        public int GetHighestLevel(string name, int planetID, int currentConsumption)
        {
            int production = Game.PlanetLevels.Get(name)[planetID];
            uint currentConsumptionMask = GenerateConsumptionMask(production, currentConsumption);
            
            return GetHighestLevel(ConsumptionMasks.Get(name)[planetID] - currentConsumptionMask, production);
        }
        // ReSharper disable Unity.PerformanceAnalysis
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
            uint consumptionMask = ConsumptionMasks.Get(name)[planetID];

            // Cases 
            // if next lvel is higher than planet level
            if (newConsumption > production)
                return false;
            
            uint currentMask = GenerateConsumptionMask(production, currentConsumption);
            uint nextMask = GenerateConsumptionMask(production, newConsumption);
            uint nextConsumptionMask = consumptionMask - currentMask + nextMask;
            
            // Check for overflow
            if (nextConsumptionMask < consumptionMask || nextConsumptionMask > MAX_USED)
                return false;

            return true;
        }

        
        
        /// <summary>
        /// Returns a uint with one bit set based on how close it is to the planetlevel
        /// 1 << 31 is when there is no level differnce
        /// 0 means that it is less than 31 levels
        /// </summary>
        /// <param name="production"></param>
        /// <param name="consumption"></param>
        /// <returns></returns>
        public uint GenerateConsumptionMask(int production, int consumption)
        {
            int diff = production - consumption;
            if (diff >= BIT_SIZE)
                return 0;
            
            return MAX_USED >> diff;
        }
        
        
    }
}