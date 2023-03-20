using Unity.Entities;

namespace Bserg.Model.Space.Components
{
    public struct Settle
    {
        [InternalBufferCapacity(20)]
        public struct Order : IBufferElementData
        {
            public Entity Destination;
            public int Parallel;
            public int Serial;
        }
    }
}