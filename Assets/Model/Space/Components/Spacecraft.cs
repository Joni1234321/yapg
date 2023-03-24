using System;
using Unity.Entities;

namespace Bserg.Model.Space.Components
{
    public struct Spacecraft
    {
        public struct Tag : IComponentData {}
        
        /// <summary>
        /// Mutuably excluded with arrivaltick, shoudl probably make it a shared variable
        /// </summary>
        public struct DepartureTick : IComponentData
        {
            public float TickF;
        }    
        public struct ArrivalTick : IComponentData
        {
            public float TickF;
        }

    
    
        /// <summary>
        /// Cargo of the spacecraft
        /// </summary>
        public struct Cargo : IComponentData
        {
            public int Population;
        }

        public struct ProcessingTag : IComponentData
        {
        }
        public struct TravelingTag : IComponentData
        {
        }
    
        /// <summary>
        /// Spacecrafts current and next step, and also how many times it should alter between the two
        /// Maybe do a dynamicbuffer thing, if you want smarter plans
        /// </summary>
        public struct FlightPlan : IComponentData
        {
            public FlightStep CurrentFlightStep;
            public FlightStep NextFlightStep;

            public int ActionsLeft;
        } 
    
        public enum ActionType
        {
            Unload,
            Load,
        }
        public struct FlightStep
        {
            public Entity DestinationPlanet;
            public ActionType Action;
        }
        
        public static bool CreateSpacecraft(EntityManager entityManager, Spacecraft.FlightStep firstStep, Spacecraft.FlightStep secondStep = default, int actions = 1)
        {
            if (!entityManager.HasComponent<SpacecraftPool>(firstStep.DestinationPlanet))
                return false;
                
            if (entityManager.GetComponentData<SpacecraftPool>(firstStep.DestinationPlanet).Available == 0)
                return false;
            
            Entity ship = entityManager.CreateEntity();
            entityManager.AddComponentData(ship, new Spacecraft.Tag());
            entityManager.AddComponentData(ship, new Spacecraft.ProcessingTag());
            entityManager.AddComponentData(ship, new Spacecraft.Cargo { Population = 0 });
            entityManager.AddComponentData(ship, new Spacecraft.FlightPlan
            {
                CurrentFlightStep = firstStep,
                NextFlightStep = secondStep,
                ActionsLeft = actions,
            });
            
            #if UNITY_EDITOR
            entityManager.SetName(ship, $"Spaceship {entityManager.GetName(firstStep.DestinationPlanet)} - {entityManager.GetName(secondStep.DestinationPlanet)}");
            #endif

            return true;
        }
        

    }
}