using System.Net;
using Bserg.Controller.Core;
using Bserg.Model.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public class UIController 
    {
        // Controllers
        public UIPlanetController UIPlanetController;
        public UITimeController UITimeController;
        public WorldLabelUI WorldLabelUI;
        
        public UIController(Core.Controller controller, UIDocument uiDocument)
        {
            // Sub controllers
            UIPlanetController = new UIPlanetController(uiDocument, controller.Game.PlanetLevels, controller.Game.BuildSystem);
            UITimeController = new UITimeController(controller.TickController, uiDocument.rootVisualElement.Q<VisualElement>("time-view"));
            WorldLabelUI = new WorldLabelUI(Resources.Load<GameObject>("View/Custom/Labels/PlanetLabel"), GameObject.Find("PlanetLabels").transform);

            uiDocument.rootVisualElement.Q<VisualElement>("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));
        }


        public void OnUpdate(Game game, OrbitController orbitController, float dt)
        {
            Vector3[] planetPositions = new Vector3[game.N];
            for (int i = 0; i < game.N; i++)
                planetPositions[i] = orbitController.GetPlanetPositionAtTickF(game, i, game.Ticks + dt);
            WorldLabelUI.DrawLabelsOnPlanets(game.PlanetNames, planetPositions);
        }

        public void OnTick(Game game)
        {
            UITimeController.UpdateGameTime(game.Ticks);
        }
        

        
    }
}
