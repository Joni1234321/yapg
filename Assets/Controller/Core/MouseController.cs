using System;
using Bserg.Controller.Overlays;
using Bserg.Model.Core;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    public class MouseController
    {
        public Camera Camera;
        public int HoverPlanetID = -1;
        public int SelectedPlanetID = -1;

        public MouseController()
        {
            Camera = Camera.main;
        }


        void CheckIfHoverOrSelectChange(Game game, Overlay activeOverlay)
        {
            int newHoverPlanetID = GetHoverPlanetID(game);
            int oldHoverPlanetID = HoverPlanetID;
            int oldSelectedPlanetID = SelectedPlanetID;
            bool oldHoverValid = HoverPlanetID != -1;
            bool newHoverValid = newHoverPlanetID != -1;
            bool oldSelectedValid = SelectedPlanetID != -1;


            // Hover change
            if (newHoverPlanetID != HoverPlanetID)
            {
                HoverPlanetID = newHoverPlanetID;
                if (oldHoverValid)
                    activeOverlay.PlanetHoverExit(game, oldHoverPlanetID, SelectedPlanetID);

                if (newHoverValid)
                    activeOverlay.PlanetHoverEnter(game, newHoverPlanetID, SelectedPlanetID);
            }

            // Selected Change
            if (Input.GetMouseButtonDown(0) && newHoverPlanetID != SelectedPlanetID)
            {
                SelectedPlanetID = newHoverPlanetID;

                if (oldSelectedValid)
                    activeOverlay.PlanetSelectedExit(game, oldSelectedPlanetID);

                if (newHoverValid)
                    activeOverlay.PlanetSelectedEnter(game, newHoverPlanetID);
            }
        }

        /// <summary>
        /// Update based on the specific state
        /// </summary>
        /// <param name="game"></param>
        /// <param name="activeOverlay"></param>
        void UpdateFocus(Game game, Overlay activeOverlay)
        {
            bool selectedValid = SelectedPlanetID != -1;
            bool hoverValid = HoverPlanetID != -1;
            // Hover
            if (hoverValid)
            {
                if (selectedValid) activeOverlay.UpdateHoverAndSelected(game, HoverPlanetID, SelectedPlanetID);
                else activeOverlay.UpdateHover(game, HoverPlanetID);
            }
            // No hover then use old selected Planet
            else
            {
                if (selectedValid) activeOverlay.UpdateSelected(game, SelectedPlanetID);
                else activeOverlay.UpdateFocusNone();
            }
        }
        
        /// <summary>
        /// Place hover effect over planet, and if clicked then select
        /// </summary>
        public void Update(Game game, Overlay activeOverlay)
        {
            // Deselect
            if (Input.GetMouseButtonDown(1))
            {
                SelectedPlanetID = -1;
                activeOverlay.UpdateFocusNone();
            }

            CheckIfHoverOrSelectChange(game, activeOverlay);
            UpdateFocus(game, activeOverlay);
            UpdateZoom();
        }

        private float closestZoom = 0.3f, farthestZoom = 400f, targetCameraSize = 10;
        void UpdateZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                targetCameraSize *= 1.1f; 
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                targetCameraSize *= .90f;

            float sizeDiff = targetCameraSize - Camera.orthographicSize;
            if (Mathf.Abs(sizeDiff) > 0.1f)
            {
                float smooth = 10 * Time.deltaTime * Mathf.Clamp(Mathf.Abs(sizeDiff), .05f, 20f) * Mathf.Sign(sizeDiff);
                Camera.orthographicSize = Mathf.Clamp(Camera.orthographicSize + smooth, closestZoom, farthestZoom);
            }
        }
        
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Returns first planet that is within hover distance of mouse
        /// </summary>
        /// <returns>Planet hovering over or null if none found</returns>
        private int GetHoverPlanetID(Game game)
        {
            int layerMask = 1 << Controller.CLICKABLE_LAYER;
            
            if (Physics.Raycast(Camera.ScreenPointToRay (Input.mousePosition), out RaycastHit hit, Mathf.Infinity, layerMask))
                return hit.collider.GetComponent<PlanetIDScript>().planetID;
            
            return -1;
        }
    }
}