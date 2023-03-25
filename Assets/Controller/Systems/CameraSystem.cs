using Bserg.Controller.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Bserg.Controller.Systems
{
    partial struct CameraSystem : ISystem
    {
        private EntityQuery managedCameraQuery;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<Global.CameraManagedComponent>();
            managedCameraQuery =
                new EntityQueryBuilder(Allocator.Temp).WithAll<Global.CameraManagedComponent>().Build(state.EntityManager);
            
            state.EntityManager.CreateSingleton(new Global.CameraManagedComponent { Camera = Camera.main } );
            state.EntityManager.CreateSingleton<Global.CameraSizeComponent>();
        }
        
        public void OnUpdate(ref SystemState state)
        {
            SystemAPI.SetSingleton(new Global.CameraSizeComponent
            {
                Size = managedCameraQuery.GetSingleton<Global.CameraManagedComponent>().Camera.orthographicSize
            });
        }

    }
}