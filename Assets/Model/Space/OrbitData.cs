using System.Collections.Generic;

namespace Bserg.Model.Space
{
    /// <summary>
    /// Contains information of what planet orbits what
    /// The sun will have its children be the planets
    /// The earth will have its children be the moon, and other satellites
    /// </summary>
    public readonly struct OrbitData
    {
        private static readonly OrbitData EMPTY = new OrbitData(-1);
        public readonly int PlanetID;
        public readonly List<OrbitData> Children;

        public OrbitData(int planetID)
        {
            PlanetID = planetID;
            Children = new List<OrbitData>();
        }

        /// <summary>
        /// Adds planetID to any child that has id equal to orbitID
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="orbitID"></param>
        /// <returns>true if have added succesfully</returns>
        public bool Add(int planetID, int orbitID)
        {
            if (PlanetID == orbitID)
            {
                Children.Add(new OrbitData(planetID));
                return true;
            }
            
            for (int i = 0; i < Children.Count; i++)
                if (Children[i].Add(planetID, orbitID))
                    return true;
            return false;
        }


        /// <summary>
        /// Gets planet with id
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="result"> the result</param>
        /// <returns>if it exists</returns>
        public bool Get(int planetID, out OrbitData result)
        {
            if (planetID == PlanetID)
            {
                result = this;
                return true;
            }
            
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Get(planetID, out result))
                    return true;
            }

            result = EMPTY;
            return false;
        }
    }
}