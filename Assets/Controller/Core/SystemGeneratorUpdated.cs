using System.Collections.Generic;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
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

        public SystemGeneratorUpdated (EntityManager entityManager, Material material, Mesh mesh) 
        {
        
            RenderMeshArray renderMeshArray = new RenderMeshArray(new[] { material }, new[] { mesh });
            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);


            EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<Planet.Tag>().WithNone<LocalTransform>()
                .Build(entityManager);

            

            NativeArray<Entity> entities = query.ToEntityArray(Allocator.Temp);
            foreach (Entity planet in entities)
            {
                Entity parent = entityManager.GetComponentData<PlanetOrbit>(planet).OrbitEntity;
                float4 color = entityManager.GetComponentData<Planet.Data>(planet).Color;
                RenderMeshUtility.AddComponents(planet, entityManager, desc, renderMeshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
                entityManager.AddComponent<DisableRendering>(planet);
                entityManager.AddComponentData(planet, LocalTransform.FromScale(1f));
                if (parent != Entity.Null)
                {
                    entityManager.AddComponentData(planet, new Parent { Value = parent });
                }
                
                
                
                Entity model = entityManager.CreateEntity();

#if  UNITY_EDITOR
                entityManager.SetName(model, "Model " + entityManager.GetComponentData<Planet.Name>(planet).Text);
#endif
                // Create model
                RenderMeshUtility.AddComponents(model, entityManager, desc, renderMeshArray, MaterialMeshInfo.FromRenderMeshArrayIndices(0,0));
                entityManager.AddComponentData(model, LocalTransform.FromScale(.5f));
                entityManager.AddComponentData(model, new Parent { Value = planet });
                entityManager.AddComponentData(model, new URPMaterialPropertyEmissionColor { Value = color });


            }
            
            query.Dispose();
            entities.Dispose();
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