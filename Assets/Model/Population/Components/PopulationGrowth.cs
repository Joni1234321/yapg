using Unity.Entities;

namespace Bserg.Model.Population.Components
{
    public struct PopulationGrowth : IComponentData
    {
        public float BirthRate;
        public float DeathRate;
    }
}