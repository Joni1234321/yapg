using Unity.Burst;
using Unity.Entities;

namespace Bserg.Model.Shared.SystemGroups
{
    public partial class TickSystemGroup : ComponentSystemGroup
    {
        [BurstCompile]
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        private static bool doTick;
        [BurstCompile]
        protected override void OnUpdate()
        {
            if (doTick)
            {
                base.OnUpdate();
                doTick = false;
            }
        }


        public static bool CanTick() => !doTick;
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Successful tick queue</returns>
        public static bool TryTick()
        {
            if (doTick)
                return false;
            
            doTick = true;
            return true;
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