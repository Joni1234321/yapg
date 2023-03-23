using Unity.Entities;
using Unity.Rendering;

namespace Bserg.Controller.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntityVisual<T>
    {
        T CreatePrototype(EntityManager entityManager, RenderMeshArray meshArray);
        T CloneEntity(EntityManager entityManager);
        
        void SetComponentData(EntityManager entityManager);
    }
    
    
    /// <summary>
    /// Allows for the visuals to be turned on and off
    /// </summary>
    public interface IEntityEnableable
    {
        void Enable(EntityManager entityManager);
        void Disable(EntityManager entityManager);
    }
    
    
    /// <summary>
    /// Allows for its values to be assigned
    /// </summary>
    public interface IEntityAssignable
    {
        void Assign(EntityManager entityManager, Entity model);
    }
}