using System;
using System.Collections.Generic;
using Bserg.Controller.Interfaces;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;

namespace Bserg.Controller.Collections
{
    public struct VisualEntityPool<T> : IDisposable where T : unmanaged, IEntityVisual<T>
    {
        /// <summary>
        /// Guarantee that pool is sorted with enabled components first and disabled last
        /// </summary>
        private int activeElements;

        private readonly RenderMeshArray meshArray;
        public NativeList<T> List;

        public VisualEntityPool(EntityManager entityManager, RenderMeshArray meshArray)
        {
            activeElements = 0;
            this.meshArray = meshArray;
            List = new NativeList<T>(Allocator.Persistent);
            
            DoublePool(entityManager);
        }
        
        /// <summary>
        /// Populates the pool with new entities
        /// </summary>
        public void Populate(EntityManager entityManager, NativeArray<Entity> models)
        {
            int n = models.Length - activeElements;

            // Stabilize
            if (n > 0)
                Enable(entityManager, n);
            else if (n < 0)
                Disable(entityManager, -n);

            // Assign
            for (int i = 0; i < models.Length; i++)
                List[i].Assign(entityManager, models[i]);
        }


        /// <summary>
        /// Enables n elements
        /// </summary>
        public void Enable(EntityManager entityManager, int n)
        {
            while (n + activeElements > List.Length)
                DoublePool(entityManager);

            for (int i = 0; i < n; i++)
                List[activeElements++].Enable(entityManager);

        }
        /// <summary>
        /// Disables n elements
        /// </summary>
        public void Disable(EntityManager entityManager, int n)
        {
            for (int i = 0; i < n; i++)
                List[--activeElements].Disable(entityManager);
        }

        /// <summary>
        /// Disables all
        /// </summary>
        public void DisableAll(EntityManager entityManager) => Disable(entityManager, activeElements);

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
            if (activeElements == List.Length)
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
            int count = math.clamp(List.Length, 16, 256);

            // This is wierd, but it is because we cant define constructor signatures in interfaces
            T prototype = new T().CreatePrototype(entityManager, meshArray);    
            prototype.Disable(entityManager);

            // Add all the objects except for prototype
            for (int i = 0; i < count; i++)
            {
                T visualEntity = prototype.Clone(entityManager);
                List.Add(visualEntity);
            }

            prototype.Destroy(entityManager);
        }

        public void Dispose()
        {
            List.Dispose();
        }
    }
}