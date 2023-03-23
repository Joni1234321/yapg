using Unity.Entities;
using Unity.Rendering;

namespace Bserg.Controller.Components
{
    
    
    [MaterialProperty("_CircleScale")]
    public struct CircleScale : IComponentData
    {
        public float Value;
    }
}