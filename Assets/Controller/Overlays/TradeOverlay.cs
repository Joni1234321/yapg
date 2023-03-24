using Bserg.Controller.UI;
using Bserg.Model.Core;
using Bserg.Model.Population;
using Bserg.Model.Space;
using Bserg.View.Custom.Counter;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Overlays
{
    /// <summary>
    /// Shows overlay when trade menu is open
    /// </summary>
    public class TradeOverlay : Overlay
    {
        private int n;
        private GameObject[] selectors;
        private MeshRenderer[] renderers;

        private Core.Controller controller;

        private int fromID, toID;
        private Label tradeFromLabel, tradeToLabel;
        private VisualElement tradeConfig, tradeFrom, tradeTo;
        private CounterControl tradeSerial, tradeParallel;

        private bool first = true;
        
        public TradeOverlay(Game game, Core.Controller controller, UIPlanetController uiPlanetController)
        {
            this.controller = controller;
            n = game.N;
                GameObject selectorPrefab = Resources.Load<GameObject>("View/Shaders/Selector/LineSelector");
            Transform selectorParent = new GameObject("SelectorParent").transform;

            selectors = new GameObject[n];
            renderers = new MeshRenderer [n];
            for (int i = 0; i < n; i++)
            {
                selectors[i] = Object.Instantiate(selectorPrefab, selectorParent);
                renderers[i] = selectors[i].GetComponent<MeshRenderer>();
                
                PlanetOld planetOld = game.Planets[i];
                
                selectors[i].transform.position = planetOld.StartingPosition;
                selectors[i].transform.localScale = Vector3.one * Mathf.Log(planetOld.Size * 2 + Mathf.Exp(1));
            }

            tradeConfig = uiPlanetController.GetUI("trade-config");

            tradeFrom = tradeConfig.Q<VisualElement>("from");
            tradeTo = tradeConfig.Q<VisualElement>("to");
            
            tradeFromLabel = tradeFrom.Q<Label>();
            tradeToLabel = tradeTo.Q<Label>();
            
            tradeSerial = tradeConfig.Q<CounterControl>("serial");
            tradeParallel = tradeConfig.Q<CounterControl>("parallel");

            
            Disable();
        }


        public override void Enable(Game game)
        {
            first = true;
            tradeConfig.style.display = DisplayStyle.Flex;

            tradeFromLabel.text = "";
            tradeToLabel.text = "";
            tradeSerial.ValueInt = 1;
            tradeParallel.ValueInt = 1;
            
            for (int i = 0; i < n; i++)
                selectors[i].SetActive(true);
            
            UpdateValidColoring(game);
        }

        public override void Disable()
        {
            tradeConfig.style.display = DisplayStyle.None;

            for (int i = 0; i < n; i++)
                selectors[i].SetActive(false);
        }

        public override void OnTick(Game game, int hoverPlanetID, int selectedPlanetID)
        {
            UpdateValidColoring(game);
        }
        
        public override void PlanetSelectedEnter(Game game, int selectedPlanetID)
        {
            bool valid = first ? game.SettleSystem.ValidDeparture(selectedPlanetID, 1000) : game.SettleSystem.ValidConnection(fromID, selectedPlanetID, 1000);
            if (!valid)
                return;

            if (!first)
            {
                toID = selectedPlanetID;
                tradeToLabel.text = game.PlanetNames[toID];
                // Complete
                for (int i = 0; i < tradeParallel.ValueInt; i++)
                    game.SettleSystem.SettleOrders.Add(new SettleOrder(fromID,toID, tradeSerial.ValueInt));
    
                controller.SetActiveOverlay(controller.NormalOverlay);
                return;
            }
            
            first = !first;
            fromID = selectedPlanetID;
            tradeFromLabel.text = game.PlanetNames[fromID];
            
            UpdateValidColoring(game);
        }
        public override void PlanetSelectedExit(Game game, int selectedPlanetID)
        {
            UpdateValidColoring(game, selectedPlanetID);
        }

        void UpdateValidColoring(Game game)
        {
            for (int planetID = 0; planetID < n; planetID++)
                UpdateValidColoring(game, planetID);
        } 

        // Color Scheme
        Color defaultColor = Color.white, selectedColor = Color.green, hoverColor = Color.yellow;
        Color connectedColor = Color.blue, invalidColor = Color.red;
        private static readonly int Emission = Shader.PropertyToID("_Emission");

        void UpdateValidColoring(Game game, int planetID)
        {
            bool valid = first ? game.SettleSystem.ValidDeparture(planetID, 1000) : game.SettleSystem.ValidConnection(fromID, planetID, 1000);
            Color color = valid ? selectedColor : invalidColor;
            renderers[planetID].material.SetColor(Emission, color);
        } 
        
    }
}