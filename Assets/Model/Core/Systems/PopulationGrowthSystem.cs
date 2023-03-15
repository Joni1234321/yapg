using Bserg.Model.Space;
using UnityEngine;

namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Handle population growth and decline
    /// UPDATE TO TAKE CARE OF PEOPLE WHO CAN MAKE BIRTH AND PEOPLE WHO CAN DIE
    /// ALSO FACTOR IN WORK AND SUCH
    /// ALSO AVERAGE AGE
    /// </summary>
    public class PopulationGrowthSystem : GameSystem
    {
        public float[] PlanetBirths, PlanetDeaths;

        public PopulationGrowthSystem(Game game) : base(game)
        {
            PlanetBirths = new float[Game.N];
            PlanetDeaths = new float[Game.N];
        }
        
        public void System()
        {
            // 8 % birthrate approx (2.15 ^ .1 = 8 %) 
            const float BirthRate = 0.02f;
            const float DeathRate = 0.005f;

            int[] housingLevels = Game.PlanetLevels.Get("Housing");
            int[] populationLevels = Game.PlanetLevels.Get("Population");
            for (int i = 0; i < Game.N; i++)
            {  
                // Only applies to planets with populations
                if (!Game.Inhabited[i])
                    continue;
                
                // Death rate
                PlanetDeaths[i] = DeathRate;

                // Birth rate
                // Affected down to 25% by not enough housing
                float modifier = 1f;
                int housingDiff = populationLevels[i] - housingLevels[i];
                if (housingDiff > 0)
                    modifier *= 1f / Mathf.Min(housingDiff, 4);
                
                PlanetBirths[i] = BirthRate * modifier;

                // New population
                Game.PlanetPopulationProgress[i] += PlanetBirths[i] - PlanetDeaths[i];
                
                // NOW CHECK AND CHANGE PROGRESS
                while (Game.PlanetPopulationProgress[i] > 1)
                {
                    Game.PlanetPopulationProgress[i] = (Game.PlanetPopulationProgress[i] - 1) * .5f;
                    
                    // Auto Upgrade all, including population
                    Game.PlanetLevels.Get("Population")[i]++;
                    string[] allLevels = Game.PlanetLevels.GetAllNames();
                    for (int namesI = 0; namesI < allLevels.Length; namesI++)
                        Game.BuildOperator.Upgrade(Recipe.Get(allLevels[namesI]), 1, i);
                }

                while (Game.PlanetPopulationProgress[i] < 0)
                {
                    Game.PlanetPopulationProgress[i] = (Game.PlanetPopulationProgress[i] + 1) * 2f;
                    
                    // Auto Upgrade all, including population
                    int[][] allLevels = Game.PlanetLevels.GetAll();
                    for (int levelI = 0; levelI < allLevels.Length; levelI++)
                        allLevels[levelI][i]--;
                }
                    
            }
        }
        

    }
}