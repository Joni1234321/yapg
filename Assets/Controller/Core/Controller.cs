using Bserg.Controller.Overlays;
using Bserg.Controller.UI;
using Bserg.Model.Core;
using Bserg.Model.Political;
using Bserg.Model.Space;
using Bserg.Model.Units;
using Bserg.View.Space;
using UnityEngine;

namespace Bserg.Controller.Core
{
    public class Controller : MonoBehaviour
    {

        public const int CLICKABLE_LAYER = 7;
        public SystemGenerator systemGenerator;
        public Game Game;

        // Overlays
        private Overlay activeOverlay;
        public NormalOverlay NormalOverlay;
        public TradeOverlay TradeOverlay;

        public TickController TickController;
        
        public InputController InputController;
        public MouseController MouseController;

        public OrbitController OrbitController;
        public SpaceflightController SpaceflightController;
        
        
        public UIController UIController;

        
        void Awake()
        {
            Planet[] planets = systemGenerator.GetPlanets();
            string[] names = systemGenerator.GetNames();
            long[] populations = systemGenerator.GetPopulations();
            PoliticalBody[] bodies = new PoliticalBody[planets.Length];

                
            Game = new Game(names, populations, bodies, planets);

            TickController = new TickController(this);
            
            InputController = new InputController();
            MouseController = new MouseController();

            OrbitController = new OrbitController(systemGenerator);
            SpaceflightController = new SpaceflightController(OrbitController);
            
            // Overlays
            NormalOverlay = new NormalOverlay(UIController, OrbitController);
            TradeOverlay = new TradeOverlay(Game, this, UIController);
            SetActiveOverlay(NormalOverlay);
        }


        private void Update()
        {
            if (TickController.Update(Game))
            {
                activeOverlay.OnTick(Game, MouseController.HoverPlanetID, MouseController.SelectedPlanetID);
                UIController.UITimeController.UpdateGameTime(Game.Ticks);
            }
            
            float dt = TickController.GetDT();

            InputController.Update(this);
            MouseController.Update(Game, activeOverlay);
            
            // No need to update if paused
            if (!TickController.Running)
                return;
            
            OrbitController.Update(Game, dt);
            SpaceflightController.Update(Game, dt);
            
        }
        




        public void OnGameSpeedChange()
        {
            UIController.UITimeController.UpdateGameSpeed(TickController.Running, TickController.GameSpeed);
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