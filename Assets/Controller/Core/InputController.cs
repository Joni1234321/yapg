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

            
            SpaceFlightRenderer.SetActive(Input.GetKey(KeyCode.Tab));

        }
    }
}