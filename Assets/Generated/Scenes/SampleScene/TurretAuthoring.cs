using Bserg.Model.Shared.Components;
using Unity.Entities;

namespace Bserg.Generated.Scenes.SampleScene
{
    class TurretAuthoring : UnityEngine.MonoBehaviour
    {
        // Bakers convert authoring MonoBehaviours into entities and components.
        // (Nesting a baker in its associated Authoring component is not necessary but is a common convention.)
        class TurretBaker : Baker<TurretAuthoring>
        {
            public override void Bake(TurretAuthoring authoring)
            {
                AddComponent<Planet.Tag>();
            }
        }
    }
}