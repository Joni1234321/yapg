using Unity.Entities;

namespace Bserg.Model.Shared.Components
{
    public struct GameTicks : IComponentData
    {
        public int Ticks;
    }
}