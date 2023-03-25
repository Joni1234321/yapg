using Bserg.Model.Shared.Components;
using Bserg.Model.Units;
using NUnit.Framework;
using Unity.Burst;
using Unity.Entities;

namespace Bserg.Model.Shared.SystemGroups
{
    public partial class TickSystemGroup : ComponentSystemGroup
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            EntityManager.CreateSingleton<GameTicks>();
            EntityManager.CreateSingleton<ShouldTick>();
            EntityManager.CreateSingleton<GameShouldTick>();
            base.OnCreate();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            if (SystemAPI.GetSingleton<ShouldTick>().Value)
            {
                SystemAPI.GetSingletonRW<GameTicks>().ValueRW.Ticks++;
                SystemAPI.SetSingleton(new ShouldTick { Value = false });
                // TODO: REMOVE THIS
                SystemAPI.SetSingleton(new GameShouldTick { Value = true });
                base.OnUpdate();
            }
        }
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickMonthSystemGroup : ComponentSystemGroup
    {
        [BurstCompile]
        protected override void OnUpdate()
        {
            int ticks = SystemAPI.GetSingleton<GameTicks>().Ticks;   
            if (ticks % GameTick.TICKS_PER_MONTH == 0)
                base.OnUpdate();
        }
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickQuarterSystemGroup : ComponentSystemGroup
    {
        [BurstCompile]
        protected override void OnUpdate()
        {
            int ticks = SystemAPI.GetSingleton<GameTicks>().Ticks;   
            if (ticks % GameTick.TICKS_PER_QUARTER == 0)
                base.OnUpdate();
        }
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickYearSystemGroup : ComponentSystemGroup
    {
        [BurstCompile]
        protected override void OnUpdate()
        {
            int ticks = SystemAPI.GetSingleton<GameTicks>().Ticks;   
            if (ticks % GameTick.TICKS_PER_YEAR == 0)
                base.OnUpdate();
        }
    }

}