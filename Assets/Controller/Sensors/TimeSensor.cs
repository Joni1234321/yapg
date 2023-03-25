using Bserg.Controller.Components;
using Bserg.Controller.UI.Planet;
using Bserg.Model.Core;
using Bserg.Model.Units;
using Unity.Entities;

namespace Bserg.Controller.Sensors
{
    /// <summary>
    /// Controls the ui for time
    /// </summary>
    public class TimeSensor : GameSensor
    {
        public TimeUI UI;        
        private Game game;
        EntityQuery gameSpeedQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameSpeed));

        public TimeSensor(Game game, TimeUI timeUI)
        {
            UI = timeUI;
            this.game = game;
        }

        public override void OnTick()
        {
            UI.DrawGameTime(game.Ticks);
        }

        public void OnGameSpeedChange()
        {
            GameSpeed gameSpeed = gameSpeedQuery.GetSingleton<GameSpeed>();

            UI.DrawGameSpeed(gameSpeed.Running, gameSpeed.Speed);
        }
        
        public static readonly float[] TICK_TIME = { 100f / GameTick.TICKS_PER_MONTH, 10f / GameTick.TICKS_PER_MONTH, 5f / GameTick.TICKS_PER_MONTH, 2f / GameTick.TICKS_PER_MONTH, 1f / GameTick.TICKS_PER_MONTH, .2f / GameTick.TICKS_PER_MONTH, .001f, .0001f };

    }
}