using Bserg.Model.Core;
using UnityEngine;

namespace Bserg.Controller.Core
{
    public class CameraController
    {
        public static Camera Camera;
        private Transform transform;

        private int focusPlanetID;
        
        public CameraController()
        {
            Camera = Camera.main;
            transform = Camera.transform;
        }
        
        
        private float closestZoom = 0.002f, farthestZoom = 400f, targetSize = 10;

        private void UpdateZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                targetSize *= 1.2f; 
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                targetSize *= .80f;

            float currentSize = Camera.orthographicSize;
            targetSize = Mathf.Clamp(targetSize, closestZoom, farthestZoom);
            
            Camera.orthographicSize = Mathf.Clamp(NextSmoothStep(currentSize, targetSize), closestZoom, farthestZoom);
            
        }
        Vector3 vel;

        private void UpdatePosition(Vector3 target)
        {
            Vector3 current = transform.position;
            target = new Vector3(target.x, target.y, -100);
            if (animationTime < .3f)
            {
                transform.position = Vector3.SmoothDamp(current, target, ref vel, .3f);
            }
            else
            {
                transform.position = target;
            }
            
        }


        /// <summary>
        /// Returns the next value
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        float NextSmoothStep(float current, float target)
        {
            const float SPEED = 15, MAX_DIFF = 1f;
            float percent = current / target;
            float deltaPercent = 1 - percent; // + - percent

            if (Mathf.Abs(deltaPercent) < 0.01f) // dont zoom if as close as 1 %
                return target;
            float smooth = SPEED * Time.deltaTime * Mathf.Clamp(deltaPercent, -MAX_DIFF, MAX_DIFF);
            return current * (1 + smooth);
        }

        private float animationTime = 0;
        public void EnterPlanetView()
        {
            if (!SelectionController.SelectedPlanetValid)
                return;
            animationTime = 0;
            focusPlanetID = SelectionController.SelectedPlanetID;
            targetSize = 0.01f;
        }

        public void EnterSolarSystemView()
        {
            animationTime = 0;
            focusPlanetID = 0;
            targetSize = 10;
        }

        public void OnUpdate(Game game, PlanetHelper planetHelper, float dt)
        {
            animationTime += Time.deltaTime;
            UpdateZoom();
            Vector3 planetPosition = planetHelper.GetPlanetPositionAtTickF(game.Planets, game.OrbitalTransferSystem, focusPlanetID, game.Ticks + dt);
            UpdatePosition(planetPosition);
        } 
        
    }
}