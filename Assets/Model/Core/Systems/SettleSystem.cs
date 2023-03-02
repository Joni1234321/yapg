using System.Collections.Generic;
using Bserg.Model.Space.SpaceMovement;
using Model.Population;

// Terraforming https://space.nss.org/space-settlement-roadmap-29-terraforming/
namespace Bserg.Model.Core.Systems
{
    /// <summary>
    ///     Handle settlement migration
    /// </summary>
    public class SettleSystem : GameSystem
    {
        // Settlement
        public List<SettleOrder> SettleOrders;

        public SettleSystem(Game game) : base(game)
        {
            SettleOrders = new List<SettleOrder>();
            SettleOrders.Add(new SettleOrder(3, 4, 500, 6));
            SettleOrders.Add(new SettleOrder(3, 2, 500, 6));
            SettleOrders.Add(new SettleOrder(3, 1, 500, 6));
        }

        public void System()
        {
            for (var i = SettleOrders.Count - 1; i >= 0; i--)
            {
                var order = SettleOrders[i];
                // If can move then do
                if (ValidDeparture(order.DepartureID, order.PopulationPerTrip))
                {
                    // No spacecraft available
                    if (!Game.SpaceflightSystem.TakeSpacecraft(order.DepartureID, out var spacecraft))
                        continue;

                    // Success 
                    SettleOrders.RemoveAt(i);

                    // Steps Load and unload
                    for (var j = 0; j < order.Trips; j++)
                    {
                        spacecraft.Steps.Enqueue(new Spacecraft.Step(order.DepartureID, Spacecraft.StepType.Load));
                        spacecraft.Steps.Enqueue(new Spacecraft.Step(order.DestinationID, Spacecraft.StepType.Unload));
                    }

                    Game.SpaceflightSystem.GiveSpacecraft(order.DepartureID, spacecraft);
                }
            }
        }


        /// <summary>
        ///     Returns if departure is valid
        ///     Only valid if population is more than a 1000 more than requested
        /// </summary>
        /// <param name="departureID"></param>
        /// <param name="populationRequest"></param>
        /// <returns></returns>
        public bool ValidDeparture(int departureID, long populationRequest)
        {
            return populationRequest + 1000 < Game.PlanetPopulationLevels[departureID];
        }

        /// <summary>
        ///     Returns if destination is valid
        ///     currently only the sun is not valid
        /// </summary>
        /// <param name="destinationID"></param>
        /// <returns></returns>
        public bool ValidDestination(int destinationID)
        {
            return destinationID != 0;
        }

        /// <summary>
        ///     Returns true if both departure and destination is valid
        /// </summary>
        /// <param name="departureID"></param>
        /// <param name="destinationID"></param>
        /// <param name="populationRequest"></param>
        /// <returns></returns>
        public bool ValidConnection(int departureID, int destinationID, long populationRequest)
        {
            return departureID != destinationID && ValidDeparture(departureID, populationRequest) &&
                   ValidDestination(destinationID);
        }
    }
}