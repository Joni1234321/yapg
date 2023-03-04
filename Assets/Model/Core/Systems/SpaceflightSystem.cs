using System;
using System.Collections.Generic;
using Bserg.Model.Space.SpaceMovement;

namespace Bserg.Model.Core.Systems
{
    /// <summary>
    /// Handle transfers between planets, makes spacecraft take off and land, load and unload
    /// Spacecrafts need to be stored in either orbit transfer or idle
    /// </summary>
    public class SpaceflightSystem : GameSystem
    {
        public List<Spaceflight> PlannedSpaceflights, Spaceflights;
        public Stack<Spacecraft>[] SpacecraftPools, SpacecraftIdle; 


        public SpaceflightSystem(Game game) : base(game)
        {
            Spaceflights = new List<Spaceflight>(128);
            PlannedSpaceflights = new List<Spaceflight>(128);

            // Pools
            SpacecraftPools = new Stack<Spacecraft>[Game.N];
            for (int i = 0; i < Game.N; i++) SpacecraftPools[i] = new Stack<Spacecraft>();
            for (int i = 0; i < 4000; i++)
            {
                SpacecraftPools[3].Push(new Spacecraft(2000));
            }

            // Idle
            SpacecraftIdle = new Stack<Spacecraft>[Game.N];
            for (int i = 0; i < Game.N; i++) SpacecraftIdle[i] = new Stack<Spacecraft>();

        }


        /// <summary>
        /// Takes a free spacecraft from the pool (removing it) at planet ID
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="spacecraft"></param>
        /// <returns>if it grabbed it</returns>
        public bool TakeSpacecraft(int planetID, out Spacecraft spacecraft)
        {
            spacecraft = default;
            if (SpacecraftPools[planetID].Count == 0)
                return false;
            
            spacecraft = SpacecraftPools[planetID].Pop();
            return true;
        }


        /// <summary>
        /// Assign spacecraft to sit idle in space, waiting for a system to process their orders
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="spacecraft"></param>
        public void GiveSpacecraft(int planetID, Spacecraft spacecraft)
        {
            SpacecraftIdle[planetID].Push(spacecraft);
        }


        /// <summary>
        /// Handles current spacecraft step
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="spacecraft"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private Spacecraft ProcessCurrentStep(int planetID, Spacecraft spacecraft)
        {
            Spacecraft.Step currentStep = spacecraft.Steps.Dequeue();

            // Do current step for each spacecraft
            switch (currentStep.Type)
            {
                case Spacecraft.StepType.Unload:
                    Game.PlanetPopulationProgress[planetID] += spacecraft.MaxPopulation;
                    spacecraft.Population = 0;
                    break;
                case Spacecraft.StepType.Load:
                    long neededPopulation = spacecraft.MaxPopulation - spacecraft.Population;
                    // Can max load down to 1000 people on planet TODO: UPDATE TO SOMETHING MORE SOPHISTICATED
                    //long validPopulationOnPlanet = Mathl.Min(Game.PlanetPopulationLevels[planetID] - 9, 0);
                    //long populationLoaded = Mathl.Min(neededPopulation, validPopulationOnPlanet);
                                
                    // Load
                    Game.PlanetPopulationProgress[planetID] -= 0;
                    spacecraft.Population += 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return spacecraft;
        }
        
        public void System()
        {
            // REMOVE ALL IDLE SPACE CRAFT, PUT THEM IN POOL OR MAKE THEM DO STUFF
            IdleSpacecraftSystem();
            
            // Take off spacecraft
            for (int i = PlannedSpaceflights.Count - 1; i >= 0; i--)
            {
                Spaceflight incomingFlight = PlannedSpaceflights[i];
                if (incomingFlight.DepartureTick - Game.Ticks == 0)
                {
                    // Take-off
                    PlannedSpaceflights.RemoveAt(i);

                    // Fly
                    Spaceflights.Add(incomingFlight);
                }
            }

            // Land spacecraft
            for (int i = Spaceflights.Count - 1; i >= 0; i--)
            {
                Spaceflight flight = Spaceflights[i];

                // if flight has completed
                if (flight.DestinationTick - Game.Ticks == 0)
                {
                    Spaceflights.RemoveAt(i);

                    // Foreach spacecraft that has arrived
                    for (int j = 0; j < flight.Spacecrafts.Count; j++)
                    {
                        Spacecraft spacecraft = flight.Spacecrafts[j];
                        spacecraft = ProcessCurrentStep(flight.DestinationID, spacecraft);
                        GiveSpacecraft(flight.DestinationID, spacecraft);
                    }
                }
            }
        }
        
        /// <summary>
        /// Plan all spacecrafts next step
        /// </summary>
        private void IdleSpacecraftSystem()
        {
            for (int planetID = 0; planetID < SpacecraftIdle.Length; planetID++)
            {
                while (SpacecraftIdle[planetID].Count != 0)
                {
                    Spacecraft spacecraft = SpacecraftIdle[planetID].Pop();
                    
                    // Plan next step for spacecraft
                    if (spacecraft.Steps.Count == 0)
                    {
                        // Return Spacecraft to pool 
                        SpacecraftPools[planetID].Push(spacecraft);
                        continue;
                    }
                    
                    // Plan next flight
                    Spacecraft.Step nextStep = spacecraft.Steps.Peek();
                    PlanSpaceflight(planetID, nextStep.DestinationID, spacecraft);
                }
                
            }
            
        }
        
        /// <summary>
        /// Adds spacecraft to the planned flight, if non exists, then it schedules a Hohmann transfer
        /// todo update this so it doesnt need to refrence system
        /// </summary>
        private void PlanSpaceflight(int departureID, int destinationID, Spacecraft spacecraft)
        {
            // If planned route exists
            for (int i = 0; i < PlannedSpaceflights.Count; i++)
            {
                if (PlannedSpaceflights[i].DepartureID != departureID ||
                    PlannedSpaceflights[i].DestinationID != destinationID)
                    continue;

                PlannedSpaceflights[i].Spacecrafts.Add(spacecraft);
                return;
            }
            
            // Create new 
            HohmannTransfer<float> transfer = Game.OrbitalTransferSystem.HohmannTransfers[departureID, destinationID];
            float nextWindowTick = (int)Game.TickAtNextEventF(transfer.Window, transfer.Offset);
            Spaceflight flight = new Spaceflight((int)nextWindowTick,  (int)(nextWindowTick + transfer.Duration), departureID,
                destinationID);
            flight.Spacecrafts.Add(spacecraft);
            PlannedSpaceflights.Add(flight);
        }
    }
}