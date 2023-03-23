using System.Linq;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.Model.Utilities;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    public class SystemGenerator : MonoBehaviour
    {
        public GameObject planetPrefab, orbitPrefab;
        public Transform orbitParent;
        public SystemPrefab prefab;
        private static readonly int Emmision = Shader.PropertyToID("_EmissionColor");
        Material material;

        public Transform[] planetTransforms;
        public OrbitData Orbits;
        private static readonly int Width = Shader.PropertyToID("_Width");
        private static readonly int FixedSize = Shader.PropertyToID("_FixedSize");

        private void OnEnable()
        {
            material = Resources.Load<Material>("View/Shaders/MarsMaterial");
        }


        public PlanetOld[] CreateSystem()
        {
            PlanetOld[] planets = new PlanetOld[prefab.planetScriptables.Length];
            Orbits = OrbitData.EMPTY();

            // Remove old planets
            int n = transform.childCount;
            for (int i = 0; i < n; i++)
                DestroyImmediate(transform.GetChild(0).gameObject);
            for (int i = 0; i < n - 1; i++)
                DestroyImmediate(orbitParent.GetChild(0).gameObject);

            planetTransforms = new Transform[prefab.planetScriptables.Length];
            // Create new ones
            for (int i = 0; i < prefab.planetScriptables.Length; i++)
            {
                PlanetScriptable p = prefab.planetScriptables[i];
                planets[i] = CreatePlanet(p);
                planetTransforms[i] = CreatePlanetGameObject(planets[i], i).transform;
                if (!Orbits.Add(i, p.OrbitObject))
                    Debug.LogError("Cant find orbiting object in system");
            }

            return planets;
        }

        PlanetOld CreatePlanet(PlanetScriptable p)
        {
            return new PlanetOld(
                GetPlanetPosition((float)p.RadiusAU, 0), 
                p.Name, 
                p.Color, 
                p.Size,
                new Mass(p.WeightEarthMass, Mass.UnitType.EarthMass),
                new Length(p.RadiusAU, Length.UnitType.AstronomicalUnits),
                p.OrbitObject);
        }
        GameObject CreatePlanetGameObject(PlanetOld planetOld, int planetID)
        {
            GameObject go = Instantiate(planetPrefab, transform);
            go.transform.position = planetOld.StartingPosition;
            int orbitID = planetOld.OrbitObject;
            if (orbitID != -1)
                go.transform.position += planetTransforms[orbitID].transform.position;
                
            go.GetComponent<PlanetIDScript>().planetID = planetID;
                
            go.name = planetOld.Name;
            go.GetComponent<MeshRenderer>().material = material;
                
            go.transform.localScale = GetRealPlanetSize(planetOld.Size);
            //go.GetComponent<SphereCollider>().radius = GetIconPlanetSize(planet.Size).x / go.transform.localScale.x;
            go.GetComponent<SphereCollider>().radius = .6f;
            // Only change color during play mode
            if (Application.isPlaying)
                go.GetComponent<MeshRenderer>().material.SetColor(Emmision, planetOld.Color);
                
            
            // ORBIT
            if (planetOld.OrbitObject == -1)
                return go;
                
            GameObject orbitIndicatorGameObject = Instantiate(orbitPrefab, orbitParent);
            float orbitRadiusWorld =
                AUToWorld((float)planetOld.OrbitRadius.To(Length.UnitType.AstronomicalUnits)) * 4;
            orbitIndicatorGameObject.transform.localScale = Vector3.one * orbitRadiusWorld;

            if (Application.isPlaying && planetOld.OrbitObject > 0)
            {
                orbitIndicatorGameObject.GetComponent<MeshRenderer>().material.SetColor(Emmision, planetOld.Color);            
                orbitIndicatorGameObject.GetComponent<MeshRenderer>().material.SetFloat(Width, 0.003f);            
                //orbitIndicatorGameObject.GetComponent<MeshRenderer>().material.SetInt(FixedSize, 0);            
            }
            return go;
        }


        private const float SIZE_MULTIPLIER = 10f;
        public static float AUToWorld(float orbitRadiusAU)
        {
            return orbitRadiusAU * SIZE_MULTIPLIER;
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
        public static Vector3 GetPlanetPosition(float semiMajorAxisAU, float semiMinorAxisAU, float angle, float offsetXAt0 = 0, float offsetAngle = 0)
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

        public static Vector3 GetPlanetPosition(float orbitRadiusAU, float angle) =>
            GetPlanetPosition(orbitRadiusAU, orbitRadiusAU, angle);



        const float EARTH_RADIUS_TO_AU = 4.26352E-5f;
        private const float OLD_SCALE = 0.005f;
        public static Vector3 GetRealPlanetSize(float size) => Vector3.one * (AUToWorld(size) * EARTH_RADIUS_TO_AU * 10);

        public static Vector3 GetIconPlanetSize(float size) =>
            Vector3.one * (2-1/(1+size));
            
        
        public PlanetOld[] GetPlanets()
        {
            return CreateSystem();
        }

        public string[] GetNames() => prefab.planetScriptables.Select(p => p.Name).ToArray();
        public long[] GetPopulations() => prefab.planetScriptables.Select(p => p.Population).ToArray();
        public float[] GetPopulationLevels() => prefab.planetScriptables.Select(p => Util.LongToLevel(p.Population)).ToArray();
    }
}
