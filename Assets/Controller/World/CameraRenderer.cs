using UnityEngine;

namespace Bserg.Controller.World
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
        
        public float TargetSize = 10;
        private readonly float closestZoom = 0.0002f, farthestZoom = 400f;
        Vector3 vel;

        
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
            UpdateZoom();
            Vector3 planetPosition = planetRenderer.GetPlanetPositionAtTickF(FocusPlanetID, ticks + dt);
            
            UpdatePosition(planetPosition);
        }
        
        
        /// <summary>
        /// Sets the order for where the camera has to go next
        /// </summary>
        /// <param name="planetID"></param>
        /// <param name="size"></param>
        public void ChangeFocus(int planetID, float size)
        {
            animationTime = 0;
            FocusPlanetID = planetID;
            TargetSize = size;
        }
        
        /// <summary>
        /// Returns the next value
        /// </summary>
        /// <param name="current"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private float NextSmoothStep(float current, float target)
        {
            const float SPEED = 15, MAX_DIFF = 1f;
            float percent = current / target;
            float deltaPercent = 1 - percent; // + - percent

            if (Mathf.Abs(deltaPercent) < 0.01f) // dont zoom if as close as 1 %
                return target;
            float smooth = SPEED * Time.deltaTime * Mathf.Clamp(deltaPercent, -MAX_DIFF, MAX_DIFF);
            return current * (1 + smooth);
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
        
        /// <summary>
        /// Updates the zoom of the object
        /// </summary>
        private void UpdateZoom()
        {

            float currentSize = Camera.orthographicSize;
            TargetSize = Mathf.Clamp(TargetSize, closestZoom, farthestZoom);
            
            Camera.orthographicSize = Mathf.Clamp(NextSmoothStep(currentSize, TargetSize), closestZoom, farthestZoom);
        }


    }
}