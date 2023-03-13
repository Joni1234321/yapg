using Bserg.Controller.Core;
using Bserg.Controller.Sensors;
using Bserg.Controller.UI.Planet;
using Bserg.Model.Core;
using Bserg.Model.Units;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    /// <summary>
    /// Controls the ui for time
    /// </summary>
    public class TimeSensor : GameSensor
    {
        public TimeUI UI;        
        private Game game;

        public TimeSensor(Game game, TimeUI timeUI)
        {
            UI = timeUI;
            this.game = game;
        }

        public override void OnTick()
        {
            UI.DrawGameTime(game.Ticks);
        }

        public void OnGameSpeedChange(bool running, int gameSpeed)
        {
            UI.DrawGameSpeed(running, gameSpeed);
        }
    }
}