namespace Model.Population
{
    public struct SettleOrder
    {
        public readonly int DepartureID, DestinationID;
        public readonly long PopulationPerTrip;
        public readonly int Trips;
        
        public SettleOrder(int departureID, int destinationID, long populationPerTrip, int trips)
        {
            DepartureID = departureID;
            DestinationID = destinationID;
            PopulationPerTrip = populationPerTrip;
            Trips = trips;
        }
    }
}