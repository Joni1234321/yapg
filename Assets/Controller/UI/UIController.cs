using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public class UIController : MonoBehaviour
    {
        // Controllers
        public Core.Controller controller;
        public UIPlanetController UIPlanetController;
        public UITimeController UITimeController;
        
        // View
        public UIDocument ui;
        void Start()
        {
            // Sub controllers
            UIPlanetController = new UIPlanetController(controller, GetUI("planet-view"), GetUI("build-view"),
                GetUI("level-view"), GetUI("transfer-view"), GetUI("migration-view"));
            UITimeController = new UITimeController(controller.TickController, GetUI("time-view"));

            GetUI("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));
        }

        public VisualElement GetUI(string name) => ui.rootVisualElement.Q<VisualElement>(name); 

        public void OnTick()
        {
        }
    }
}
