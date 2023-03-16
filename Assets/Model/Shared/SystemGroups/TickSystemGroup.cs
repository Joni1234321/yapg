using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Bserg.Model.Shared.SystemGroups
{
    public class TickSystemGroup : ComponentSystemGroup
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
    public class TickMonthSystemGroup : ComponentSystemGroup
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
    public class TickQuarterSystemGroup : ComponentSystemGroup
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
    public class TickYearSystemGroup : ComponentSystemGroup
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