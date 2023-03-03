using UnityEngine;
using UnityEngine.Serialization;
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
        public UIDocument uiDocument;
        void Awake()
        {
            // Sub controllers
            UIPlanetController = new UIPlanetController(uiDocument);
            UITimeController = new UITimeController(controller.TickController, uiDocument.rootVisualElement.Q<VisualElement>("time-view"));

            uiDocument.rootVisualElement.Q<VisualElement>("trade-menu").Q<VisualElement>("button-settle")
                .RegisterCallback<ClickEvent>(_ => controller.SetActiveOverlay(controller.TradeOverlay));
        }


        public void OnTick()
        {
        }
    }
}
