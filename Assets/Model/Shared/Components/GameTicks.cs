using Unity.Entities;

namespace Bserg.Model.Shared.Components
{
    public struct GameTicks : IComponentData
    {
        public int Ticks;
    }

    /// <summary>
    /// Used to check if TickSystemGroup should tick this frame
    /// </summary>
    public struct ShouldTick : IComponentData
    {
        public bool Value;
    }

    // TODO: REMOVE THE NEED FOR GAME TO DO A TICK AS WELL AS THIS TIMEGROUP
    /// <summary>
    /// Used to check if Game should tick this frame
    /// </summary>
    public struct GameShouldTick : IComponentData
    {
        public bool Value;
    }

    /// <summary>
    /// Used to indicate if ticksystem made a tick this frame (and if ui needs to update
    /// </summary>
    public struct DidTickThisFrame : IComponentData
    {
        public bool Value;
    }
}