using System;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine.Assertions;

namespace Bserg.Model.Space.Systems
{
    /// <summary>
    /// Process ships that has landed
    /// by taking off cargo, and fill it up with new cargo and new destination
    /// </summary>
    [BurstCompile]
    [WithAll(typeof(Spacecraft.ProcessingTag))]
    internal partial struct SpaceflightProcessJob : IJobEntity
    {
        public EntityCommandBuffer Ecb;
        [ReadOnly] public int GameTick;

        public ComponentLookup<PopulationProgress> PopulationProgresses;
        [ReadOnly] public ComponentLookup<PopulationLevel> PopulationLevels;
        public ComponentLookup<SpacecraftPool> Pools;

        private const int CARGO_MAX = 3;

        public void Execute(
            Entity e,
            ref Spacecraft.Cargo cargo,
            ref Spacecraft.FlightPlan flightPlan)
        {
            Entity currentPlanet = flightPlan.CurrentFlightStep.DestinationPlanet;
            PopulationLevel populationLevel = PopulationLevels[currentPlanet];
            PopulationProgress planetProgress = PopulationProgresses[currentPlanet];

            // Do its actions
            switch (flightPlan.CurrentFlightStep.Action)
            {
                case Spacecraft.ActionType.Unload:
                    planetProgress.Progress += Util.LevelProgress(populationLevel.Level, cargo.Population);
                    cargo.Population = 0;
                    break;
                case Spacecraft.ActionType.Load:
                    // Can max load down to 1000 people on planet TODO: UPDATE TO SOMETHING MORE SOPHISTICATED
                    planetProgress.Progress -= Util.LevelProgress(populationLevel.Level, CARGO_MAX);
                    cargo.Population = CARGO_MAX;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            PopulationProgresses[currentPlanet] = planetProgress;

            // If no more orders, then destroy and send to pool 
            if (flightPlan.ActionsLeft == 0)
            {
                // Automatically unload if cargo is not empty
                planetProgress.Progress += Util.LevelProgress(populationLevel.Level, cargo.Population);
                PopulationProgresses[currentPlanet] = planetProgress;
                
                // Destroy
                Ecb.DestroyEntity(e);
                SpacecraftPool pool = Pools[currentPlanet];
                pool.Available++;
                Pools[currentPlanet] = pool;
                return;
            }
            
            // Assign new orders
            flightPlan.ActionsLeft--;
            (flightPlan.CurrentFlightStep, flightPlan.NextFlightStep) = (flightPlan.NextFlightStep, flightPlan.CurrentFlightStep);

            // If already at planet, then process again next tick
            if (flightPlan.CurrentFlightStep.DestinationPlanet == currentPlanet)
            {
                return;
            }
            
            // Fly to space
            Ecb.RemoveComponent<Spacecraft.ProcessingTag>(e);
            int departureTick = GameTick + 50;
            Ecb.AddComponent(e, new Spacecraft.DepartureTick { Tick = departureTick });
            
            Assert.IsTrue(departureTick > GameTick);
        }
    }
}