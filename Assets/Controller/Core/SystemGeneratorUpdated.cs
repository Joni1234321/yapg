using System.Linq;
using Bserg.Controller.Collections;
using Bserg.Controller.VisualEntities;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Entity = Unity.Entities.Entity;

namespace Bserg.Controller.Core
{
    public class SystemGeneratorUpdated
    {
        public VisualEntityPool<PlanetVisual> PlanetPool;
        public VisualEntityPool<TravelingSpacecraftVisual> ShipPool;

        private EntityQuery planetQuery, shipQuery;
        public SystemGeneratorUpdated (EntityManager entityManager, Material planetMaterial, Mesh planetMesh, Material orbitMaterial, Material circleMaterial, Mesh quad, Material spacecraftOrbitMaterial) 
        {
            GameObject.Find("System").SetActive(false);
            GameObject.Find("Orbits").SetActive(false);
        
            RenderMeshArray meshArray = new RenderMeshArray(new[] { planetMaterial, orbitMaterial, circleMaterial, spacecraftOrbitMaterial }, new[] { planetMesh, quad });

            // Get all planets without transforms and create a Visual for them
            // Remember that EntityQueryBuilder disposes itself, and can always be ALLOCATE.TEMP
            planetQuery = new EntityQueryBuilder(Allocator.Temp).
                WithAll<Planet.Tag, OrbitPlanet>().WithNone<LocalTransform>()
                .Build(entityManager);
            shipQuery = new EntityQueryBuilder(Allocator.Temp).
                WithAll<Spacecraft.Tag, Spacecraft.TravelingTag, Spacecraft.FlightPlan>().WithNone<LocalTransform>()
                .Build(entityManager);


            PlanetPool = new (entityManager, meshArray);
            ShipPool = new (entityManager, meshArray);
            
            NativeArray<Entity> models = planetQuery.ToEntityArray(Allocator.Temp);
            NativeArray<OrbitPlanet> orbits = planetQuery.ToComponentDataArray<OrbitPlanet>(Allocator.Temp);
            PlanetPool.Populate(entityManager, models);
            
            // Add parents afterwards
            for (int i = 0; i < models.Length; i++)
            {
                int parentIndex = FindParentIndex(orbits[i].OrbitEntity, models);
                if (parentIndex == -1) 
                    continue; // none found
                
                Entity parentVisual = PlanetPool.List[parentIndex].Main;
                entityManager.AddComponentData(PlanetPool.List[i].Main, new Parent { Value = parentVisual });
                entityManager.AddComponentData(PlanetPool.List[i].Orbit, new Parent { Value = parentVisual });
            }
            
            models.Dispose();
            orbits.Dispose();
        }

        public void UpdateShips(EntityManager entityManager)
        {
            NativeArray<Entity> models = shipQuery.ToEntityArray(Allocator.Temp);
            ShipPool.Populate(entityManager, models);
            models.Dispose();
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

