using Unity.Entities;

namespace Bserg.Model.Shared.Components
{
    public struct LandLevel : IComponentData
    {
        public int Level;
    }
    
    public struct HousingLevel : IComponentData
    {
        public int Level;
    }

    public struct FoodLevel : IComponentData
    {
        public int Level;
    }
}