namespace Model.Population
{
    public struct SettleOrder
    {
        public readonly int DepartureID, DestinationID;
        public readonly int Trips;
        
        public SettleOrder(int departureID, int destinationID, int trips)
        {
            DepartureID = departureID;
            DestinationID = destinationID;
            Trips = trips;
        }
    }
}