using System.Collections.Generic;
using Bserg.Controller.Components;
using Bserg.Controller.Drivers;
using Bserg.Controller.Overlays;
using Bserg.Controller.Sensors;
using Bserg.Controller.UI;
using Bserg.Controller.UI.Planet;
using Bserg.Controller.VisualEntities;
using Bserg.Controller.WorldRenderer;
using Bserg.Model.Core;
using Bserg.Model.Political;
using Bserg.Model.Space;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Core
{
    public partial class Controller : MonoBehaviour
    {

        public const int UI_LAYER = 5, CLICKABLE_LAYER = 7;
        public SystemGenerator systemGenerator;
        public Game Game;

        public CameraDriver CameraDriver;

        // Overlays
        private Overlay activeOverlay;
        public NormalOverlay NormalOverlay;
        public TradeOverlay TradeOverlay;

        public UIPlanetController UIPlanetController;
        
        public TimeSensor TimeSensor;
        public TimeDriver TimeDriver;
        
        public MouseController MouseController;

        public PlanetUIDrawer PlanetUIDrawer;

        public WorldSensor WorldSensor;
        public UIDocument uiDocument;


        private List<int> allPlanets, outerPlanets;

        public SystemGeneratorUpdated SystemGeneratorUpdated;
        public Material planetMaterial, orbitMaterial, circleMaterial, spacecraftOrbitMaterial, spacecraftMaterial;
        public Mesh planetMesh, orbitMesh;
        
        private EntityQuery gameTicksFQuery, gameSpeedQuery;
        private EntityQuery cameraQuery;
        private EntityQuery planetTransformsQuery;

        private EntityManager entityManager;
        void Awake()
        {
            SelectionHelper.SelectedPlanetID = -1;
            SelectionHelper.HoverPlanetID = -1;
            
            PlanetOld[] planets = systemGenerator.GetPlanets();
            string[] names = systemGenerator.GetNames();
            float[] populationLevels = systemGenerator.GetPopulationLevels();

            PoliticalBody[] bodies = new PoliticalBody[planets.Length];

                
            Game = new Game(names, populationLevels, bodies, planets, systemGenerator.Orbits);

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            SystemGeneratorUpdated = new SystemGeneratorUpdated(entityManager, 
                planetMaterial, planetMesh, orbitMaterial, circleMaterial, 
                orbitMesh, spacecraftOrbitMaterial, spacecraftMaterial);


            MouseController = new MouseController();
            
            PlanetUIDrawer = new PlanetUIDrawer();

            CameraDriver = new CameraDriver();
            
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
            WorldSensor = new WorldSensor();
            TimeSensor = new TimeSensor(Game, new TimeUI(uiDocument.rootVisualElement.Q<VisualElement>("time-view")));
            TimeDriver = new TimeDriver(TimeSensor);
            UIPlanetController = new UIPlanetController(uiDocument, Game);

            // Set earth as planet
            UIPlanetController.SetFocusedPlanet(3);
            
            // Overlays
            NormalOverlay = new NormalOverlay(UIPlanetController);
            TradeOverlay = new TradeOverlay(Game, this, UIPlanetController);
            SetActiveOverlay(NormalOverlay);


            gameTicksFQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameTicksF));
            gameSpeedQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameSpeed));
            cameraQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(Global.CameraAnimation));
            planetTransformsQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(
                typeof(LocalToWorld), typeof(PlanetVisual.Model));

            
        }


        private bool firstTickAfterPause = true;
        private void Update()
        {
            if (Game.DoTick())
            {
                activeOverlay.OnTick(Game, SelectionHelper.HoverPlanetID, SelectionHelper.SelectedPlanetID);
                WorldSensor.OnTick();
                TimeSensor.OnTick();
            }
            
            float dt = gameTicksFQuery.GetSingleton<GameTicksF>().DeltaTick;
            
            HandleInput();
            MouseController.OnUpdate(Game, activeOverlay, dt);

            UpdateRenderers(Game.Ticks, dt);

            // No need to update if paused, unless one time
            if (!gameSpeedQuery.GetSingleton<GameSpeed>().Running)
            {
                if (!firstTickAfterPause)
                    return;
                
                firstTickAfterPause = false;
            }
            else
                firstTickAfterPause = true;
            
            
        }


        private bool showSpacecrafts = true;

        public void ToggleSpacecraftsView()
        {
            showSpacecrafts = !showSpacecrafts;
            if (!showSpacecrafts)
                SystemGeneratorUpdated.ShipPool.DisableAll(entityManager);
        }

        private void UpdateRenderers(int ticks, float dt)
        {
            var localToWorlds = planetTransformsQuery.ToComponentDataArray<LocalToWorld>(Allocator.Temp);
            float3[] planetPositions = new float3[localToWorlds.Length];

            for (int i = 0; i < localToWorlds.Length; i++)
                planetPositions[i] = localToWorlds[i].Value.Translation();

            PlanetUIDrawer.Draw(planetPositions, Game.Planets, allPlanets);

/*
            if (systemGenerator.Orbits.Get(CameraDriver.FocusedPlanetQuery.GetSingleton<Global.FocusedPlanet>().Value, out OrbitData orbitData))
            {
                List<int> visibleIds = new(orbitData.Children.Count + 1);
                for (int i = 0; i < orbitData.Children.Count; i++)
                    visibleIds.Add(orbitData.Children[i].PlanetID);
                visibleIds.Add(orbitData.PlanetID);
                // Add all its orbiting planets and so on
                while (Game.Planets[visibleIds[^1]].OrbitObject != -1)
                    visibleIds.Add(Game.Planets[visibleIds[^1]].OrbitObject); 
                
                //CameraRenderer.Camera.orthographicSize < 40f ? allPlanets : outerPlanets
                PlanetRenderer.SetVisiblePlanets(Camera.main.orthographicSize < 40f ? visibleIds : outerPlanets);
            }*/
            if (showSpacecrafts)
                SystemGeneratorUpdated.UpdateShips(entityManager);
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