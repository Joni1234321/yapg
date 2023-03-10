using System.Collections.Generic;
using System.Linq;
using Bserg.Controller.Core;
using Bserg.Model.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public class UIController
    {
        public Camera Camera;
        // Controllers
        public UIPlanetController UIPlanetController;
        public UITimeController UITimeController;
        public WorldUI WorldUI;
        
        private List<Model.Space.Planet> allPlanets, outerPlanets;

        public UIController(Core.Controller controller, UIDocument uiDocument)
        {
            Camera = Camera.main;
            // Sub controllers
            UIPlanetController = new UIPlanetController(uiDocument, controller.Game);
            UITimeController = new UITimeController(controller.TickController, uiDocument.rootVisualElement.Q<VisualElement>("time-view"));
            WorldUI = new WorldUI(controller.PlanetController, controller.CameraController, GameObject.Find("PlanetLabels").transform);

            uiDocument.rootVisualElement.Q<VisualElement>("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));

            allPlanets = controller.Game.Planets.ToList();
            outerPlanets = new List<Model.Space.Planet>();
            for (int i = 0; i < allPlanets.Count; i++)
            {
                if (i > 0 && i < 5)
                    continue;
                outerPlanets.Add(allPlanets[i]);
            }
        }


        public void OnUpdate(Game game, PlanetController planetController, float dt)
        {
            bool showInner = Camera.orthographicSize < 40f;
            List<Model.Space.Planet> planets = showInner ? allPlanets : outerPlanets;
            Vector3[] planetPositions = new Vector3[planets.Count];
            planetPositions[0] = planetController.GetPlanetPositionAtTickF(game, 0, game.Ticks + dt);
            for (int i = 1; i < planetPositions.Length; i++)
                planetPositions[i] = planetController.GetPlanetPositionAtTickF(game, i + (showInner ? 0 : 4), game.Ticks + dt);
            
            
            WorldUI.DrawUI(planetPositions, planets);
        }

        
        public void OnTick(Game game)
        {
            UITimeController.UpdateGameTime(game.Ticks);
        }
        

        
    }
}
