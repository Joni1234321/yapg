using Bserg.Controller.Components;
using Unity.Burst;
using Unity.Entities;

namespace Bserg.Controller.Systems
{
    public partial struct TimeSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            //state.EntityManager.CreateSingleton<GameTicksF>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            
        }
    }
}