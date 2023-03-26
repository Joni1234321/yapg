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

        // TODO: MAKE INTO BLOB
        public struct CameraOptions : IComponentData
        {
            public float ClosestZoom;
            public float FarthestZoom;
        }

        public struct CameraAnimation : IComponentData
        {
            public Entity FollowEntity;
            public float TargetSize;
        }
        
    }
}