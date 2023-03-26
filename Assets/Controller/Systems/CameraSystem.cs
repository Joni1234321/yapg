using Bserg.Controller.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Bserg.Controller.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    partial struct CameraSystem : ISystem
    {
        private EntityQuery managedCameraQuery;
        
        public void OnCreate(ref SystemState state)
        {
            managedCameraQuery =
                new EntityQueryBuilder(Allocator.Temp).WithAll<Global.CameraManagedComponent>().Build(state.EntityManager);

            state.EntityManager.CreateSingleton(new Global.CameraManagedComponent { Camera = Camera.main } );
            state.EntityManager.CreateSingleton<Global.CameraSizeComponent>();
            state.EntityManager.CreateSingleton<Global.CameraOptions>();
            state.EntityManager.CreateSingleton<Global.CameraAnimation>();

            SystemAPI.SetSingleton(new Global.CameraOptions
            {
                ClosestZoom = 0.0002f,
                FarthestZoom = 400f,
            });
            SystemAPI.SetSingleton(new Global.CameraAnimation { TargetSize = 10f });

        }
        
        public void OnUpdate(ref SystemState state)
        {
            Camera camera = managedCameraQuery.GetSingleton<Global.CameraManagedComponent>().Camera;
            Global.CameraOptions options = SystemAPI.GetSingleton<Global.CameraOptions>();
            ref Global.CameraAnimation animation = ref SystemAPI.GetSingletonRW<Global.CameraAnimation>().ValueRW;
            
                
            // Clamp Components
            animation.TargetSize = math.clamp(animation.TargetSize, options.ClosestZoom, options.FarthestZoom);

            if (SystemAPI.Exists(animation.FollowEntity))
                FollowEntity(camera, SystemAPI.GetComponent<LocalToWorld>(animation.FollowEntity).Value.Translation());

            UpdateZoom(camera, animation, options, SystemAPI.Time.DeltaTime);
            
            SystemAPI.SetSingleton(new Global.CameraSizeComponent
            {
                Size = camera.orthographicSize
            });
        }

        /// <summary>
        /// Smoothly goes to target zoom
        /// </summary>
        void UpdateZoom(Camera camera, Global.CameraAnimation animation, Global.CameraOptions options, float deltaTime)
        {
            float nextSize = NextSmoothStep(camera.orthographicSize, animation.TargetSize, deltaTime);
            // Clamp Camera value 
            camera.orthographicSize = math.clamp(nextSize, options.ClosestZoom, options.FarthestZoom);
        }

        /// <summary>
        /// Goes directly to position of entity
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="position"></param>
        void FollowEntity(Camera camera, float3 position)
        {
            camera.transform.position = new float3(position.x, position.y, -100);
        }

        /// <summary>
        /// Returns the next value
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private float NextSmoothStep(float current, float target, float deltaTime)
        {
            const float SPEED = 15, MAX_DIFF = 1f;
            float percent = current / target;
            float deltaPercent = 1 - percent; // + - percent

            if (math.abs(deltaPercent) < 0.01f) // dont zoom if as close as 1 %
                return target;
            float smooth = SPEED * deltaTime * math.clamp(deltaPercent, -MAX_DIFF, MAX_DIFF);
            return current * (1 + smooth);
        }
    }
}