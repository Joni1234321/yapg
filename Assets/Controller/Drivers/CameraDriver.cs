using Bserg.Controller.Components;
using Bserg.Controller.Core;
using Bserg.Controller.VisualEntities;
using Bserg.Controller.WorldRenderer;
using Unity.Collections;
using Unity.Entities;

namespace Bserg.Controller.Drivers
{
    public class CameraDriver
    {
        
        EntityQuery cameraQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Global.CameraAnimation));
        public EntityQuery FocusedPlanetQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Global.FocusedPlanet));
        EntityQuery planetQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(PlanetVisual.Model));

        
        public CameraDriver()
        {
        } 
        
        public void EnterPlanetView()
        {
            if (!SelectionHelper.SelectedPlanetValid)
                return;

            
            ChangeFocus(SelectionHelper.SelectedPlanetID, 0.1f);
        }

        public void EnterSolarSystemView()
        {
            ChangeFocus(0, 10);
        }

        void ChangeFocus(int planetID, float size)
        {
            ref Global.CameraAnimation animation = ref cameraQuery.GetSingletonRW<Global.CameraAnimation>().ValueRW;
            FocusedPlanetQuery.SetSingleton(new Global.FocusedPlanet { Value = planetQuery.ToEntityArray(Allocator.Temp)[planetID] });
            
            animation.TargetSize = size;

        }
    }
}