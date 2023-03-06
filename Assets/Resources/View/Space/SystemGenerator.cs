using System.Linq;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Model.Utilities;
using UnityEngine;


namespace Bserg.View.Space
{
    public class SystemGenerator : MonoBehaviour
    {
        public GameObject planetPrefab, orbitPrefab;
        public Transform orbitParent;
        public SystemPrefab prefab;
        private static readonly int Emmision = Shader.PropertyToID("_Emmision");

        public int offsetFromSun;
        public float distanceMult = 1f, sizeMult = 1f; 
        public Planet[] CreateSystem()
        {
            Planet[] planets = new Planet[prefab.planetScriptables.Length];
            
            // Remove old planets
            int n = transform.childCount;
            for (int i = 0; i < n; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
            for (int i = 0; i < n - 1; i++)
                DestroyImmediate(orbitParent.GetChild(0).gameObject);

            // Create new ones
            for (int i = 0; i < prefab.planetScriptables.Length; i++)
            {
                PlanetScriptable planetScriptable = prefab.planetScriptables[i];
                
                GameObject go = Instantiate(planetPrefab, transform);
                go.GetComponent<PlanetIDScript>().planetID = i;
                
                go.name = planetScriptable.Name;
                go.GetComponent<MeshRenderer>().material = planetScriptable.material;
                
                go.transform.localScale = GetRealPlanetSize(planetScriptable.Size);
                go.GetComponent<SphereCollider>().radius = GetIconPlanetSize(planetScriptable.Size).x / go.transform.localScale.x;

                // Only change color during play mode
                if (Application.isPlaying)
                    go.GetComponent<MeshRenderer>().material.SetColor(Emmision, planetScriptable.Color);
                
                if (i == 0)
                    go.transform.position = Vector3.zero;
                else
                    go.transform.position = GetPlanetPosition((float)planetScriptable.RadiusAU, 0);

                // Just for shortcut
                PlanetScriptable p = planetScriptable;
                planets[i] = new Planet(go.transform.position, p.Name, p.Color, p.Size, new Mass(p.WeightEarthMass, Mass.UnitType.EarthMass), new Length(p.RadiusAU, Length.UnitType.AstronomicalUnits));
                
                // ORBIT
                if (i == 0) continue;
                GameObject orbitOG = Instantiate(orbitPrefab, orbitParent);
                orbitOG.transform.localScale = Vector3.one  * (AUToWorld((float)planetScriptable.RadiusAU) * 4);
            }

            return planets;
        }


        public float AUToWorld(float orbitRadiusAU)
        {
            return orbitRadiusAU * distanceMult;
        }

        /// <summary>
        /// Returns the position of the planet on an ellipse
        /// https://www.youtube.com/watch?v=pRvVK2m_wGE
        /// kepler coordinates
        /// </summary>
        /// <param name="semiMajorAxisAU">a</param>
        /// <param name="semiMinorAxisAU">b</param>
        /// <param name="angle">theta</param>
        /// <param name="offsetXAt0">distance from center to focus (orbit/sun) at 0 degrees </param>
        /// <param name="offsetAngle">how it is rotated</param>
        /// <returns></returns>
        public Vector3 GetPlanetPosition(float semiMajorAxisAU, float semiMinorAxisAU, float angle, float offsetXAt0 = 0, float offsetAngle = 0)
        {
            // Taken from https://en.wikipedia.org/wiki/Ellipse parameterization of an ellipse
         

            // Adding the rotation equation https://math.stackexchange.com/questions/2645689/what-is-the-parametric-equation-of-a-rotated-ellipse-given-the-angle-of-rotatio
            float a = AUToWorld(semiMajorAxisAU);
            float b = AUToWorld(semiMinorAxisAU);
            float x = a * Mathf.Cos(angle) * Mathf.Cos(offsetAngle) - b * Mathf.Sin(angle) * Mathf.Sin(offsetAngle);
            float y = a * Mathf.Cos(angle) * Mathf.Sin(offsetAngle) + b * Mathf.Sin(angle) * Mathf.Cos(offsetAngle);
            float cx = AUToWorld(offsetXAt0) * Mathf.Cos(offsetAngle);
            float cy = AUToWorld(offsetXAt0) * Mathf.Sin(offsetAngle);
            

                
            return new Vector3(x + cx, y + cy, 0);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trueAnomaly">based on orbiting object, nu</param>
        /// <param name="radius">in au</param>
        /// <param name="offsetXAt0">in au</param>
        /// <param name="offsetAngle">in radians</param>
        /// <returns></returns>
        public Vector3 GetPositionInOrbit(float trueAnomaly, float radius, float offsetXAt0 = 0, float offsetAngle = 0)
        {
            float r = AUToWorld(radius);

            float x = r * Mathf.Cos(trueAnomaly) * Mathf.Cos(offsetAngle) - r * Mathf.Sin(trueAnomaly) * Mathf.Sin(offsetAngle);
            float y = r * Mathf.Cos(trueAnomaly) * Mathf.Sin(offsetAngle) + r * Mathf.Sin(trueAnomaly) * Mathf.Cos(offsetAngle);
            float cx = AUToWorld(offsetXAt0) * Mathf.Cos(offsetAngle);
            float cy = AUToWorld(offsetXAt0) * Mathf.Sin(offsetAngle);
            
            return new Vector3(x + cx, y + cy, 0);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radiusDeparture">au</param>
        /// <param name="radiusDestination">au</param>
        /// <param name="semiMajorAxis">au</param>
        /// <param name="eccentricity"></param>
        /// <param name="distanceTraveled"></param>
        /// <param name="offsetAngle"></param>
        /// <returns></returns>
        public Vector3 GetPositionInOrbit(float radiusDeparture, float radiusDestination, float semiMajorAxis, float eccentricity, float distanceTraveled, float offsetAngle = 0)
        {
            float offsetAtX0 = radiusDeparture - Mathf.Min(radiusDeparture, radiusDestination);
            float meanAnomaly = distanceTraveled * Mathf.PI;
                
            // If the destination is closer to the sun, then invert 
            if (radiusDeparture > radiusDestination)
            {
                meanAnomaly += Mathf.PI;
                offsetAngle += Mathf.PI;
                offsetAtX0 = radiusDestination - Mathf.Min(radiusDeparture, radiusDestination);
            }
                
            float nu = EllipseMechanics.MeanAnomalyToTrueAnomaly(meanAnomaly, eccentricity);
            float r = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(nu));
            
            return GetPositionInOrbit(nu, r, offsetAtX0 , offsetAngle);
        }

        public Vector3 GetPlanetPosition(float orbitRadiusAU, float angle) =>
            GetPlanetPosition(orbitRadiusAU, orbitRadiusAU, angle);



        public float reduceSize = .01f;
        public Vector3 GetRealPlanetSize(float size) => Vector3.one * size * reduceSize;

        public Vector3 GetIconPlanetSize(float size) =>
            Vector3.one * (2-1/(1+size));
        
        public Planet[] GetPlanets()
        {
            return CreateSystem();
        }

        public string[] GetNames() => prefab.planetScriptables.Select(p => p.Name).ToArray();
        public long[] GetPopulations() => prefab.planetScriptables.Select(p => p.Population).ToArray();
        public float[] GetPopulationLevels() => prefab.planetScriptables.Select(p => Util.LongToLevel(p.Population)).ToArray();
    }
}
