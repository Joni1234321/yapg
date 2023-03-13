using UnityEngine;

namespace Bserg.View.Space
{
    [CreateAssetMenu]
    [System.Serializable]
    public class SystemPrefab : ScriptableObject
    {
       public PlanetScriptable[] planetScriptables;
    }

    [System.Serializable]
    public struct SolarSystemData
    {
        //public PlanetData[] PlanetData;
        public PlanetScriptable[] planetScriptables;

        public SolarSystemData(PlanetScriptable[] planetScriptables)
        {
            this.planetScriptables = planetScriptables;

        }
    }
    
    
}
