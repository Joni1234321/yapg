
using Unity.Mathematics;
using UnityEngine;

namespace Bserg.Model.Utilities
{
    public static class Util
    {
        // if for every 10, we need 3, then the base is 10^(1/3) = 2.15443469003, then base^3 = 10
        //const float BASE = 2.15443469003f;
        private const float BASE = 2f;
        const float INVERSE_BASE = 1f / BASE;
        static readonly float LOG_BASE = math.log10(BASE), INVERSE_LOG_BASE = 1f / LOG_BASE;
        
        /// <summary>
        /// Converts real value to level
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float LongToLevel(long val)
        {
            if (val == 0)
                return 0;
            return math.log10(val) * INVERSE_LOG_BASE;
        }
        /// <summary>
        /// Converts level to real value
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static long LevelToLong(float level)
        {
            if (level == 0)
                return 0;
            
            return (long)math.pow(10, level * LOG_BASE);
        }


        /// <summary>
        /// Returns the amount of level progress gained by adding levelincrease to planetlevel
        /// Example: adding a level 3 population to a planet with a level 3 population, results in a level 4 population so levelprogress here is 1 entire point
        /// adding a level 2 to level 3 is .5 points
        /// adding a level 4 to level 3 is 2 points
        /// </summary>
        /// <param name="planetLevel"></param>
        /// <param name="levelIncrease"></param>
        /// <returns></returns>
        public static float LevelProgress(int planetLevel, int levelIncrease)
        {
            return math.pow(2, levelIncrease - planetLevel);
        } 

    }
}