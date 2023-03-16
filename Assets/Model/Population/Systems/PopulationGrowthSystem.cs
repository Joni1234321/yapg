using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Bserg.Model.Population.Systems
{
    [UpdateInGroup(typeof(TickYearSystemGroup))]
    internal partial struct PopulationGrowthSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var growthDependency = new GrowthJob().ScheduleParallel(state.Dependency);
            var progressDependency = new ProgressHelper().ScheduleParallel(growthDependency);
            state.Dependency = progressDependency;
        }
    }

    
    [BurstCompile]
    internal partial struct GrowthJob : IJobEntity
    {
        public void Execute(in PlanetActiveTag _,
            in PopulationGrowth growth,
            ref PopulationProgress populationProgress)
        {
            // New population
            populationProgress.Progress += growth.BirthRate - growth.DeathRate;
        }
    }
    
    /// <summary>
    /// Upgrades all whose progress is higher than 1
    /// Degrades all whose progress is lower than 1
    /// Maybe do generic or some crap
    /// </summary>
    [BurstCompile]
    internal partial struct ProgressHelper : IJobEntity
    {
        // TODO: Auto Upgrade all, including population
        // TODO: Auto Downgrade all, including population
        public void Execute(in PlanetActiveTag _, 
            ref PopulationProgress progress, 
            ref PopulationLevel level)
        {
            while (progress.Progress >= 1)
            {
                progress.Progress = (progress.Progress - 1) * .5f;
                level.Level++;
            }

            while (progress.Progress < 0)
            {
                progress.Progress = progress.Progress * 2f + 1;
                level.Level--;
            }
        }
        
    }

}