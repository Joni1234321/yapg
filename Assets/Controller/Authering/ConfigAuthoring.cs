using Bserg.Controller.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Bserg.Controller.Authering
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject PlanetPrefab, PlanetUIPrefab;

    }

    public class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity planetPrefab = GetEntity(authoring.PlanetPrefab);

            AddComponent(new Config
            {
                PlanetPrefab = planetPrefab,
                PlanetUIPrefab = GetEntity(authoring.PlanetUIPrefab),
            });
            
            
        }
    }
    
}