using Bserg.Model.Space.Components;
using UnityEngine;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Manages input, and calls their hotkeys
    /// </summary>
    public partial class Controller
    {
        public void HandleInput ()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                TimeDriver.ToggleGameRunning();
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                TimeDriver.IncreaseGameSpeed(1);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                TimeDriver.IncreaseGameSpeed(-1);
            
            if (Input.GetKeyDown(KeyCode.E))
                CameraDriver.EnterPlanetView();
            if (Input.GetKeyDown(KeyCode.Q))
                CameraDriver.EnterSolarSystemView();

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SelectionHelper.SelectedPlanetID = (Game.Planets.Length + SelectionHelper.SelectedPlanetID - 1) % Game.Planets.Length;
                CameraDriver.EnterPlanetView();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SelectionHelper.SelectedPlanetID = (Game.Planets.Length + SelectionHelper.SelectedPlanetID + 1) % Game.Planets.Length;
                CameraDriver.EnterPlanetView();
            }
            
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
                CameraRenderer.TargetSize *= 1.2f; 
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
                CameraRenderer.TargetSize *= .80f;

            if (Input.GetKeyDown(KeyCode.H))
                    CreateSpacecraft();


            //SpaceFlightRenderer.SetActive(Input.GetKey(KeyCode.Tab));

            if (Input.GetKeyDown(KeyCode.Tab))
                ToggleSpacecraftsView();
        }


        private void CreateSpacecraft()
        {
            
            bool result = Spacecraft.CreateSpacecraft(Game.EntityManager,
                new Spacecraft.FlightStep
                {
                    DestinationPlanet = Game.Entities[3],
                    Action = Spacecraft.ActionType.Load
                },
                new Spacecraft.FlightStep
                {
                    DestinationPlanet = Game.Entities[4],
                    Action = Spacecraft.ActionType.Unload
                },
                10000
            );
            Debug.Log($"Spacecraft {result}");
        }
    }
}