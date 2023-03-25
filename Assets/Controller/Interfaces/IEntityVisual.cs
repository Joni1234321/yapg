using Unity.Entities;
using Unity.Rendering;

namespace Bserg.Controller.Interfaces
{
    /// <summary>
    /// Used for visual entities
    /// </summary>
    public interface IEntityVisual<T> : IEntityAssignable, IEntityCloneable<T>, IEntityPrototypeable<T>, IEntityEnableable, IEntityDestroyable
    {
    }

    /// <summary>
    /// Allows for the entities to be cloned
    /// </summary>
    public interface IEntityCloneable<T>
    {
        /// <summary>
        /// Clones the current objects fields, by the use of instantiate [FAST]
        /// REMEMBER TO SET THE HIERARCHY AGAIN
        /// </summary>
        T Clone(EntityManager entityManager);
    }
    
    /// <summary>
    /// Allows for the visuals to be turned on and off
    /// </summary>
    public interface IEntityEnableable
    {
        /// <summary>
        /// Enables the visual entities
        /// Recommended use of RemoveComponent DisableRendering on every field
        /// </summary>
        void Enable(EntityManager entityManager);
        /// <summary>
        /// Disables the visual entities 
        /// Recommended use of AddComponent DisableRendering on every field
        /// </summary>
        void Disable(EntityManager entityManager);
    }

    /// <summary>
    /// Allows for its entities to be destroyed
    /// </summary>
    public interface IEntityDestroyable
    {
        /// <summary>
        /// Destroys entities
        /// </summary>
        public void Destroy(EntityManager entityManager);
    }


    public interface IEntityPrototypeable<T>
    {
        /// <summary>
        /// Adds components one by one to the objects fields [SLOW]
        /// Recommended only use AddComponent
        /// </summary>
        T CreatePrototype(EntityManager entityManager, RenderMeshArray meshArray);

    }
    
    /// <summary>
    /// Allows for its values to be assigned
    /// </summary>
    public interface IEntityAssignable
    {
        /// <summary>
        /// Assigns values from the model to the struct's fields
        /// </summary>
        void Assign(EntityManager entityManager, Entity model);
    }
}