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

            SpaceFlightRenderer.SetActive(Input.GetKey(KeyCode.Tab));

        }
    }
}