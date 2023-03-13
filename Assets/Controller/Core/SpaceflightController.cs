using Bserg.Controller.World;
using Bserg.Model.Core;
using Bserg.Model.Core.Systems;
using Bserg.Model.Space.SpaceMovement;
using Bserg.Model.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Bserg.Controller.Core
{
    public class SpaceflightController
    {
        private Transform spaceflightParent, flightOrbitParent;
        private GameObject spacecraftPrefab, flightOrbitPrefab, oneOrbit;
        private PlanetRenderer planetRenderer;

        public SpaceflightController(PlanetRenderer planetRenderer)
        {
            spacecraftPrefab = Resources.Load<GameObject>("View/Shaders/Spacecraft/Spacecraft");
            flightOrbitPrefab = Resources.Load<GameObject>("View/Shaders/Line/FlightOrbit");
            
            spaceflightParent = new GameObject("Spaceflights").transform;
            flightOrbitParent = new GameObject("Orbits").transform;
            
            this.planetRenderer = planetRenderer;
        }
        
        public void Update(Game game, float dt)
        {   
            ShowSpaceflights(game, game.SpaceflightSystem, dt);
        }

        private bool active = true;
        /// <summary>
        /// Show the current spaceflights and their progress
        /// </summary>
        public void ShowSpaceflights(Game game, SpaceflightSystem spaceflightSystem, float dt)
        {
            if (active && !Input.GetKey(KeyCode.Tab))
            {
                active = false;
                spaceflightParent.gameObject.SetActive(false);
                flightOrbitParent.gameObject.SetActive(false);
            }

            if (!active && Input.GetKey(KeyCode.Tab))
            {
                active = true;
                spaceflightParent.gameObject.SetActive(true);
                flightOrbitParent.gameObject.SetActive(true);
            }

            int flights = spaceflightSystem.Spaceflights.Count;
            int missingGameObjects = flights - spaceflightParent.childCount ;

            // Add more gameobjects
            for (int i = 0; i < missingGameObjects; i++)
            {
                Object.Instantiate(spacecraftPrefab, spaceflightParent);
                Object.Instantiate(flightOrbitPrefab, flightOrbitParent);

            }
            // Remove not used
            for (int i = 0; i < -missingGameObjects; i++)
            {
                Object.Destroy(spaceflightParent.GetChild(0).gameObject);
                Object.Destroy(flightOrbitParent.GetChild(0).gameObject);
            }

            for (int i = 0; i < flights; i++)
            {
                Spaceflight flight = spaceflightSystem.Spaceflights[i];
                
                // Destination percentage traveled
                float distanceTraveled = (float)((game.Ticks + dt) - flight.DepartureTick) / (flight.DestinationTick - flight.DepartureTick);
                float startAngle = planetRenderer.GetPlanetAngleAtTicksF(flight.DepartureID, flight.DepartureTick) % (2 * Mathf.PI);
                
                // Means it has reached target
                if (distanceTraveled > 1)
                    continue;
    
                float r1 = (float)game.Planets[flight.DepartureID].OrbitRadius.To(Length.UnitType.AstronomicalUnits);
                float r2 = (float)game.Planets[flight.DestinationID].OrbitRadius.To(Length.UnitType.AstronomicalUnits);

                float a = (r1 + r2) * .5f;
                float c = GetLinearEccentricity(a, Mathf.Min(r1, r2));
                float b = GetSemiMinorAxis(a, c);
                float offsetAngle =
                    planetRenderer.GetPlanetAngleAtTicksF(flight.DepartureID, flight.DepartureTick);
                
                // 
                // Eccentricity
                float e = c / a;
                // Calculate true anomaly
                // https://en.wikipedia.org/wiki/True_anomaly

                spaceflightParent.GetChild(i).transform.position = planetRenderer.SystemGenerator.GetPositionInOrbit(r1, r2, a, e, distanceTraveled, offsetAngle);
                
                // Orbit
                float diff = planetRenderer.SystemGenerator.AUToWorld(r1 - a);
                flightOrbitParent.GetChild(i).transform.position = new Vector3(diff * Mathf.Cos(startAngle), diff * Mathf.Sin(startAngle), 0);
                flightOrbitParent.GetChild(i).transform.localScale = planetRenderer.SystemGenerator.AUToWorld(4) * new Vector3(b, a, 1);
                flightOrbitParent.GetChild(i).transform.eulerAngles = new Vector3(0, 0, startAngle * Mathf.Rad2Deg - 90);
            }
        }

        /// <summary>
        /// Returns the distance from center of ellipse to focus point
        /// </summary>
        /// <param name="semiMajorAxis">a</param>
        /// <param name="closestDistanceToOrbitingObject">distance to Focus point</param>
        /// <returns></returns>
        float GetLinearEccentricity(float semiMajorAxis, float closestDistanceToOrbitingObject)
        {
            //https://en.wikipedia.org/wiki/Ellipse Linear eccentricity
            return Mathf.Abs(semiMajorAxis - closestDistanceToOrbitingObject);
        } 
        
        /// <summary>
        /// Returns b of the ellipse
        /// </summary>
        /// <param name="semiMajorAxis">a</param>
        /// <param name="linearEccentricity">c</param>
        /// <returns></returns>
        float GetSemiMinorAxis(float semiMajorAxis, float linearEccentricity)
        {
            //https://en.wikipedia.org/wiki/Ellipse Linear eccentricity
            // b^2 = a^2 - c^2
            return Mathf.Sqrt(semiMajorAxis * semiMajorAxis - linearEccentricity * linearEccentricity);

        }
    }
}