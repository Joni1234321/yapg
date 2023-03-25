using Unity.Entities;
using UnityEngine;

namespace Bserg.Controller.Components
{
    public struct Global
    {
        public class CameraManagedComponent : IComponentData
        {
            public Camera Camera;
        }
        public struct CameraSizeComponent : IComponentData
        {
            public float Size;
        }
        
    }
}