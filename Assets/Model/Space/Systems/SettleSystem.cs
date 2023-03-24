using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Bserg.Model.Space.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Bserg.Model.Space.Systems
{
    /// <summary>
    /// Goes through all settle orders and will try and sent spaceship
    /// </summary>
    [UpdateInGroup(typeof(TickSystemGroup))]
    internal partial struct SettleSystem : ISystem
    {

        private BufferLookup<Settle.Order> orderLookup;
#if UNITY_EDITOR
        private ComponentLookup<Planet.Name> planetNameLookup;
#endif
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            orderLookup = state.GetBufferLookup<Settle.Order>();

#if UNITY_EDITOR
            planetNameLookup = state.GetComponentLookup<Planet.Name>();
#endif
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            orderLookup.Update(ref state);
#if UNITY_EDITOR
            planetNameLookup.Update(ref state);
#endif
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();

            EntityCommandBuffer ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            state.Dependency = new SettleJob
            {
                ParallelWriter = ecb.AsParallelWriter(),
                SettleOrderLookup = orderLookup,
#if UNITY_EDITOR
                PlanetNameLookup = planetNameLookup,
#endif

            }.ScheduleParallel(state.Dependency);
        }
    }


    [BurstCompile]
    internal partial struct SettleJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ParallelWriter;
        // Disable since we are restricting dynamic buffer to only edit a single entity per job
        [NativeDisableParallelForRestriction] public BufferLookup<Settle.Order> SettleOrderLookup;
#if UNITY_EDITOR
        [ReadOnly] public ComponentLookup<Planet.Name> PlanetNameLookup;
#endif

        public void Execute(Entity planetEntity, [ChunkIndexInQuery] int index, ref SpacecraftPool spacecraftPool)
        {
            int n = SettleOrderLookup[planetEntity].Length;
            for (int i = n - 1; i >= 0; i--)
            {
                // No more ships left in pool
                if (spacecraftPool.Available == 0)
                    return;

                Settle.Order order = SettleOrderLookup[planetEntity][i];
                int shipCount = order.Parallel;
                
                // Order is too expensive
                if (spacecraftPool.Available < shipCount)
                    continue;

                // Success
                SettleOrderLookup[planetEntity].RemoveAtSwapBack(i);

                for (int j = 0; j < shipCount; j++)
                {
                    Entity ship = ParallelWriter.CreateEntity(index);
                    ParallelWriter.AddComponent(index, ship, new Spacecraft.Tag());
                    ParallelWriter.AddComponent(index, ship, new Spacecraft.ProcessingTag());
                    ParallelWriter.AddComponent(index, ship, new Spacecraft.Cargo { Population = 0 });
                    ParallelWriter.AddComponent(index, ship, new Spacecraft.FlightPlan
                    {
                        CurrentFlightStep = new Spacecraft.FlightStep { DestinationPlanet = planetEntity, Action = Spacecraft.ActionType.Load},
                        NextFlightStep = new Spacecraft.FlightStep { DestinationPlanet = order.Destination, Action = Spacecraft.ActionType.Unload},
                        ActionsLeft = 10,
                    });

#if UNITY_EDITOR
                    ParallelWriter.SetName(index, ship,
                        $"Spaceship {PlanetNameLookup[planetEntity].Text} - {PlanetNameLookup[order.Destination].Text}");
#endif 
                }
            }
        }
    }
}