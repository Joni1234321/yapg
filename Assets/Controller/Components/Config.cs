using Unity.Entities;

namespace Bserg.Controller.Components
{
    public struct Config : IComponentData
    {
        public Entity PlanetPrefab;
        public Entity PlanetUIPrefab;
        public Entity TextMeshProPrefab;
        public Entity TestLinkedPrefab;
    }
}