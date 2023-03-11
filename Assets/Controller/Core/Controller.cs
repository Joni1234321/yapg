using Bserg.Controller.Overlays;
using Bserg.Controller.Sensors;
using Bserg.Controller.UI;
using Bserg.Model.Core;
using Bserg.Model.Political;
using Bserg.Model.Space;
using Bserg.View.Space;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Core
{
    public class Controller : MonoBehaviour
    {

        public const int UI_LAYER = 5, CLICKABLE_LAYER = 7;
        public SystemGenerator systemGenerator;
        public Game Game;
        
        public CameraController CameraController;

        // Overlays
        private Overlay activeOverlay;
        public NormalOverlay NormalOverlay;
        public TradeOverlay TradeOverlay;

        public TickController TickController;
        
        public InputController InputController;
        public MouseController MouseController;

        public PlanetController PlanetController;
        public SpaceflightController SpaceflightController;

        public UIWorldReadonlyDriver UIWorldReadonlyDriver;
        public UIDocument uiDocument;

        
        void Awake()
        {
            SelectionController.SelectedPlanetID = -1;
            SelectionController.HoverPlanetID = -1;
            
            Planet[] planets = systemGenerator.GetPlanets();
            string[] names = systemGenerator.GetNames();
            float[] populationLevels = systemGenerator.GetPopulationLevels();

            PoliticalBody[] bodies = new PoliticalBody[planets.Length];

                
            Game = new Game(names, populationLevels, bodies, planets);

            CameraController = new CameraController();
            
            TickController = new TickController(this);
            
            InputController = new InputController();
            MouseController = new MouseController(CameraController);

            PlanetController = new PlanetController(systemGenerator);
            SpaceflightController = new SpaceflightController(PlanetController);
            
        }

        void Start()
        {
            // UI
            UIWorldReadonlyDriver = new UIWorldReadonlyDriver(this, uiDocument);
            // Set earth as planet
            UIWorldReadonlyDriver.UIPlanetController.SetPlanet(3);
            
            // Overlays
            NormalOverlay = new NormalOverlay(UIWorldReadonlyDriver, PlanetController);
            TradeOverlay = new TradeOverlay(Game, this, UIWorldReadonlyDriver);
            SetActiveOverlay(NormalOverlay);

        }


        private bool firstTickAfterPause = true;
        private void Update()
        {

            if (TickController.Update(Game))
            {
                activeOverlay.OnTick(Game, SelectionController.HoverPlanetID, SelectionController.SelectedPlanetID);
                UIWorldReadonlyDriver.OnTick(Game);
            }
            
            float dt = TickController.DeltaTick;

            InputController.OnUpdate(this);
            MouseController.OnUpdate(Game, activeOverlay, dt);
            CameraController.OnUpdate(Game, PlanetController, dt);

            // UI LAST
            UIWorldReadonlyDriver.OnUpdate(Game, PlanetController, dt);


            // No need to update if paused, unless one time
            if (!TickController.Running)
            {
                if (!firstTickAfterPause)
                    return;
                
                firstTickAfterPause = false;
            }
            else
                firstTickAfterPause = true;
            
            PlanetController.Update(Game, dt);
            SpaceflightController.Update(Game, dt);
            
        }
        




        public void OnGameSpeedChange()
        {
            UIWorldReadonlyDriver.UITimeController.UpdateGameSpeed(TickController.Running, TickController.GameSpeed);
        }


        /// <summary>
        /// Activates the new overlay, and deactivates the old
        /// </summary>
        /// <param name="overlay"></param>
        public void SetActiveOverlay(Overlay overlay)
        {
            activeOverlay?.Disable();
            overlay.Enable(Game);
            activeOverlay = overlay;
        }
        
      
        
        
        
    }
}