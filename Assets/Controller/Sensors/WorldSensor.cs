using Bserg.Controller.UI;
using Bserg.Controller.World;
using Bserg.Model.Core;
using UnityEngine.UIElements;

namespace Bserg.Controller.Sensors
{
    public class UIWorldSensor : GameSensor
    {
        // Renders
        public PlanetRenderer PlanetRenderer;

        private Game game;
        
        public UIWorldSensor(Core.Controller controller, UIDocument uiDocument)
        {
            uiDocument.rootVisualElement.Q<VisualElement>("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));

            PlanetRenderer = new PlanetRenderer(controller.Game.Planets, controller.Game.OrbitalTransferSystem, controller.PlanetHelper);
            game = controller.Game;
        }

        public override void OnTick()
        {
        }
    }
}
