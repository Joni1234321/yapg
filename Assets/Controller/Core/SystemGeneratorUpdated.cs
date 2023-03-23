using System.Linq;
using Bserg.Controller.Collections;
using Bserg.Controller.Components;
using Bserg.Controller.VisualEntities;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
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
        
            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);
            RenderMeshArray meshArray = new RenderMeshArray(new[] { planetMaterial, orbitMaterial, circleMaterial }, new[] { planetMesh, quad });


            // Get all planets without transforms and craete a gameobject for them
            EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<Planet.Tag>().WithNone<LocalTransform>()
                .Build(entityManager);

            planetPool = new (entityManager, meshArray, 2);

            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            planetPool.Populate(entityManager, meshArray, entities.ToList());
/*
            foreach (Entity planet in entities)
            {
                Entity parent = entityManager.GetComponentData<PlanetOrbit>(planet).OrbitEntity;
                Planet.Data planetData = entityManager.GetComponentData<Planet.Data>(planet);
                float radius = SystemGenerator.AUToWorld((float)planetData.OrbitRadius.To(Length.UnitType.AstronomicalUnits));

                
                CreatePlanetPosition(entityManager, planet);
                
                if (parent != Entity.Null)
                {
                    entityManager.AddComponentData(planet, new Parent { Value = parent });
                    entityManager.AddComponentData(planet, new SpaceTransform.MoveOnCircle
                    {
                        PeriodTicksF = entityManager.GetComponentData<OrbitPeriod>(planet).TicksF,
                        Radius = radius,
                        OffsetAngle = 0,
                    });
                    CreateOrbitEntity(entityManager, planet, parent, planetData, meshArray, desc);
                }
                
                CreateModelEntity(entityManager, planet, planetData, desc, meshArray);
            }*/
            
            query.Dispose();
            entities.Dispose();
        }

        
        
        
        Entity CreatePlanetPosition(EntityManager entityManager,  Entity planet)
        {
            entityManager.AddComponentData(planet, LocalTransform.FromScale(1f));
            entityManager.AddComponentData(planet, new LocalToWorld());
            return planet;
        }


        Entity CreateModelEntity(EntityManager entityManager, Entity planet, Planet.Data planetData,
            RenderMeshDescription desc, RenderMeshArray meshArray)
        {
            Entity model = entityManager.CreateEntity();

#if  UNITY_EDITOR
            entityManager.SetName(model, "Model " + entityManager.GetComponentData<Planet.Name>(planet).Text);
#endif
            RenderMeshUtility.AddComponents(model, entityManager, desc, meshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(2,1));
            entityManager.AddComponentData(model, LocalTransform.FromScale(SystemGenerator.GetRealPlanetSize(planetData.Size).x));
            entityManager.AddComponentData(model, new Parent { Value = planet });
            entityManager.AddComponentData(model, new URPMaterialPropertyEmissionColor { Value = planetData.Color });
            entityManager.AddComponentData(model, new URPMaterialPropertyBaseColor { Value = planetData.Color });
            entityManager.AddComponentData(model, new SpaceTransform.UIWorldTransition
            {
                WorldScale = SystemGenerator.GetRealPlanetSize(planetData.Size).x,
                UIScale = SystemGenerator.GetIconPlanetSize(planetData.Size).x,
                WorldMaterialIndex = 0,
                WorldMeshIndex = 0,
                UIMaterialIndex = 2,
                UIMeshIndex = 1,
            });
            
            return model;
        }
        Entity CreateOrbitEntity(EntityManager entityManager, Entity planet, Entity parent, Planet.Data planetData, RenderMeshArray meshArray, RenderMeshDescription desc)
        {
            Entity orbit = entityManager.CreateEntity();

#if  UNITY_EDITOR
            entityManager.SetName(orbit, "Orbit " + entityManager.GetComponentData<Planet.Name>(planet).Text);
#endif
            RenderMeshUtility.AddComponents(orbit, entityManager, desc, meshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(1,1));
            float radius = SystemGenerator.AUToWorld((float)planetData.OrbitRadius.To(Length.UnitType.AstronomicalUnits)) * 4;
            entityManager.AddComponentData(orbit, LocalTransform.FromScale(radius));
            entityManager.AddComponentData(orbit, new Parent { Value = parent });
            entityManager.AddComponentData(orbit, new SpaceTransform.Rotate
            {
                PeriodTicksF = entityManager.GetComponentData<OrbitPeriod>(planet).TicksF
            });

            return orbit;
        }

    }

}

