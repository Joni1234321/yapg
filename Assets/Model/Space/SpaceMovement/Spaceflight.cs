using System.Collections.Generic;

namespace Bserg.Model.Space.SpaceMovement
{
    /// <summary>
    /// Contains information about the spaceflight from planet a to planet b
    /// </summary>
    public struct Spaceflight
    {
        public readonly int DepartureTick, DestinationTick;
        public readonly int DepartureID, DestinationID;
        public List<Spacecraft> Spacecrafts;


        public Spaceflight(int departureTick, int destinationTick, int departureID, int destinationID)
        {
            DepartureTick = departureTick;
            DestinationTick = destinationTick;
            DepartureID = departureID;
            DestinationID = destinationID;
            Spacecrafts = new List<Spacecraft>();
        }
    }
}