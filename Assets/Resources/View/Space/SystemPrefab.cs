using UnityEngine;
using UnityEngine.Serialization;

namespace Bserg.View.Space
{
    [CreateAssetMenu]
    public class SystemPrefab : ScriptableObject
    {
        [FormerlySerializedAs("Planets")] public PlanetScriptable[] planetScriptables;
    }
}
