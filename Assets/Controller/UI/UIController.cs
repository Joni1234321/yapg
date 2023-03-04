using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public class UIController 
    {
        // Controllers
        public UIPlanetController UIPlanetController;
        public UITimeController UITimeController;
        
        public UIController(Core.Controller controller, UIDocument uiDocument)
        {
            // Sub controllers
            UIPlanetController = new UIPlanetController(uiDocument, controller.Game.PlanetLevels);
            UITimeController = new UITimeController(controller.TickController, uiDocument.rootVisualElement.Q<VisualElement>("time-view"));

            uiDocument.rootVisualElement.Q<VisualElement>("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));
        }

        
    }
}
