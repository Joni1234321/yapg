using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.World;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Bserg.Controller.Systems
{
    [UpdateInGroup(typeof(DrawSystemGroup))] 
    internal partial struct PlanetControllerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameTicksF>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float ticksF = SystemAPI.GetSingleton<GameTicksF>().TicksF;

            state.Dependency = new PlanetRenderJob()
            {
                GameTicksF = ticksF,
            }.ScheduleParallel(state.Dependency);

            state.Dependency.Complete();
            
        }
    }

    [BurstCompile]
    internal partial struct PlanetRenderJob : IJobEntity
    {
        [ReadOnly] public float GameTicksF;
        public void Execute(ref LocalTransform localTransform, in OrbitRadius orbitRadius, in OrbitPeriod orbitPeriod)
        {
            localTransform.Position = 
                PlanetRenderer.GetLocalPlanetPositionAtTickF(orbitRadius.RadiusAU, orbitPeriod.TicksF, GameTicksF);

        } 
    }
}