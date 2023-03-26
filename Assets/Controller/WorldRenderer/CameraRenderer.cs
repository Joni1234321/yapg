using Bserg.Controller.Components;
using Bserg.Controller.VisualEntities;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Bserg.Controller.WorldRenderer
{
    /// <summary>
    /// Draws the camera and its position
    /// </summary>
    public class CameraRenderer : WorldRenderer
    {
        // Camera
        public static Camera Camera;
        private readonly Transform transform;

        private readonly PlanetRenderer planetRenderer;

        public int FocusPlanetID { get; private set; }
        private float animationTime;
        
        Vector3 vel;

        EntityQuery cameraQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Global.CameraAnimation));
        EntityQuery planetQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(PlanetVisual.Model));

        //TODO: Remove game from constructor
        public CameraRenderer(PlanetRenderer planetRenderer)
        {
            Camera = Camera.main;
            transform = Camera.transform;

            this.planetRenderer = planetRenderer;
        }
        
        public override void OnUpdate(int ticks, float dt)
        {
            animationTime += Time.deltaTime;
            //UpdateZoom();
            Vector3 planetPosition = planetRenderer.GetLocalPlanetPositionAtTickF(FocusPlanetID, ticks + dt);

            planetPosition.z = -100;
            //transform.position = planetPosition;
            //UpdatePosition(planetPosition);
        }
        
        
        /// <summary>
        /// Sets the order for where the camera has to go next
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="size"></param>
        public void ChangeFocus(int planetID, float size)
        {
            animationTime = 0;
            ref Global.CameraAnimation animation = ref cameraQuery.GetSingletonRW<Global.CameraAnimation>().ValueRW;
            animation.FollowEntity = planetQuery.ToEntityArray(Allocator.Temp)[planetID];
            
            FocusPlanetID = planetID;
            animation.TargetSize = size;
        }
        
        
        /// <summary>
        /// Updates the position of the camera 
        /// </summary>
        /// <param name="target"></param>
        private void UpdatePosition(Vector3 target)
        {
            Vector3 current = transform.position;
            target = new Vector3(target.x, target.y, -100);
            if (animationTime > 0.3f)
            {
                if ( animationTime < .5f)
                    transform.position = Vector3.SmoothDamp(current, target, ref vel, .15f);
                else
                    transform.position = target;
            }
        }
        


    }
}