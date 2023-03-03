using System.Collections.Generic;

namespace Bserg.Model.Space
{
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
    }
}