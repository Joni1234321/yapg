using Bserg.Controller.Components;
using Bserg.Controller.Drivers;
using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using NUnit.Framework;
using Unity.Burst;
using Unity.Entities;

namespace Bserg.Controller.Systems
{
    [UpdateBefore(typeof(TickSystemGroup))]
    internal partial struct TimeSystem : ISystem
    {
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ShouldTick>();
            state.RequireForUpdate<GameTicks>();
            
            state.EntityManager.CreateSingleton<GameSpeed>();
            state.EntityManager.CreateSingleton<GameTicksF>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            GameSpeed gameSpeed = SystemAPI.GetSingleton<GameSpeed>();
            GameTicksF gameTicksF = SystemAPI.GetSingleton<GameTicksF>();
            
            int ticks = SystemAPI.GetSingleton<GameTicks>().Ticks;
            
            // dont go proceed the game
            if (!gameSpeed.Running)
                return;
            
            // proceed if overflow then add next tick
            float deltaTick = gameTicksF.DeltaTick + (SystemAPI.Time.DeltaTime / TimeDriver.TICK_TIME[gameSpeed.Speed]);
            if (deltaTick >= 1)
            {
                
                Assert.IsFalse(SystemAPI.GetSingleton<ShouldTick>().Value);
                
                SystemAPI.SetSingleton(new ShouldTick { Value = true });

                // Force to show start of tick always
                deltaTick = 0;
                // This value is predicted 
                ticks++;
            }
            
            // Only set gameTicksF
            SystemAPI.SetSingleton(new GameTicksF
            {
                TicksF = ticks + deltaTick,
                DeltaTick = deltaTick,
            });
        }
    }
}