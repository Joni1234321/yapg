using Unity.Entities;

namespace Bserg.Controller.Components
{
    /// <summary>
    /// The model that the visual entity is based of
    /// </summary>
    public struct EntityModel : IComponentData
    {
        public Entity Value;
    }
}