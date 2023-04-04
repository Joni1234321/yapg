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
            /*
            ref GameSpeed gameSpeed = ref gameSpeedQuery.GetSingletonRW<GameSpeed>().ValueRW;
            gameSpeed.Running = !gameSpeed.Running;
            UISensor.UI.DrawGameSpeed(gameSpeed.Running, gameSpeed.Speed);*/
        }
        public void IncreaseGameSpeed(int increase)
        {/*
           ref GameSpeed gameSpeed = ref gameSpeedQuery.GetSingletonRW<GameSpeed>().ValueRW;
           gameSpeed.Speed = math.clamp(gameSpeed.Speed + increase, 0, TICK_TIME.Length - 1);
           UISensor.UI.DrawGameSpeed(gameSpeed.Running, gameSpeed.Speed);*/
        }
        
        
    }
}