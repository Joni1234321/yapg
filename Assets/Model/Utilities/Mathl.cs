using System.Runtime.InteropServices;
using UnityEngine;

namespace Model.Utilities
{
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Mathl
    {
        
        public static long Max (long a, long b) => a > b ? a : b; 
        public static long Min (long a, long b) => a > b ? b : a; 
    }

    public struct Mathb
    {

        // https://en.wikipedia.org/wiki/Sigmoid_function
        // https://www.reddit.com/r/gamedesign/comments/96fa6f/if_you_are_making_an_rpg_you_need_to_know_the/
        // ANOTHER VERSION IS x / (1 + ABS(x))
        /// <summary>
        /// Returns s curve value between 0 and 1
        /// 0 = .50
        /// 1 = .73
        /// 2 = .88
        ///
        ///
        /// Steepness of 2 makes this become
        /// 0 = .50
        /// 1 = .62
        /// 2 = .73
        /// 4 = .88
        /// </summary>
        /// <param name="x">x</param>
        /// <param name="steepness">Multiplying x</param>
        /// <returns>Value beetween 0 and 1</returns>
        public static float Sigmoid(float x) => 1f / (1f + Mathf.Exp(x));

        public static float Logistic(float x) => 1f / (1f + Mathf.Exp(x));
        /// <summary>
        /// 0 = 0
        /// 1 = 0.46
        /// 2 = -0.46
        /// </summary>
        /// <param name="x"></param>
        /// <returns>[-1;1]</returns>
        public static float LogisticCentered(float x) => 2f / (1f + Mathf.Exp(x)) - 1;
    }
}