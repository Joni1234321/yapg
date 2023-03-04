using Bserg.Model.Space.SpaceMovement;
using UnityEngine;

namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Handle population migrations
    /// </summary>
    public class MigrationSystem : GameSystem
    {
        // Migration
        public float[] PlanetAttraction;

        // List of people from where to where they want to go, and the amount of people
        public float[,] PlanetImmigration;

        public MigrationSystem(Game game) : base(game)
        {
            PlanetAttraction = new float[Game.N];
            PlanetImmigration = new float[Game.N,Game.N];
        }

        public void System()
        {
            const int NormalAttractionLevel = 5;
            const int MaxAttractionFromPopulation = 12, MinAttractionFromPopulation = -2;

            // Attraction
            for (int planetID = 0; planetID < Game.N; planetID++)
            {
                // If population is above carrying capacity
                // If there is space for 100 times the population then its gonna max out at 12
                // Maybe it should be something like if the population can be sustained instead of carrying capacity
                float populationAttraction = Game.Planets[planetID].CarryingCapacity - Game.PlanetPopulationProgress[planetID];
                
                // TODO: add burst hints
                if (float.IsNaN(populationAttraction))
                {
                    Debug.Assert(false);
                    populationAttraction = 100;
                }
                PlanetAttraction[planetID] = Mathf.Clamp(populationAttraction, MinAttractionFromPopulation, MaxAttractionFromPopulation);

            }

            // .04% of population leave evry year
            const float NormalEmigrationRate = 0.0001f;
            // Migration
            for (int departureID = 0; departureID < Game.N; departureID++)
            {

                // Send people to the planets
                for (int destinationID = 0; destinationID < Game.N; destinationID++)
                {
                    // Cant migrate internally, or maybe they actually should
                    if (departureID == destinationID || destinationID == 0)
                        continue;
                    
                    // Migration is a rate relative to the normal attraction rate.
                    // So if it is below attraction then more people migrate.
                    // if it is below then less people will migrate.
                    // every point above attraction level will increase it by 120%
                    float destinationAttraction = PlanetAttraction[destinationID] - PlanetAttraction[departureID];
                    float emigrationRate = NormalEmigrationRate * Mathf.Pow(1.2f, destinationAttraction);
                    PlanetImmigration[departureID, destinationID] = emigrationRate;  
                }
            }
            
            // Send them with planes
            // If can move then do
            
            // Migration
            for (int departureID = 0; departureID < Game.N; departureID++)
            {
                // Send people to the planets
                for (int destinationID = 0; destinationID < Game.N; destinationID++)
                {
                    // Try and take a spacecraft
                    if (!Game.SpaceflightSystem.TakeSpacecraft(departureID, out Spacecraft spacecraft))
                        break;

                    // Only process places with full spaceship capacities
                    if (PlanetImmigration[departureID, destinationID] < spacecraft.MaxPopulation)
                    {
                        Game.SpaceflightSystem.GiveSpacecraft(departureID, spacecraft);
                        continue;
                    }
                    
                    PlanetImmigration[departureID, destinationID] -= spacecraft.MaxPopulation; 

                    // Load fly and return
                    spacecraft.Steps.Enqueue(new Spacecraft.Step(departureID, Spacecraft.StepType.Load));
                    spacecraft.Steps.Enqueue(new Spacecraft.Step(destinationID, Spacecraft.StepType.Unload));
                    //spacecraft.Steps.Enqueue(new Spacecraft.Step(departureID, Spacecraft.StepType.Unload));
                    
                    Game.SpaceflightSystem.GiveSpacecraft(departureID, spacecraft);

                }
            }

        }
        

    }

    
        /*
         *                     
                    // send to the planets, but max the current population
                    // Todo: prioritize sending population to planets that are the closest and have highest attraction
                    
                    // Can max send 20% of the planets population to the planet.
                    // Total capacity taking account other migration sent
                    long immigrationCapacityDestination = (long)(PlanetPopulations[destinationID] * .2f);
                    PlanetImmigration[departureID, destinationID] += Mathl.Min(wantsToEmigrate, immigrationCapacityDestination);
                    
                    // People who will be sent
                    long emigration = Mathl.Min(wantsToEmigrate, immigrationCapacityDestination);
                    
                    // Migrate
                    PlanetPopulations[departureID] -= emigration;
                    PlanetImmigration[departureID, destinationID] += wantsToEmigrate;
                    wantsToEmigrate -= emigration;
                    
                                for (int i = 0; i < N; i++)
            {
                PlanetPopulations[i] += PlanetImmigration[i];
            }   
         */
}