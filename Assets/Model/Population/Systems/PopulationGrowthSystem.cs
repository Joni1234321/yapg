using Bserg.Model.Population.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Shared.SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Bserg.Model.Population.Systems
{
    /// <summary>
    /// Handle population growth and decline
    /// UPDATE TO TAKE CARE OF PEOPLE WHO CAN MAKE BIRTH AND PEOPLE WHO CAN DIE
    /// ALSO FACTOR IN WORK AND SUCH
    /// ALSO AVERAGE AGE
    /// </summary>
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

    
    /// <summary>
    /// Grows the populationlevel of the planet
    /// </summary>
    [WithAll(typeof(Planet.ActiveTag))]
    [BurstCompile]
    internal partial struct GrowthJob : IJobEntity
    {
        public void Execute(
            in PopulationGrowth growth,
            in HousingLevel housingLevel,
            in PopulationLevel populationLevel, 
            ref PopulationProgress populationProgress)
        {
            // Birth rate Affected down to 25% by not enough housing
            float modifier = 1f;
            int housingDiff = populationLevel.Level - housingLevel.Level;
            if (housingDiff > 0)
                modifier *= 1f / math.min(housingDiff, 4);

            // New population
            populationProgress.Progress += growth.BirthRate * modifier - growth.DeathRate;
        }
    }
    
    /// <summary>
    /// Upgrades all whose progress is higher than 1
    /// Degrades all whose progress is lower than 1
    /// Maybe do generic or some crap
    /// </summary>
    [WithAll(typeof(Planet.ActiveTag))]
    [BurstCompile]
    internal partial struct ProgressHelper : IJobEntity
    {
        // TODO: Auto Upgrade all, including population
        // TODO: Auto Downgrade all, including population
        public void Execute(ref PopulationProgress progress, 
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