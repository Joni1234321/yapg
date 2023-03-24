using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.Interfaces;
using Bserg.Controller.WorldRenderer;
using Bserg.Model.Shared.Components;
using Bserg.Model.Space.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Rendering;

namespace Bserg.Controller.VisualEntities
{
    public struct TravelingSpacecraftVisual : IEntityVisual<TravelingSpacecraftVisual>
    {
        public Entity Main;
        public Entity Orbit;

        public void Assign(EntityManager entityManager, Entity model)
        {
            entityManager.SetComponentData(Main, new EntityModel { Value = model } );
            
            Spacecraft.FlightPlan flightPlan = entityManager.GetComponentData<Spacecraft.FlightPlan>(model);

            Entity departurePlanet = flightPlan.CurrentFlightStep.DestinationPlanet;
            Entity destinationPlanet = flightPlan.NextFlightStep.DestinationPlanet;

            float departureTickF = entityManager.GetComponentData<Spacecraft.DepartureTick>(model).TickF;
            
#if  UNITY_EDITOR
            FixedString32Bytes name = entityManager.GetComponentData<Planet.Name>(departurePlanet).Text;
            entityManager.SetName(Main, "Spaceflight: " + name);
            entityManager.SetName(Orbit, "Orbit: " + name);
#endif

            float distanceTraveled = .5f;//((ticks + dt) - flight.DepartureTick) / (flight.DestinationTick - flight.DepartureTick);

            
            float r1 = entityManager.GetComponentData<OrbitRadius>(departurePlanet).RadiusAU;
            float r2 = entityManager.GetComponentData<OrbitRadius>(destinationPlanet).RadiusAU;

            float p1 = entityManager.GetComponentData<OrbitPeriod>(departurePlanet).TicksF;
            
            float a = (r1 + r2) * .5f;
            float c = SpaceFlightRenderer.GetLinearEccentricity(a, math.min(r1, r2));
            float b = SpaceFlightRenderer.GetSemiMinorAxis(a, c);
            float e = c / a;

            
            float offsetAngle = PlanetRenderer.GetPlanetAngleAtTicksF(p1, departureTickF);
            float startAngle = math.fmod(offsetAngle, 2 * math.PI);

            entityManager.SetComponentData(Main, LocalTransform.FromPosition(
                SystemGenerator.GetPositionInOrbit(r1, r2, a, e, distanceTraveled, offsetAngle)
                ));
            
                
            float diff = SystemGenerator.AUToWorld(r1 - a);
            float3 position = new float3(diff * math.cos(startAngle), diff * math.sin(startAngle), 0);
            float3 scale = SystemGenerator.AUToWorld(4) * new float3(b, a, 1);
           
#if true
            entityManager.SetComponentData(Orbit, LocalTransform.FromPositionRotation(
                position,
                quaternion.RotateZ(math.degrees(startAngle) - 90)
                ));
            entityManager.SetComponentData(Orbit, new PostTransformMatrix { Value = float4x4.Scale(scale)});
            
#else
            entityManager.SetComponentData(Orbit, new LocalToWorld
            {
                Value = float4x4.TRS(position, quaternion.RotateZ(math.degrees(startAngle) - 90), scale)
            });
#endif
        }

        public void Enable(EntityManager entityManager)
        {
            entityManager.RemoveComponent<DisableRendering>(Main);
            entityManager.RemoveComponent<DisableRendering>(Orbit);
        }

        public void Disable(EntityManager entityManager)
        {
#if UNITY_EDITOR
            FixedString32Bytes name = "Disabled";
            entityManager.SetName(Main, "Spaceship: " + name);
            entityManager.SetName(Orbit, "Orbit: " + name);
#endif
            
            entityManager.AddComponent<DisableRendering>(Main);
            entityManager.AddComponent<DisableRendering>(Orbit);
        }

        public TravelingSpacecraftVisual Clone(EntityManager entityManager)
        {
            var clone = new TravelingSpacecraftVisual
            {
                Main = entityManager.Instantiate(Main),
                Orbit = entityManager.Instantiate(Orbit),
            };
            
            var buffer = entityManager.AddBuffer<LinkedEntityGroup>(clone.Main);
            buffer.Add(new LinkedEntityGroup { Value = clone.Main });
            buffer.Add(new LinkedEntityGroup { Value = clone.Orbit });

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
                Orbit = CreateOrbitPrototype(entityManager, desc, meshArray),
            };

            return prototype;
        }

        Entity CreatePositionPrototype(EntityManager entityManager)
        {
            Entity e = entityManager.CreateEntity();
            entityManager.AddComponent<EntityModel>(e);
            entityManager.AddComponent<LocalTransform>(e);
            entityManager.AddComponent<LocalToWorld>(e);
            entityManager.AddComponent<SpaceTransform.MoveOnCircle>(e); 

            return e;
        }
        Entity CreateOrbitPrototype(EntityManager entityManager, RenderMeshDescription desc,
            RenderMeshArray meshArray)
        {
            Entity e = entityManager.CreateEntity();

            RenderMeshUtility.AddComponents(e, entityManager, desc, meshArray,
                MaterialMeshInfo.FromRenderMeshArrayIndices(1, 1));
            entityManager.AddComponent<LocalTransform>(e);
            entityManager.AddComponent<PostTransformMatrix>(e);

            return e;
        }
    }
}