using Bserg.Controller.Components;
using Bserg.Controller.Sensors;
using Bserg.Model.Units;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.UIElements;

namespace Bserg.Controller.Drivers
{
    /// <summary>
    /// Contains data about the game in between ticks
    /// </summary>
    public class TimeDriver
    {
        public readonly TimeSensor Sensor;
        public static readonly float[] TICK_TIME = { 100f / GameTick.TICKS_PER_MONTH, 10f / GameTick.TICKS_PER_MONTH, 5f / GameTick.TICKS_PER_MONTH, 2f / GameTick.TICKS_PER_MONTH, 1f / GameTick.TICKS_PER_MONTH, .2f / GameTick.TICKS_PER_MONTH, .001f, .0001f };

        public TimeDriver (TimeSensor sensor)
        {
            Sensor = sensor;
            Sensor.UI.GameSpeedButton.RegisterCallback<ClickEvent>(_ => ToggleGameRunning());
        }
        
        EntityQuery gameSpeedQuery = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GameSpeed));

        public void ToggleGameRunning()
        {
            RefRW<GameSpeed> gameSeed = gameSpeedQuery.GetSingletonRW<GameSpeed>();
            gameSeed.ValueRW.Running = !gameSeed.ValueRO.Running;
            Sensor.OnGameSpeedChange();
            
        }
        public void IncreaseGameSpeed(int increase)
        {
            RefRW<GameSpeed> gameSpeedRW = gameSpeedQuery.GetSingletonRW<GameSpeed>();
            gameSpeedRW.ValueRW.Speed = math.clamp(gameSpeedRW.ValueRO.Speed + increase, 0, TICK_TIME.Length - 1);
            Sensor.OnGameSpeedChange();
        }
        
        
    }
}