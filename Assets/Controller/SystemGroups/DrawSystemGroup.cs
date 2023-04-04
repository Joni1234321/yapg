using Bserg.Controller.Systems;
using Bserg.Model.Shared.SystemGroups;
using Unity.Entities;
using Unity.Transforms;

namespace Bserg.Controller.SystemGroups
{
    [UpdateAfter(typeof(TickSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(TimeSystem))]
    public partial class DrawSystemGroup: ComponentSystemGroup
    {
    }
}