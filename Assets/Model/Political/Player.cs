namespace Bserg.Model.Political
{
    /// <summary>
    /// Political class for each player
    /// Politics are internal and decision making
    /// It is laws, constitutions, systems of governance
    ///
    /// They have their own rituals (kingdoms) and companions (servants and knights)
    /// They have differetn traditions and such (Could be cool if we could randomly assign them to each body (no impact on game, just for aesthetics))
    /// </summary>
    public class Player
    {
        public string PlayerName;

        public PoliticalBody PoliticalBody;
        
        public Player(string playerName, string politicalSystem, string politicalParty)
        {
            PlayerName = playerName;
            
            PoliticalBody = new PoliticalBody(politicalSystem, politicalParty);
        }
    }
    
    
    
}