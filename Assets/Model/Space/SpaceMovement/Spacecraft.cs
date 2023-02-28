using System.Collections.Generic;

namespace Bserg.Model.Space.SpaceMovement
{
    // Calculate ORBITS
    // http://www.phy6.org/stargaze/Smars1.htm
    // https://www.jpl.nasa.gov/edu/teach/activity/lets-go-to-mars-calculating-launch-windows/
    // Orbits here https://en.wikipedia.org/wiki/Orbital_maneuver
    // Low energy transfer, used between moons of planets
    // THey are using the interplanetary transport network. MOST FUEL EFFICIENT
    // Low energy orbits 
    // https://en.wikipedia.org/wiki/Low-energy_transfer ( 20 % fuel efficiency, but much slower 90 days instead of 7 days)
    // Basically all orbits are just elliptical shapes, that u can change by velocity
    // So one orbit is just increase your elliptical apogee to collide with the wished orbit, and use speed to change current orbit to fit the wished orbit
    // So maybe do stuff where u have options between different type of orbits.
    // One fuel efficient and relatively good (Hohmann orbit or), WEEKS
    // Then one very fuel efficient LOW ENERGY TRANSFER, MONTHS
    // Only use ITA for moon transfers, basically no fuel, but takes way too long YEARS to the moon
    // And then direct DAYS to the moon
    
    // 2 Burn Effictient most times 
    // 3 Burn Efficient more but slower
    // Overshoot Very fast but inefficient
    
    
    // OK OK PLANETARY MECHANICS
    // https://en.wikipedia.org/wiki/Standard_gravitational_parameter
    // Time for orbit around SUN 
    // 2 * PI / (sqrt(Mu)) * sqrt(radius^3) = T  

    /// <summary>
    /// Contains info to move spacecraft
    /// Also contains info how much in the spacecraft
    /// Contains info how close to destination
    /// </summary>
    public struct Spacecraft
    {
        public long Population, MaxPopulation;
        public Queue<Step> Steps;
        
        public Spacecraft(long maxPopulation)
        {
            Steps = new Queue<Step>();
            Population = 0;
            MaxPopulation = maxPopulation;
        }
        
        public enum StepType
        {
            Unload,
            Load,
        }
        public struct Step
        {
            public int DestinationID;
            public StepType Type;

            public Step(int destinationID, StepType type)
            {
                DestinationID = destinationID;
                Type = type;
            }
        }
    }




}