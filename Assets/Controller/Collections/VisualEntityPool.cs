using System.Collections.Generic;
using Bserg.Controller.Interfaces;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Bserg.Controller.Collections
{
    public class VisualEntityPool<T> where T : IEntityVisual<T>, IEntityAssignable, IEntityEnableable, new()
    {
        /// <summary>
        /// Guarantee that pool is sorted with enabled components first and disabled last
        /// </summary>
        private int activeElements;

        private readonly RenderMeshArray meshArray;
        public readonly List<T> List;

        public VisualEntityPool(EntityManager entityManager, RenderMeshArray meshArray)
        {
            this.meshArray = meshArray;
            List = new List<T>();
            activeElements = 0;
        }
        
        /// <summary>
        /// Populates the pool with new entities
        /// </summary>
        public void Populate(EntityManager entityManager, List<Entity> models)
        {
            // Double pool size
            while (models.Count > List.Count)
                DoublePool(entityManager);
                    
            // Make active elements fit
            while (activeElements < models.Count)
                List[activeElements++].Enable(entityManager);
            
            while (activeElements > models.Count)
                List[activeElements--].Disable(entityManager);

            // Set elements
            for (int i = 0; i < models.Count; i++)
                List[i].Assign(entityManager, models[i]);

        }

        /// <summary>
        /// Disable element at index, and swap with last element
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="index"></param>
        public void Remove(EntityManager entityManager, int index)
        {
            Assert.IsTrue(index < activeElements);
            
            List[index].Disable(entityManager);
            (List[activeElements - 1], List[index]) = (List[index], List[activeElements - 1]);

            activeElements--;
        }

        /// <summary>
        /// Adds a single new entity to the pool
        /// doubles pool if cap has reached
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="model"></param>
        public void Add(EntityManager entityManager, Entity model)
        {
            if (activeElements == List.Count)
                DoublePool(entityManager);
            
            List[activeElements].Enable(entityManager);
            List[activeElements].Assign(entityManager, model);
            
            activeElements++;
        }

        /// <summary>
        /// Double the pool size, and by creating empty elements that are all disabled
        /// </summary>
        /// <param name="entityManager"></param>
        /// <returns>List of newly created empty gameobjects</returns>
        private void DoublePool(EntityManager entityManager)
        {
            int count = math.clamp(List.Count, 16, 256);

            // This is wierd, but it is because we cant define constructor signatures in interfaces
            T prototype = new T().CreatePrototype(entityManager, meshArray);    
            prototype.Disable(entityManager);

            // Add all the objects except for prototype
            for (int i = 1; i < count; i++)
            {
                T visualEntity = prototype.CloneEntity(entityManager);
                visualEntity.SetComponentData(entityManager);
                List.Add(visualEntity);
            }
            
            // Then do once for the prototype
            prototype.SetComponentData(entityManager);
            List.Add(prototype);
        }
    }
}