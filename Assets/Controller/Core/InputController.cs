using UnityEngine;

namespace Bserg.Controller.Core
{
    public class InputController
    {
        public void OnUpdate (Controller controller)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                controller.TickController.ToggleGameRunning();
            if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus))
                controller.TickController.IncreaseGameSpeed(1);
            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
                controller.TickController.IncreaseGameSpeed(-1);
        }
    }
}