using UnityEngine;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Manages input, and calls their hotkeys
    /// </summary>
    public class InputController
    {
        public void OnUpdate (Controller controller)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                controller.TimeDriver.ToggleGameRunning();
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                controller.TimeDriver.IncreaseGameSpeed(1);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                controller.TimeDriver.IncreaseGameSpeed(-1);
            
            if (Input.GetKeyDown(KeyCode.E))
                controller.CameraController.EnterPlanetView();
            if (Input.GetKeyDown(KeyCode.Q))
                controller.CameraController.EnterSolarSystemView();
        }
    }
}