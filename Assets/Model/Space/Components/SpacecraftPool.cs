using Unity.Entities;

namespace Bserg.Model.Space.Components
{
    /// <summary>
    /// Contains the remaining amount of spacecrafts available right now
    /// Every spacecraft has the same size
    /// TODO: Make it os that it contains different size of spacecraft (stored in a fixed array, where each level contains the amount of the specific spacecraft)
    /// </summary>
    public struct SpacecraftPool : IComponentData
    {
        public int Available;
    }


    
}