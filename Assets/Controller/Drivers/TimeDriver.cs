using Bserg.Controller.Components;
using Bserg.Controller.Sensors;
using Bserg.Model.Core;
using Bserg.Model.Units;
using Unity.Entities;
using UnityEngine.UIElements;

namespace Bserg.Controller.Drivers
{
    /// <summary>
    /// Contains data about the game in between ticks
    /// </summary>
    public class TimeDriver
    {
        public readonly TimeSensor Sensor;
        
        public float DeltaTick { get; private set; }
        public int GameSpeed { private set; get; }
        public bool Running { private set; get; }
        
        public static readonly float[] TICK_TIME = { 100f / GameTick.TICKS_PER_MONTH, 10f / GameTick.TICKS_PER_MONTH, 5f / GameTick.TICKS_PER_MONTH, 2f / GameTick.TICKS_PER_MONTH, 1f / GameTick.TICKS_PER_MONTH, .2f / GameTick.TICKS_PER_MONTH, .001f, .0001f };


        public TimeDriver (TimeSensor sensor)
        {
            Sensor = sensor;
            Sensor.UI.GameSpeedButton.RegisterCallback<ClickEvent>(_ => ToggleGameRunning());
        }
        
        EntityQuery gameSpeedQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameSpeed));

        public void ToggleGameRunning()
        {
            Running = !Running;
            gameSpeedQuery.GetSingletonRW<GameSpeed>().ValueRW.Running = Running;
            Sensor.OnGameSpeedChange(Running, GameSpeed);
        }
        public void IncreaseGameSpeed(int deltaSpeed)
        {
            GameSpeed = UnityEngine.Mathf.Clamp(GameSpeed + deltaSpeed, 0, TICK_TIME.Length - 1);
            gameSpeedQuery.GetSingletonRW<GameSpeed>().ValueRW.Speed = GameSpeed;

            Sensor.OnGameSpeedChange(Running, GameSpeed);
        }
        
        
        /// <summary>
        /// Returns true if did tick
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public bool Update(Game game)
        {
            if (!Running)
                return false;
            
            DeltaTick += UnityEngine.Time.deltaTime / TICK_TIME[GameSpeed];
            if (DeltaTick >= 1)
            {
                DeltaTick = 0;
                game.DoTick();
                return true;
            }

            return false;
        }
    }
}