using Bserg.Model.Shared.Components;
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
            base.OnCreate();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            if (SystemAPI.GetSingleton<ShouldTick>().Value)
            {
                SystemAPI.GetSingletonRW<GameTicks>().ValueRW.Ticks++;
                base.OnUpdate();
                SystemAPI.SetSingleton(new ShouldTick { Value = false });
            }
        }
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickMonthSystemGroup : ComponentSystemGroup
    {
        private static int remainingTicks = 0;
        [BurstCompile]
        protected override void OnUpdate()
        {
            if (remainingTicks == 0)
                return;

            while (remainingTicks > 0)
            {
                base.OnUpdate();
                remainingTicks--;
            }
        }

        public static void Tick()
        {
            remainingTicks++;
        } 
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickQuarterSystemGroup : ComponentSystemGroup
    {
        private static int remainingTicks = 0;
        [BurstCompile]
        protected override void OnUpdate()
        {
            if (remainingTicks == 0)
                return;

            while (remainingTicks > 0)
            {
                base.OnUpdate();
                remainingTicks--;
            }
        }

        public static void Tick()
        {
            remainingTicks++;
        } 
    }
    
    [UpdateInGroup(typeof(TickSystemGroup))]
    public partial class TickYearSystemGroup : ComponentSystemGroup
    {
        private static int remainingTicks = 0;
        [BurstCompile]
        protected override void OnUpdate()
        {
            if (remainingTicks == 0)
                return;

            while (remainingTicks > 0)
            {
                base.OnUpdate();
                remainingTicks--;
            }
        }

        public static void Tick()
        {
            remainingTicks++;
        } 
    }

}