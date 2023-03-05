using System.Collections.Generic;
using System.Linq;

namespace Bserg.Model.Space
{
    /// <summary>
    /// Contains dictionary where it is possible to refer to the given level with a string
    /// </summary>
    public class PlanetLevels
    {
        public readonly int N;
        public PlanetLevels(int n)
        {
            N = n;
            foreach (string s in new string[] { "Population", "Land", "Housing", "Food"})
            {
                levels.Add(s, new int[n]);
            }
        }

        private readonly Dictionary<string, int[]> levels = new();

        public int[] Get(string name) => levels[name];

        /// <summary>
        /// Returns all stats
        /// </summary>
        /// <returns></returns>
        public int[][] GetAll() => levels.Values.ToArray();

        public string[] GetAllNames() => levels.Keys.ToArray();
    }

    public class PlanetLevelsGeneric<T>
    {
        public readonly int N;
        public PlanetLevelsGeneric(int n)
        {
            N = n;
            foreach (string s in new string[] { "Population", "Land", "Housing", "Food"})
            {
                levels.Add(s, new T[n]);
            }
        }

        private readonly Dictionary<string, T[]> levels = new();

        public T[] Get(string name) => levels[name];

        /// <summary>
        /// Returns all stats
        /// </summary>
        /// <returns></returns>
        public T[][] GetAll() => levels.Values.ToArray();
    }
}