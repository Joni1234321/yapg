using System.Collections.Generic;
using System.Linq;
using Bserg.Controller.Collections;
using Bserg.Controller.Components;
using Bserg.Controller.VisualEntities;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Entity = Unity.Entities.Entity;

namespace Bserg.Controller.Core
{
    public class SystemGeneratorUpdated
    {
        private VisualEntityPool<PlanetVisual> planetPool;

        public SystemGeneratorUpdated (EntityManager entityManager, Material planetMaterial, Mesh planetMesh, Material orbitMaterial, Material circleMaterial, Mesh quad) 
        {
            GameObject.Find("System").SetActive(false);
            GameObject.Find("Orbits").SetActive(false);
        
            RenderMeshArray meshArray = new RenderMeshArray(new[] { planetMaterial, orbitMaterial, circleMaterial }, new[] { planetMesh, quad });

            // Get all planets without transforms and create a gameobject for them
            EntityQuery modelQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Planet.Tag, OrbitPlanet>().WithNone<LocalTransform>()
                .Build(entityManager);

            planetPool = new (entityManager, meshArray);

            NativeArray<Entity> models = modelQuery.ToEntityArray(Allocator.Temp);
            NativeArray<OrbitPlanet> orbits = modelQuery.ToComponentDataArray<OrbitPlanet>(Allocator.Temp);
            planetPool.Populate(entityManager, models.ToList());
            
            // Add parents afterwards
            for (int i = 0; i < models.Length; i++)
            {
                int parentIndex = FindParentIndex(orbits[i].OrbitEntity, models);
                if (parentIndex == -1) 
                    continue; // none found
                
                Entity parentVisual = planetPool.List[parentIndex].Main;
                entityManager.AddComponentData(planetPool.List[i].Main, new Parent { Value = parentVisual });
                entityManager.AddComponentData(planetPool.List[i].Orbit, new Parent { Value = parentVisual });
            }
            
            planetPool.Remove(entityManager, 1);
            planetPool.Populate(entityManager, models.ToList());

            modelQuery.Dispose();
            models.Dispose();
            orbits.Dispose();
        }


        int FindParentIndex(Entity targetModel, NativeArray<Entity> models)
        {
            for (int i = 0; i < models.Length; i++)
                if (models[i] == targetModel)
                    return i;

            return -1;
        }
    }

}

