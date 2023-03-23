using Unity.Entities;

namespace Bserg.Controller.Components
{
    public struct Global
    {
        public struct CameraComponent : IComponentData
        {
            public float Size;
        }
        
    }
}