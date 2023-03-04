using System.Collections.Generic;
using Bserg.Controller.Overlays;
using Bserg.Model.Core;
using Bserg.View.Space;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Core
{
    public class MouseController
    {
        public Camera Camera;
        UIDocument uiDocument;

        public int HoverPlanetID = -1;
        public int SelectedPlanetID = -1;

        public MouseController()
        {
            Camera = Camera.main;
            uiDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        }


        void CheckIfHoverOrSelectChange(Game game, Overlay activeOverlay)
        {
            int newHoverPlanetID = GetHoverPlanetID(game);
            int oldHoverPlanetID = HoverPlanetID;
            int oldSelectedPlanetID = SelectedPlanetID;
            bool oldHoverValid = HoverPlanetID > -1;
            bool newHoverValid = newHoverPlanetID > -1;
            bool oldSelectedValid = SelectedPlanetID > -1;


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
            if (WorldMouseButtonDown(0) && newHoverPlanetID != SelectedPlanetID)
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
        public void OnUpdate(Game game, Overlay activeOverlay, float dt)
        {
            activeOverlay.DeltaTick = dt;
            // Deselect
            if (WorldMouseButtonDown(1))
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
        /// <returns>Planet hovering over, -1 if none found</returns>
        private int GetHoverPlanetID(Game game)
        {
            int layerMask = (1 << Controller.CLICKABLE_LAYER) ;

            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity,
                    layerMask))
            {
                return hit.collider.GetComponent<PlanetIDScript>().planetID;
            }
            
            return -1;
        }


        /// <summary>
        /// Returns wether the mouse has been hit while not over a ui object
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        bool WorldMouseButtonDown(int button)
        {
            if (Input.GetMouseButtonDown(button))
                return !IsMouseOverUI(Input.mousePosition);

            return false;
        }
        
        /// <summary>
        /// Checks if pointer is over ui, by checking the alpha value over the position of the mouse over the current UI
        /// </summary>
        /// <param name="screenPos"> Mouse Position</param>
        /// <returns></returns>
        bool IsMouseOverUI ( Vector2 screenPos )
        {
            Vector2 pointerUiPos = new Vector2{ x = screenPos.x , y = Screen.height - screenPos.y };
            List<VisualElement> picked = new List<VisualElement>();
            uiDocument.rootVisualElement.panel.PickAll( pointerUiPos , picked );
            foreach( var ve in picked )
                if( ve!=null )
                {
                    Color32 bcol = ve.resolvedStyle.backgroundColor;
                    if( bcol.a!=0 && ve.enabledInHierarchy )
                        return true;
                }
            return false;
        }

    }
}