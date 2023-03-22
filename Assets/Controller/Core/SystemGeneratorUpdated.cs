using System.Collections.Generic;
using Bserg.Controller.Components;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering;
using Entity = Unity.Entities.Entity;

namespace Bserg.Controller.Core
{
    public class SystemGeneratorUpdated
    {

        public SystemGeneratorUpdated (EntityManager entityManager, Material planetMaterial, Mesh planetMesh, Material orbitMaterial, Mesh orbitMesh) 
        {
            GameObject.Find("System").SetActive(false);
            GameObject.Find("Orbits").SetActive(false);
        
            RenderMeshArray planetMeshArray = new RenderMeshArray(new[] { planetMaterial }, new[] { planetMesh });
            RenderMeshArray orbitMeshArray = new RenderMeshArray(new[] { orbitMaterial }, new[] { orbitMesh });
            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);


            EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<Planet.Tag>().WithNone<LocalTransform>()
                .Build(entityManager);

            

            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            foreach (Entity planet in entities)
            {
                Entity parent = entityManager.GetComponentData<PlanetOrbit>(planet).OrbitEntity;
                Planet.Data planetData = entityManager.GetComponentData<Planet.Data>(planet);
                RenderMeshUtility.AddComponents(planet, entityManager, desc, planetMeshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
                entityManager.AddComponent<DisableRendering>(planet);
                entityManager.AddComponentData(planet, LocalTransform.FromScale(1f));
                if (parent != Entity.Null)
                {
                    entityManager.AddComponentData(planet, new Parent { Value = parent });
                    CreateOrbit(entityManager, planet, parent, planetData, orbitMeshArray, desc);
                }

                CreateModel(entityManager, planet, planetData, planetMeshArray, desc);
            }
            
            query.Dispose();
            entities.Dispose();
        }



        Entity CreateModel(EntityManager entityManager, Entity planet, Planet.Data planetData, RenderMeshArray meshArray, RenderMeshDescription desc)
        {
            Entity model = entityManager.CreateEntity();

#if  UNITY_EDITOR
            entityManager.SetName(model, "Model " + entityManager.GetComponentData<Planet.Name>(planet).Text);
#endif
            RenderMeshUtility.AddComponents(model, entityManager, desc, meshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
            entityManager.AddComponentData(model, LocalTransform.FromScale(SystemGenerator.GetRealPlanetSize(planetData.Size).x));
            entityManager.AddComponentData(model, new Parent { Value = planet });
            entityManager.AddComponentData(model, new URPMaterialPropertyEmissionColor { Value = planetData.Color });

            return model;
        }
        
        Entity CreateOrbit(EntityManager entityManager, Entity planet, Entity parent, Planet.Data planetData, RenderMeshArray meshArray, RenderMeshDescription desc)
        {
            Entity orbit = entityManager.CreateEntity();

#if  UNITY_EDITOR
            entityManager.SetName(orbit, "Orbit " + entityManager.GetComponentData<Planet.Name>(planet).Text);
#endif
            RenderMeshUtility.AddComponents(orbit, entityManager, desc, meshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
            float scale = SystemGenerator.AUToWorld((float)planetData.OrbitRadius.To(Length.UnitType.AstronomicalUnits)) * 4;
            entityManager.AddComponentData(orbit, LocalTransform.FromScale(scale));
            entityManager.AddComponentData(orbit, new Parent { Value = parent });
            entityManager.AddComponent<OrbitRotateTag>(orbit);
            entityManager.AddComponentData(orbit, entityManager.GetComponentData<OrbitPeriod>(planet));

            return orbit;
        }
        
        public List<Entity> Entities;
        
        public void CreatePool(int count, EntityManager entityManager, Material material, Mesh mesh)
        {
            Assert.IsTrue(Entities == null);
            Entities = new List<Entity>(count);
            
            RenderMeshArray renderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });

            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);

            for (int i = 0; i < count; i++)
            {
                Entity e = entityManager.CreateEntity();
                Entities[i] = e;
                float4 color = entityManager.GetComponentData<Planet.Data>(e).Color;
                RenderMeshUtility.AddComponents(e, entityManager, desc, renderMeshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
                entityManager.AddComponentData(e, new LocalTransform { Position = new float3(1, 0, 0) });
                Entity parent = entityManager.GetComponentData<PlanetOrbit>(e).OrbitEntity;
                if (parent != Entity.Null) entityManager.AddComponentData(e, new Parent { Value = parent });
                entityManager.AddComponentData(e, new URPMaterialPropertyEmissionColor { Value = color });
            }
        }


    }

}