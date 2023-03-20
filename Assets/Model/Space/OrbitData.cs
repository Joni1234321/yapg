using System;
using System.Collections.Generic;
using Bserg.Model.Space.Components;
using Bserg.Model.Units;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Assertions;
using Time = Bserg.Model.Units.Time;

namespace Bserg.Model.Space
{
    /// <summary>
    /// Contains information of what planet orbits what
    /// The sun will have its children be the planets
    /// The earth will have its children be the moon, and other satellites
    /// </summary>
    public class OrbitData
    {
        // ReSharper disable once InconsistentNaming
        public static OrbitData EMPTY() => new OrbitData(-1);
        public readonly int PlanetID;
        public readonly List<OrbitData> Children;
        
        // Only add after
        public Entity Entity;
        public Length OrbitRadius;
        public NativeHashMap<Entity, HohmannTransfer>.ReadOnly NeighbourTransfers;       
        
        public OrbitData(int planetID)
        {
            PlanetID = planetID;
            Entity = Entity.Null;
            Children = new List<OrbitData>();
        }

        /// <summary>
        /// Adds planetID to any child that has id equal to orbitID
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="orbitID"></param>
        /// <returns>true if have added succesfully</returns>
        public bool Add(int planetID, int orbitID)
        {
            if (PlanetID == orbitID)
            {
                Children.Add(new OrbitData(planetID));
                return true;
            }
            
            for (int i = 0; i < Children.Count; i++)
                if (Children[i].Add(planetID, orbitID))
                    return true;
            return false;
        }

        /// <summary>
        /// Gets planet with id
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="result"> the result</param>
        /// <returns>if it exists</returns>
        public bool Get(int planetID, out OrbitData result)
        {
            if (planetID == PlanetID)
            {
                result = this;
                return true;
            }
            
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Get(planetID, out result))
                    return true;
            }

            result = EMPTY();
            return false;
        }
        
        
        public bool Get(Entity entity, out OrbitData result)
        {
            if (entity == Entity)
            {
                result = this;
                return true;
            }
            
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Get(entity, out result))
                    return true;
            }

            result = EMPTY();
            return false;
        }

        /// <summary>
        /// Updates the class' hohmann transfers
        /// </summary>
        public void GenerateTransfersForChildren(EntityManager entityManager, ref NativeHashMap<EntityPair, HohmannTransfer> map)
        {
            StandardGravitationalParameter orbitMu = Entity == Entity.Null
                ? new StandardGravitationalParameter { Value = 0 }
                : entityManager.GetComponentData<StandardGravitationalParameter>(Entity);

            // Calculate hashmap between children
            foreach (OrbitData departure in Children)
            {
                // Should not compute empty children
                Assert.IsTrue(departure.Entity != Entity.Null);

                Time departurePeriod = GameTick.ToTime(
                    entityManager.GetComponentData<OrbitPeriod>(departure.Entity).TicksF);
                
                // From each departure to destination
                foreach (OrbitData destination in Children)
                {
                    // Should not compute empty children
                    Assert.IsTrue(destination.Entity != Entity.Null);
                    if (departure.Entity == destination.Entity)
                        continue;

                    Time destinationPeriod = GameTick.ToTime(
                        entityManager.GetComponentData<OrbitPeriod>(destination.Entity).TicksF);

                    HohmannTransfer transfer = new HohmannTransfer(orbitMu,
                        departurePeriod, destinationPeriod,
                        departure.OrbitRadius, destination.OrbitRadius);
                    
                    // Add 
                    EntityPair key = new EntityPair
                    {
                        Departure = departure.Entity,
                        Destination = destination.Entity
                    };
                    map.Add(key,transfer);
                }

                // Recursively generate for all children
                departure.GenerateTransfersForChildren(entityManager, ref map);
            }
            
        }
        
    }


    
    /// <summary>
    /// Hohmann transfer has a launch window and a duration to destination
    /// </summary>
    public struct HohmannTransfer
    {
        public readonly float Offset, Window, Duration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset">first launch window</param>
        /// <param name="window">delta time till next launch windows</param>
        /// <param name="duration">time it takes from launch at departure till destination</param>
        public HohmannTransfer(float offset, float window, float duration)
        {
            Offset = offset;
            Window = window;
            Duration = duration;
        }

        /// <summary>
        /// Calculates the hohmann transfer info between two planets that share a common orbit
        /// </summary>
        /// <param name="commonMu">Gravitational constant for common orbit</param>
        /// <param name="departureOrbitalPeriod">Orbital period for planet departing from</param>
        /// <param name="destinationOrbitalPeriod">Orbital period for planet going to</param>
        /// <param name="departureRadius">Radius of planet departing from </param>
        /// <param name="destinationRadius">Radius of planet going to</param>
        /// <returns></returns>
        public HohmannTransfer(StandardGravitationalParameter commonMu, Time departureOrbitalPeriod,
            Time destinationOrbitalPeriod, Length departureRadius, Length destinationRadius)
        {
            Time window = 
                OrbitalMechanics.GetSynodicPeriod(departureOrbitalPeriod, destinationOrbitalPeriod);
            Time duration =
                OrbitalMechanics.HohmannTransferDuration(new StandardGravitationalParameterOld
                    {
                        val = commonMu.Value
                    },
                    departureRadius,
                    destinationRadius);
            double deltaAngleAtLaunch =
                OrbitalMechanics.HohmannAngleDifferenceAtLaunch(destinationOrbitalPeriod, duration);
            Time timeUntilFirstWindow = OrbitalMechanics.GetTimeUntilAngleDifference(deltaAngleAtLaunch,
                departureOrbitalPeriod,
                destinationOrbitalPeriod);

            Offset = GameTick.ToTickF(timeUntilFirstWindow);
            Window = GameTick.ToTickF(window);
            Duration = GameTick.ToTickF(duration);
        } 
    }

    /// <summary>
    /// Used to flatten dictionary 
    /// </summary>
    public struct EntityPair : IEquatable<EntityPair>
    {
        public Entity Departure;
        public Entity Destination;

        public bool Equals(EntityPair other)
        {
            return Departure.Equals(other.Departure) && Destination.Equals(other.Destination);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            // Suitable nullity checks etc, of course :)
            hash = hash * 23 + Departure.GetHashCode();
            hash = hash * 23 + Destination.GetHashCode();
            return hash;
            
            // Got an error with burst
            return HashCode.Combine(Departure, Destination);
        }
    }
    
}