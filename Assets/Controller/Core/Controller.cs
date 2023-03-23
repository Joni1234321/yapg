using System.Collections.Generic;
using Bserg.Controller.Components;
using Bserg.Controller.Drivers;
using Bserg.Controller.Overlays;
using Bserg.Controller.Sensors;
using Bserg.Controller.UI;
using Bserg.Controller.UI.Planet;
using Bserg.Controller.World;
using Bserg.Model.Core;
using Bserg.Model.Political;
using Bserg.Model.Space;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Bserg.Controller.Core
{
    public partial class Controller : MonoBehaviour
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
        
        public MouseController MouseController;

        public SpaceFlightRenderer SpaceFlightRenderer;

        public PlanetRenderer PlanetRenderer;
        public WorldSensor WorldSensor;
        public UIDocument uiDocument;


        private List<int> allPlanets, outerPlanets;

        public SystemGeneratorUpdated SystemGeneratorUpdated;
        public Material planetMaterial, orbitMaterial, circleMaterial;
        public Mesh planetMesh, orbitMesh;
        private EntityQuery gameTicksFQuery;
        private EntityQuery cameraQuery;
        
        void Awake()
        {
            SelectionHelper.SelectedPlanetID = -1;
            SelectionHelper.HoverPlanetID = -1;
            
            PlanetOld[] planets = systemGenerator.GetPlanets();
            string[] names = systemGenerator.GetNames();
            float[] populationLevels = systemGenerator.GetPopulationLevels();

            PoliticalBody[] bodies = new PoliticalBody[planets.Length];

                
            Game = new Game(names, populationLevels, bodies, planets, systemGenerator.Orbits);

            EntityManager entityManager = Unity.Entities.World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemGeneratorUpdated = new SystemGeneratorUpdated(entityManager, planetMaterial, planetMesh, orbitMaterial, circleMaterial, orbitMesh);

            entityManager.CreateSingleton(new GameTicksF() { });
            entityManager.CreateSingleton(new Global.CameraComponent());
            gameTicksFQuery = entityManager.CreateEntityQuery(typeof(GameTicksF));
            cameraQuery = entityManager.CreateEntityQuery(typeof(Global.CameraComponent));


            MouseController = new MouseController();
            
            PlanetRenderer = new PlanetRenderer(Game.Planets, Game.OrbitalTransferSystem, systemGenerator);
            
            SpaceFlightRenderer = new SpaceFlightRenderer(Game.Planets, Game.SpaceflightSystem.Spaceflights, PlanetRenderer);

            CameraRenderer = new CameraRenderer(PlanetRenderer);
            CameraDriver = new CameraDriver(CameraRenderer);
            
            allPlanets = new();
            outerPlanets = new();
            for (int i = 0; i < planets.Length; i++)
            {
                allPlanets.Add(i);
                if (i == 0 || (i > 4 && i < 10))
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
            WorldSensor.PlanetRenderer.SetVisiblePlanets(CameraRenderer.Camera.orthographicSize < 80f ? allPlanets : outerPlanets);

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
            // TODO: Move this to system
            gameTicksFQuery.GetSingletonRW<GameTicksF>().ValueRW.TicksF = Game.Ticks + dt;
            gameTicksFQuery.GetSingletonRW<GameTicksF>().ValueRW.DeltaTick = dt;
            cameraQuery.GetSingletonRW<Global.CameraComponent>().ValueRW.Size = CameraRenderer.Camera.orthographicSize;
                
            HandleInput();
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
            
            
        }


        private void UpdateRenderers(int ticks, float dt)
        {
            CameraRenderer.OnUpdate(ticks, dt);

            if (systemGenerator.Orbits.Get(CameraRenderer.FocusPlanetID, out OrbitData orbitData))
            {
                List<int> visibleIds = new(orbitData.Children.Count + 1);
                for (int i = 0; i < orbitData.Children.Count; i++)
                    visibleIds.Add(orbitData.Children[i].PlanetID);
                visibleIds.Add(orbitData.PlanetID);
                // Add all its orbiting planets and so on
                while (Game.Planets[visibleIds[^1]].OrbitObject != -1)
                    visibleIds.Add(Game.Planets[visibleIds[^1]].OrbitObject); 
                
                //CameraRenderer.Camera.orthographicSize < 40f ? allPlanets : outerPlanets
                PlanetRenderer.SetVisiblePlanets(CameraRenderer.Camera.orthographicSize < 40f ? visibleIds : outerPlanets);
                PlanetRenderer.OnUpdate(ticks, dt);
            }
            SpaceFlightRenderer.OnUpdate(ticks, dt);
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