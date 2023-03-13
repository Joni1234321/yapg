using System.Collections.Generic;
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

        public UIPlanetController UIPlanetController;
        public UITimeController UITimeController;

        
        public TickController TickController;
        
        public InputController InputController;
        public MouseController MouseController;

        public PlanetHelper PlanetHelper;
        public SpaceflightController SpaceflightController;

        public UIWorldSensor UIWorldSensor;
        public UIDocument uiDocument;


        private List<int> allPlanets, outerPlanets;

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
            MouseController = new MouseController();

            PlanetHelper = new PlanetHelper(systemGenerator);
            SpaceflightController = new SpaceflightController(PlanetHelper);
            
            
            allPlanets = new();
            outerPlanets = new();
            for (int i = 0; i < planets.Length; i++)
            {
                allPlanets.Add(i);
                if (i == 0 || i > 4)
                    outerPlanets.Add(i);
            }
            UIWorldSensor.PlanetRenderer.SetVisiblePlanets(CameraController.Camera.orthographicSize < 40f ? allPlanets : outerPlanets);
        }

        void Start()
        {
            // UI
            UIWorldSensor = new UIWorldSensor(this, uiDocument);
            UIPlanetController = new UIPlanetController(uiDocument, Game);
            UITimeController = new UITimeController(TickController, uiDocument.rootVisualElement.Q<VisualElement>("time-view"));

            // Set earth as planet
            UIWorldSensor.UIPlanetController.SetFocusedPlanet(3);
            
            // Overlays
            NormalOverlay = new NormalOverlay(UIWorldSensor, PlanetHelper);
            TradeOverlay = new TradeOverlay(Game, this, UIWorldSensor);
            SetActiveOverlay(NormalOverlay);

        }


        private bool firstTickAfterPause = true;
        private void Update()
        {

            if (TickController.Update(Game))
            {
                activeOverlay.OnTick(Game, SelectionController.HoverPlanetID, SelectionController.SelectedPlanetID);
                UIWorldSensor.OnTick();
            }
            
            float dt = TickController.DeltaTick;

            InputController.OnUpdate(this);
            MouseController.OnUpdate(Game, activeOverlay, dt);
            CameraController.OnUpdate(Game, PlanetHelper, dt);

            // UI LAST
            UIWorldSensor.PlanetRenderer.SetVisiblePlanets(CameraController.Camera.orthographicSize < 40f ? allPlanets : outerPlanets);
            UIWorldSensor.PlanetRenderer.OnUpdate(Game.Ticks, dt);


            // No need to update if paused, unless one time
            if (!TickController.Running)
            {
                if (!firstTickAfterPause)
                    return;
                
                firstTickAfterPause = false;
            }
            else
                firstTickAfterPause = true;
            
            PlanetHelper.Update(Game, dt);
            SpaceflightController.Update(Game, dt);
            
        }
        




        public void OnGameSpeedChange()
        {
            UIWorldSensor.UITimeController.UpdateGameSpeed(TickController.Running, TickController.GameSpeed);
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