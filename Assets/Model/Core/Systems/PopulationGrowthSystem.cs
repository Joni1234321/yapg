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
        public long[] PlanetBirths, PlanetDeaths;

        public PopulationGrowthSystem(Game game) : base(game)
        {
            PlanetBirths = new long[Game.N];
            PlanetDeaths = new long[Game.N];
        }
        
        public void System()
        {
            const float BirthRate = 0.01f;
            const float DeathRate = 0.005f;
            for (int i = 0; i < Game.N; i++)
            {
                long oldPopulation = Game.PlanetPopulations[i];
                
                // Death rate
                PlanetDeaths[i] = (long)(oldPopulation * DeathRate);

                // Birth rate
                PlanetBirths[i] = (long)(oldPopulation * BirthRate);

                // New population
                Game.PlanetPopulations[i] += PlanetBirths[i] - PlanetDeaths[i];
            }
            
        }
    }
}