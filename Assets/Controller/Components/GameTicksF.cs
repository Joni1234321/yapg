using Unity.Entities;

namespace Bserg.Controller.Components
{
    public struct GameTicksF : IComponentData
    {
        /// <summary>
        /// tick + DeltaTick
        /// </summary>
        public float TicksF;
        /// <summary>
        /// how much of percentage of a tick has passed since last tick
        /// </summary>
        public float DeltaTick;
    }

    /// <summary>
    /// How fast the game is playing right now
    /// </summary>
    public struct GameSpeed : IComponentData
    {
        public bool Running;
        public int Speed;
    }
}