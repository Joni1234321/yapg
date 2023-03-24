using Bserg.Controller.Components;
using TMPro;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Bserg.Controller.Authoring
{
    public class ConfigAuthoring : MonoBehaviour
    {
        public GameObject PlanetPrefab, PlanetUIPrefab;
        public GameObject TextMeshProPrefab;
        public GameObject TestLinkedPrefab;

    }

    public class ConfigBaker : Baker<ConfigAuthoring>
    {
        public override void Bake(ConfigAuthoring authoring)
        {
            Entity e = GetEntity(TransformUsageFlags.None);
            Entity planetPrefab = GetEntity(authoring.PlanetPrefab, TransformUsageFlags.Dynamic);

            TextMeshPro tmp = authoring.TextMeshProPrefab.GetComponent<TextMeshPro>();
            
            Entity entityText = GetEntity(authoring.TextMeshProPrefab, TransformUsageFlags.Dynamic);
            
            AddComponent(e, new Config
            {
                PlanetPrefab = planetPrefab,
                PlanetUIPrefab = GetEntity(authoring.PlanetUIPrefab, TransformUsageFlags.Dynamic),
                TextMeshProPrefab = entityText,
                TestLinkedPrefab = GetEntity(authoring.TestLinkedPrefab, TransformUsageFlags.Dynamic)
            });
            
            World.DefaultGameObjectInjectionWorld.EntityManager.AddComponentObject(entityText, new UIManaged
            {
                Text = tmp,
            });

        }
    }

    public class UIManaged : IComponentData
    {
        public TextMeshPro Text;
    }

    [RequireMatchingQueriesForUpdate]
    public partial class TestSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            Config config = SystemAPI.GetSingleton<Config>();
            Entity e = EntityManager.Instantiate(config.TextMeshProPrefab);
            //EntityManager.Instantiate(config.TestLinkedPrefab);
            Entity cool = EntityManager.Instantiate(config.TestLinkedPrefab);
            //EntityManager.RemoveComponent<Parent>(EntityManager.GetBuffer<Child>(cool)[0].Value);
            //EntityManager.RemoveComponent<Child>(cool);
            Entities.ForEach((TextMeshPro tmp) =>
            {
                tmp.text = "FIX ME !!!!!!!!!!!!!!!!!!!";
            }).WithoutBurst().Run();        
        }

        protected override void OnUpdate()
        {

        }
    } 
}