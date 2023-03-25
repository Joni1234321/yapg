using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.Interfaces;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Bserg.Controller.VisualEntities
{
    public struct PlanetVisual : IComponentData, IEntityVisual<PlanetVisual>
    {
        public Entity Main;
        public Entity Planet;
        public Entity Orbit;


        public void Assign(EntityManager entityManager, Entity model)
        {
#if  UNITY_EDITOR
            FixedString32Bytes name = entityManager.GetComponentData<Planet.Name>(model).Text;
            entityManager.SetName(Main, "Planet: " + name);
            entityManager.SetName(Planet, "Planet Model: " + name);
            entityManager.SetName(Orbit, "Planet Orbit: " + name);

#endif
            Planet.Data planetData = entityManager.GetComponentData<Planet.Data>(model);
            float orbitRadiusWorld = SystemGenerator.AUToWorld(
                (float)planetData.OrbitRadius.To(Length.UnitType.AstronomicalUnits));
            
            entityManager.SetComponentData(Main, new EntityModel { Value = model });
            
            AssignPosition(entityManager, model, orbitRadiusWorld);
            AssignModel(entityManager, model, planetData);
            AssignOrbit(entityManager, model, orbitRadiusWorld);
        }

        public void Enable(EntityManager entityManager)
        {
            entityManager.RemoveComponent<DisableRendering>(Main);
            entityManager.RemoveComponent<DisableRendering>(Planet);
            entityManager.RemoveComponent<DisableRendering>(Orbit);
        }

        public void Disable(EntityManager entityManager)
        {
            
#if UNITY_EDITOR
            FixedString32Bytes name = "Disabled";
            entityManager.SetName(Main, "View: " + name);
            entityManager.SetName(Planet, "Planet: " + name);
            entityManager.SetName(Orbit, "Orbit: " + name);
#endif
            entityManager.AddComponent<DisableRendering>(Main);
            entityManager.AddComponent<DisableRendering>(Planet);
            entityManager.AddComponent<DisableRendering>(Orbit);
        }
        
        public PlanetVisual Clone(EntityManager entityManager)
        {
            var clone = new PlanetVisual
            {
                Main = entityManager.Instantiate(Main),
                Planet = entityManager.Instantiate(Planet),
                Orbit = entityManager.Instantiate(Orbit),
            };

            entityManager.SetComponentData(clone.Planet, new Parent { Value = clone.Main });

            var buffer = entityManager.AddBuffer<LinkedEntityGroup>(clone.Main);
            buffer.Add(new LinkedEntityGroup { Value = clone.Main });
            buffer.Add(new LinkedEntityGroup { Value = clone.Planet });
            buffer.Add(new LinkedEntityGroup { Value = clone.Orbit });

            return clone;
        }

        public PlanetVisual CreatePrototype(EntityManager entityManager, RenderMeshArray meshArray)
        {
            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);

            var prototype = new PlanetVisual
            {
                Main = CreatePlanetPositionPrototype(entityManager),
                Planet = CreateModelPrototype(entityManager, desc, meshArray),
                Orbit = CreateOrbitPrototype(entityManager, desc, meshArray),
            };
            
            return prototype;
        }


        #region Assign
        void AssignPosition(EntityManager entityManager, Entity planet, float orbitRadiusWorld)
        {
            entityManager.SetComponentData(Main, new SpaceTransform.MoveOnCircle
            {
                PeriodTicksF = entityManager.GetComponentData<OrbitPeriod>(planet).TicksF,
                Radius = orbitRadiusWorld,
                Angle0 = 0,
            });
        }


        void AssignModel(EntityManager entityManager, Entity planet, Planet.Data planetData)
        {
            // Color
            entityManager.SetComponentData(Planet, new URPMaterialPropertyEmissionColor { Value = planetData.Color });
            entityManager.SetComponentData(Planet, new URPMaterialPropertyBaseColor { Value = planetData.Color });
            
            // Scale
            entityManager.SetComponentData(Planet, LocalTransform.FromScale(SystemGenerator.GetRealPlanetSize(planetData.Size).x));
            entityManager.SetComponentData(Planet, new SpaceTransform.UIWorldTransition
            {
                WorldScale = SystemGenerator.GetRealPlanetSize(planetData.Size).x,
                UIScale = SystemGenerator.GetIconPlanetSize(planetData.Size).x,
                WorldMaterialIndex = 0,
                WorldMeshIndex = 0,
                UIMaterialIndex = 2,
                UIMeshIndex = 1,
            });
        }
        void AssignOrbit(EntityManager entityManager, Entity planet, float orbitRadiusWorld)
        {
            entityManager.SetComponentData(Orbit, LocalTransform.FromScale(orbitRadiusWorld * 4));
            entityManager.SetComponentData(Orbit, new SpaceTransform.Rotate
            {
                PeriodTicksF = entityManager.GetComponentData<OrbitPeriod>(planet).TicksF
            });
        }
        #endregion
        
        #region Prototypes
        Entity CreatePlanetPositionPrototype(EntityManager entityManager)
        {
            Entity e = entityManager.CreateEntity();
            entityManager.AddComponent<EntityModel>(e);
            entityManager.AddComponentData(e, LocalTransform.FromScale(1f));
            entityManager.AddComponent<LocalToWorld>(e);
            entityManager.AddComponent<SpaceTransform.MoveOnCircle>(e);

            return e;
        }

        Entity CreateModelPrototype(EntityManager entityManager, RenderMeshDescription desc,
            RenderMeshArray meshArray)
        {
            Entity e = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(e, entityManager, desc, meshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(2, 1));
            entityManager.AddComponent<Parent>(e);
            entityManager.AddComponent<LocalTransform>(e);
            entityManager.AddComponent<URPMaterialPropertyEmissionColor>(e);
            entityManager.AddComponent<URPMaterialPropertyBaseColor>(e);
            entityManager.AddComponent<SpaceTransform.UIWorldTransition>(e);

            return e;
        }

        Entity CreateOrbitPrototype(EntityManager entityManager, RenderMeshDescription desc,
            RenderMeshArray meshArray)
        {
            Entity e = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(e, entityManager, desc, meshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(1, 1));
            entityManager.AddComponent<LocalTransform>(e);
            entityManager.AddComponent<SpaceTransform.Rotate>(e);

            return e;
        }
        #endregion
       
        public void Destroy(EntityManager entityManager)
        {
            entityManager.DestroyEntity(Main);
            entityManager.DestroyEntity(Planet);
            entityManager.DestroyEntity(Orbit);
        }
    }
}