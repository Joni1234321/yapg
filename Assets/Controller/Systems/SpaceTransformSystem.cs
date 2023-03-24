using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace Bserg.Controller.Systems
{
    [UpdateInGroup(typeof(DrawSystemGroup))]
    internal partial struct SpaceTransformSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameTicksF>();
            state.RequireForUpdate<Global.CameraComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float ticksF = SystemAPI.GetSingleton<GameTicksF>().TicksF;
            float cameraSize = SystemAPI.GetSingleton<Global.CameraComponent>().Size; 
            
            state.Dependency = new UIWorldTransitionJob
            {
                CameraSize = cameraSize,
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new MoveOnCircleJob
            {
                GameTicksF = ticksF,
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new RotateJob
            {
                GameTicksF = ticksF,
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
        }

        /// <summary>
        /// Moves to point on circle with the MoveOnCircle Component
        /// </summary>
        [BurstCompile]
        internal partial struct MoveOnCircleJob : IJobEntity
        {
            [ReadOnly] public float GameTicksF;
            public void Execute(ref LocalTransform localTransform, in SpaceTransform.MoveOnCircle moveOnCircle)
            {
                float angle = moveOnCircle.PeriodTicksF == 0 ? 0 : 2 * math.PI * GameTicksF / moveOnCircle.PeriodTicksF;
                float offsetAngle = moveOnCircle.OffsetAngle;
                float a = moveOnCircle.Radius;
                float b = moveOnCircle.Radius;

                float x = a * math.cos(angle) * math.cos(offsetAngle) - b * math.sin(angle) * math.sin(offsetAngle);
                float y = a * math.cos(angle) * math.sin(offsetAngle) + b * math.sin(angle) * math.cos(offsetAngle);

                
                /*
                 UPDATE WITH OFFSET
                float cx = AUToWorld(offsetXAt0) * Mathf.Cos(offsetAngle);
                float cy = AUToWorld(offsetXAt0) * Mathf.Sin(offsetAngle);
                return new Vector3(x + cx, y + cy, 0);
                */
                localTransform.Position = new float3(x, y, 0);
            }
        }

        /// <summary>
        /// Rotates components with rotate 
        /// </summary>
        [BurstCompile]
        internal partial struct RotateJob : IJobEntity
        {
            [ReadOnly] public float GameTicksF;

            public void Execute(ref LocalTransform localTransform, in SpaceTransform.Rotate rotate)
            {
                float angle = rotate.PeriodTicksF == 0 ? 0 : 2 * math.PI * GameTicksF / rotate.PeriodTicksF;
                localTransform.Rotation = quaternion.RotateZ(.5f * math.PI + angle);
            }
        }
        
        /// <summary>
        /// Changes material and mesh from world view to ui view when camera is close enough
        /// </summary>
        [BurstCompile]
        internal partial struct UIWorldTransitionJob : IJobEntity
        {
            [ReadOnly] public float CameraSize;
            public void Execute(ref LocalTransform localTransform, ref MaterialMeshInfo meshInfo, in SpaceTransform.UIWorldTransition transition)
            {
                float uiSize = transition.UIScale * CameraSize * .03f;
                
                // Set mesh and material
                if (transition.WorldScale > uiSize)
                {
                    // World 
                    meshInfo =
                        MaterialMeshInfo.FromRenderMeshArrayIndices(
                            transition.WorldMaterialIndex,
                            transition.WorldMeshIndex);

                    localTransform.Scale = transition.WorldScale;
                }
                else
                {
                    meshInfo =
                        MaterialMeshInfo.FromRenderMeshArrayIndices(
                            transition.UIMaterialIndex, 
                            transition.UIMeshIndex);
                    
                    localTransform.Scale = uiSize;
                }
            }
        }
    }
}