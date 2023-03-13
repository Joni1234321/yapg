using System.Collections.Generic;
using Bserg.Controller.Drivers;
using Bserg.Controller.Overlays;
using Bserg.Controller.Sensors;
using Bserg.Controller.UI;
using Bserg.Controller.UI.Planet;
using Bserg.Controller.World;
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

        public CameraRenderer CameraRenderer;
        public CameraDriver CameraDriver;

        // Overlays
        private Overlay activeOverlay;
        public NormalOverlay NormalOverlay;
        public TradeOverlay TradeOverlay;

        public UIPlanetController UIPlanetController;
        
        public TimeSensor TimeSensor;
        public TimeDriver TimeDriver;
        
        public InputController InputController;
        public MouseController MouseController;

        public SpaceflightController SpaceflightController;

        public PlanetRenderer PlanetRenderer;
        public WorldSensor WorldSensor;
        public UIDocument uiDocument;


        private List<int> allPlanets, outerPlanets;

        void Awake()
        {
            SelectionHelper.SelectedPlanetID = -1;
            SelectionHelper.HoverPlanetID = -1;
            
            Planet[] planets = systemGenerator.GetPlanets();
            string[] names = systemGenerator.GetNames();
            float[] populationLevels = systemGenerator.GetPopulationLevels();

            PoliticalBody[] bodies = new PoliticalBody[planets.Length];

                
            Game = new Game(names, populationLevels, bodies, planets);

            
            InputController = new InputController();
            MouseController = new MouseController();
            
            PlanetRenderer = new PlanetRenderer(Game.Planets, Game.OrbitalTransferSystem, systemGenerator);
            
            SpaceflightController = new SpaceflightController(PlanetRenderer);

            CameraRenderer = new CameraRenderer(PlanetRenderer);
            CameraDriver = new CameraDriver(CameraRenderer);
            
            allPlanets = new();
            outerPlanets = new();
            for (int i = 0; i < planets.Length; i++)
            {
                allPlanets.Add(i);
                if (i == 0 || i > 4)
                    outerPlanets.Add(i);
            }
        }

        void Start()
        {
            // UI
            WorldSensor = new WorldSensor(PlanetRenderer);
            TimeSensor = new TimeSensor(Game, new TimeUI(uiDocument.rootVisualElement.Q<VisualElement>("time-view")));
            TimeDriver = new TimeDriver(TimeSensor);
            UIPlanetController = new UIPlanetController(uiDocument, Game);

            // Set earth as planet
            UIPlanetController.SetFocusedPlanet(3);
            
            // Overlays
            NormalOverlay = new NormalOverlay(UIPlanetController, PlanetRenderer);
            TradeOverlay = new TradeOverlay(Game, this, UIPlanetController);
            SetActiveOverlay(NormalOverlay);
            WorldSensor.PlanetRenderer.SetVisiblePlanets(CameraRenderer.Camera.orthographicSize < 40f ? allPlanets : outerPlanets);

        }


        private bool firstTickAfterPause = true;
        private void Update()
        {

            if (TimeDriver.Update(Game))
            {
                activeOverlay.OnTick(Game, SelectionHelper.HoverPlanetID, SelectionHelper.SelectedPlanetID);
                WorldSensor.OnTick();
                TimeSensor.OnTick();
            }
            
            float dt = TimeDriver.DeltaTick;

            InputController.OnUpdate(this);
            MouseController.OnUpdate(Game, activeOverlay, dt);

            UpdateRenderers(Game.Ticks, dt);

            // No need to update if paused, unless one time
            if (!TimeDriver.Running)
            {
                if (!firstTickAfterPause)
                    return;
                
                firstTickAfterPause = false;
            }
            else
                firstTickAfterPause = true;
            
            SpaceflightController.Update(Game, dt);
            
        }


        private void UpdateRenderers(int ticks, float dt)
        {
            WorldSensor.PlanetRenderer.SetVisiblePlanets(CameraRenderer.Camera.orthographicSize < 40f ? allPlanets : outerPlanets);
            WorldSensor.PlanetRenderer.OnUpdate(ticks, dt);
            PlanetRenderer.OnUpdate(ticks, dt);
            CameraRenderer.OnUpdate(ticks, dt);
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