using System.Collections.Generic;
using Bserg.Controller.Overlays;
using Bserg.Model.Core;
using Bserg.View.Space;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Handles mouse input
    /// </summary>
    public class MouseController
    {
        private CameraController cameraController;
        UIDocument uiDocument;


        public MouseController(CameraController cameraController)
        {
            this.cameraController = cameraController;
            uiDocument = GameObject.Find("UIDocument").GetComponent<UIDocument>();
        }


        void CheckIfHoverOrSelectChange(Game game, Overlay activeOverlay)
        {
            int newHoverPlanetID = GetHoverPlanetID(game);
            int oldHoverPlanetID = SelectionController.HoverPlanetID;
            int oldSelectedPlanetID = SelectionController.SelectedPlanetID;
            bool oldHoverValid = SelectionController.HoverPlanetID > -1;
            bool newHoverValid = newHoverPlanetID > -1;
            bool oldSelectedValid = SelectionController.SelectedPlanetID > -1;


            // Hover change
            if (newHoverPlanetID != SelectionController.HoverPlanetID)
            {
                SelectionController.HoverPlanetID = newHoverPlanetID;
                if (oldHoverValid)
                    activeOverlay.PlanetHoverExit(game, oldHoverPlanetID, SelectionController.SelectedPlanetID);

                if (newHoverValid)
                    activeOverlay.PlanetHoverEnter(game, newHoverPlanetID, SelectionController.SelectedPlanetID);
            }


            // Selected Change
            if (WorldMouseButtonDown(0) && newHoverPlanetID != SelectionController.SelectedPlanetID)
            {
                SelectionController.SelectedPlanetID = newHoverPlanetID;

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
            // Hover
            if (SelectionController.HoverPlanetValid)
            {
                if (SelectionController.SelectedPlanetValid) activeOverlay.UpdateHoverAndSelected(game, SelectionController.HoverPlanetID, SelectionController.SelectedPlanetID);
                else activeOverlay.UpdateHover(game, SelectionController.HoverPlanetID);
            }
            // No hover then use old selected Planet
            else
            {
                if (SelectionController.SelectedPlanetValid) activeOverlay.UpdateSelected(game, SelectionController.SelectedPlanetID);
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
                activeOverlay.PlanetSelectedExit(game, SelectionController.SelectedPlanetID);
                SelectionController.SelectedPlanetID = -1;
            }

            CheckIfHoverOrSelectChange(game, activeOverlay);
            UpdateFocus(game, activeOverlay);
        }

        
        
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Returns first planet that is within hover distance of mouse
        /// </summary>
        /// <returns>Planet hovering over, -1 if none found</returns>
        private int GetHoverPlanetID(Game game)
        {
            int layerMask = (1 << Controller.CLICKABLE_LAYER) | (1 << Controller.UI_LAYER);

            if (Physics.Raycast(cameraController.Camera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity,
                    layerMask))
            {
                if (hit.collider.gameObject.layer == Controller.UI_LAYER)
                    Debug.Log("Hit UI");
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