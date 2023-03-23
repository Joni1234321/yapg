using System.Collections.Generic;
using Bserg.Controller.Interfaces;
using Unity.Entities;
using Unity.Rendering;

namespace Bserg.Controller.Collections
{
    public class VisualEntityPool<T> where T : IEntityVisual<T>, IEntityAssignable, IEntityEnableable, new()
    {
        /// <summary>
        /// Guarantee that pool is sorted with enabled components first and disabled last
        /// </summary>
        private int activeElements;
        public readonly List<T> List;

        public VisualEntityPool(EntityManager entityManager, RenderMeshArray meshArray, int count = 16)
        {
            List = CreateBiggerPool(entityManager, meshArray, count);
            activeElements = 0;
        }
        
        /// <summary>
        /// Populates the pool with new entities
        /// </summary>
        public void Populate(EntityManager entityManager, RenderMeshArray meshArray, List<Entity> models)
        {
            // Double pool size
            while (models.Count > List.Capacity)
                List.AddRange(CreateBiggerPool(entityManager, meshArray, List.Capacity));

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
        /// Returns a list of new PlanetGameObjects added, all disabled
        /// </summary>
        /// <param name="entityManager"></param>
        /// <param name="meshArray"></param>
        /// <param name="count"></param>
        /// <returns>List of newly created empty gameobjects</returns>
        private List<T> CreateBiggerPool(EntityManager entityManager, RenderMeshArray meshArray, int count)
        {
            List<T> re = new (count);

            // Return if requesting a bigger pool of size 0
            if (count < 1)
                return re;

            // This is wierd, but it is because we cant define constructor signatures in interfaces
            T prototype = new T().CreatePrototype(entityManager, meshArray);    
            prototype.Disable(entityManager);

            // Add all the objects except for prototype
            for (int i = 1; i < count; i++)
            {
                T visualEntity = prototype.CloneEntity(entityManager);
                visualEntity.SetComponentData(entityManager);
                re.Add(visualEntity);
            }
            
            // Then do once for the prototype
            prototype.SetComponentData(entityManager);
            re.Add(prototype);

            return re;
        }


    }
}