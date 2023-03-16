using Unity.Entities;

namespace Bserg.Model.Shared.Components
{
    public struct PopulationLevel : IComponentData
    {
        public int Level;
    }

    public struct PopulationProgress : IComponentData
    {
        public float Progress;
    }
}