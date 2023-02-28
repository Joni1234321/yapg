using System.Collections.Generic;

namespace Bserg.Model.Political
{
    /// <summary>
    /// Everything you own needs to be under a political body
    /// Political bodies are often trees, so that there is one body at the top. This top body can be either a single person or a group of people controlling everythihng
    /// Planets are bodies
    /// Regions on planets are bodies
    /// 
    /// 
    /// </summary>
    public class PoliticalBody
    {
        public string PoliticalSystem;
        public string PoliticalParty;

        public List<PoliticalBody> Children;

        public PoliticalBody(string politicalSystem, string politicalParty)
        {
            PoliticalSystem = politicalSystem;
            PoliticalParty = politicalParty;
        }
    }
    /// When colonizing planet, add ot the political party
    /// Add political parties, so that poly party system works
}