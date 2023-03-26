
using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.Interfaces;
using Bserg.Controller.WorldRenderer;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space;
using Bserg.Model.Space.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;


namespace Bserg.Controller.VisualEntities
{
    

    public struct TravelingSpacecraftVisual : IEntityVisual<TravelingSpacecraftVisual>
    {
        public struct Model : IComponentData { public Entity Value; }

        public Entity Main;
        public Entity Spacecraft;
        public Entity Orbit;

        public void Assign(EntityManager entityManager, Entity model)
        {
            entityManager.SetComponentData(Main, new Model { Value = model } );
            
#if  UNITY_EDITOR && FALSE
            FixedString32Bytes name = entityManager.GetComponentData<Planet.Name>(departurePlanet).Text + " - " +
                                      entityManager.GetComponentData<Planet.Name>(destinationPlanet).Text;
            entityManager.SetName(Main, "Traveling Spacecraft: " + name);
            entityManager.SetName(Spacecraft, "Spacecraft: " + name);
            entityManager.SetName(Orbit, "Traveling Spacecraft Orbit: " + name);
#endif
            
            Spacecraft.FlightPlan flightPlan = entityManager.GetComponentData<Spacecraft.FlightPlan>(model);

            Entity departurePlanet = flightPlan.NextFlightStep.DestinationPlanet;
            Entity destinationPlanet = flightPlan.CurrentFlightStep.DestinationPlanet;

            float departureTickF = entityManager.GetComponentData<Spacecraft.DepartureTick>(model).TickF;
            float arrivalTickF = entityManager.GetComponentData<Spacecraft.ArrivalTick>(model).TickF;

            float r1 = entityManager.GetComponentData<OrbitRadius>(departurePlanet).RadiusAU;
            float r2 = entityManager.GetComponentData<OrbitRadius>(destinationPlanet).RadiusAU;

            float p1 = entityManager.GetComponentData<OrbitPeriod>(departurePlanet).TicksF;
            
            float a = (r1 + r2) * .5f;
            float c = EllipseMechanics.GetLinearEccentricity(a, math.min(r1, r2));
            float b = EllipseMechanics.GetSemiMinorAxis(a, c);
            float e = c / a;

            
            float startAngle = PlanetRenderer.GetPlanetAngleAtTicksF(p1, departureTickF);

            entityManager.SetComponentData(Main, new SpaceTransform.MoveOnEllipticalOrbit
            {
                DepartureTickF = departureTickF,
                ArrivalTickF = arrivalTickF,
                DepartureRadius = r1,
                DestinationRadius = r2,
                Eccentricity = e,
                Angle0 = startAngle % (2 * math.PI),
            });
            
            // Orbit
            float diff = SystemGenerator.AUToWorld(r1 - a);
            float3 position = new float3(diff * math.cos(startAngle), diff * math.sin(startAngle), 0);
            float3 scale = SystemGenerator.AUToWorld(4) * new float3(b, a, 1);
           
            entityManager.SetComponentData(Orbit, LocalTransform.FromPositionRotation(
                position,
                //quaternion.identity
                quaternion.RotateZ(startAngle - .5f * math.PI)
                ));
            entityManager.SetComponentData(Orbit, new PostTransformMatrix { Value = float4x4.Scale(scale)});
        }

        public void Enable(EntityManager entityManager)
        {
            entityManager.RemoveComponent<DisableRendering>(Main);
            entityManager.RemoveComponent<DisableRendering>(Spacecraft);
            entityManager.RemoveComponent<DisableRendering>(Orbit);
        }

        public void Disable(EntityManager entityManager)
        {
#if UNITY_EDITOR && FALSE
            FixedString32Bytes name = "Disabled";
            entityManager.SetName(Main, "Main: " + name);
            entityManager.SetName(Spacecraft, "Spacecraft: " + name);
            entityManager.SetName(Orbit, "Orbit: " + name);
#endif
            
            entityManager.AddComponent<DisableRendering>(Main);
            entityManager.AddComponent<DisableRendering>(Spacecraft);
            entityManager.AddComponent<DisableRendering>(Orbit);
        }

        public TravelingSpacecraftVisual Clone(EntityManager entityManager)
        {
            var clone = new TravelingSpacecraftVisual
            {
                Main = entityManager.Instantiate(Main),
                Spacecraft = entityManager.Instantiate(Spacecraft),
                Orbit = entityManager.Instantiate(Orbit),
            };

            entityManager.AddComponentData(clone.Spacecraft, new Parent { Value = clone.Main });
            
#if UNITY_EDITOR
            entityManager.SetName(Main, "~Spacecraft");
            entityManager.SetName(Spacecraft, "~Spacecraft View");
            entityManager.SetName(Orbit, "~Spacecraft Orbit");
#endif
            /*var buffer = entityManager.AddBuffer<LinkedEntityGroup>(clone.Main);
            buffer.Add(new LinkedEntityGroup { Value = clone.Main });
            buffer.Add(new LinkedEntityGroup { Value = clone.Spacecraft });
            buffer.Add(new LinkedEntityGroup { Value = clone.Orbit });*/

            return clone;
        }

        public TravelingSpacecraftVisual CreatePrototype(EntityManager entityManager, RenderMeshArray meshArray)
        {
            RenderMeshDescription desc = new RenderMeshDescription(
                shadowCastingMode: ShadowCastingMode.Off,
                receiveShadows: false);

            var prototype = new TravelingSpacecraftVisual
            {
                Main = CreatePositionPrototype(entityManager),
                Spacecraft = CreateSpacecraftPrototype(entityManager, desc, meshArray),
                Orbit = CreateOrbitPrototype(entityManager, desc, meshArray),
            };

            return prototype;
        }

        Entity CreatePositionPrototype(EntityManager entityManager)
        {
            Entity e = entityManager.CreateEntity();
            entityManager.AddComponent<Model>(e);
            entityManager.AddComponentData(e, LocalTransform.FromScale(1f));
            entityManager.AddComponent<LocalToWorld>(e);
            entityManager.AddComponent<SpaceTransform.MoveOnEllipticalOrbit>(e); 

            return e;
        }
        Entity CreateSpacecraftPrototype(EntityManager entityManager, RenderMeshDescription desc,
            RenderMeshArray meshArray)
        {
            Entity e = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(e, entityManager, desc, meshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(4, 0));
            entityManager.AddComponentData(e, LocalTransform.FromScale(.2f));
            entityManager.AddComponentData(e, new SpaceTransform.UIWorldTransition
            {
                WorldScale = SystemGenerator.GetRealPlanetSize(.1f).x,
                UIScale = .5f,
                WorldMaterialIndex = 4,
                WorldMeshIndex = 0,
                UIMaterialIndex = 4,
                UIMeshIndex = 0
            });

            return e;
        }

        
        Entity CreateOrbitPrototype(EntityManager entityManager, RenderMeshDescription desc,
            RenderMeshArray meshArray)
        {
            Entity e = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(e, entityManager, desc, meshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(3, 1));
            entityManager.AddComponent<LocalTransform>(e);
            entityManager.AddComponent<PostTransformMatrix>(e);
            return e;
        }

        public void Destroy(EntityManager entityManager)
        {
            entityManager.DestroyEntity(Main);
            entityManager.DestroyEntity(Spacecraft);
            entityManager.DestroyEntity(Orbit);
        }
    }
}