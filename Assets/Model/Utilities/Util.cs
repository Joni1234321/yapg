using UnityEngine;

namespace Model.Utilities
{
    public static class Util
    {
        // if for every 10, we need 3, then the base is 10^(1/3) = 2.15443469003, then base^3 = 10
        //const float BASE = 2.15443469003f;
        private const float BASE = 2f;
        const float INVERSE_BASE = 1f / BASE;
        static readonly float LOG_BASE = Mathf.Log10(BASE), INVERSE_LOG_BASE = 1f / LOG_BASE;
        
        /// <summary>
        /// Converts real value to level
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static float LongToLevel(long val)
        {
            if (val == 0)
                return 0;
            return Mathf.Log10(val) * INVERSE_LOG_BASE;
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
            
            return (long)Mathf.Pow(10, level * LOG_BASE);
        }

    }
}