using Bserg.Controller.Sensors;
using Bserg.Model.Core.Operators;
using Bserg.Model.Space;
using UnityEngine;
using UnityEngine.UIElements;

namespace Bserg.Controller.Drivers
{
    public class BuildDriver
    {
        public readonly BuildSensor Sensor;
        public readonly BuildOperator Operator;
        private readonly PlanetLevels planetLevels;

        public BuildDriver(BuildOperator buildOperator, BuildSensor buildSensor, PlanetLevels planetLevels)
        {
            Operator = buildOperator;
            Sensor = buildSensor;
            this.planetLevels = planetLevels;            
            
            buildSensor.UI.Upgrade.RegisterCallback<ClickEvent>(_ => Upgrade());
            buildSensor.UI.Downgrade.RegisterCallback<ClickEvent>(_ => Downgrade());
        }
        
        /// <summary>
        /// Upgrades the level count on the current recipe
        /// </summary>
        public void Upgrade()
        {
            if (!Operator.Upgrade(Sensor.CurrentRecipe, 1, Sensor.CurrentPlanetID))
                Debug.Log("No Upgrade");
            Sensor.RedrawBuildLevels();
        }

        /// <summary>
        /// Downgrades the current level count on the current recipe
        /// </summary>
        public void Downgrade()
        {
            if (!Operator.Downgrade(Sensor.CurrentRecipe, 1, Sensor.CurrentPlanetID))
                Debug.Log("No Downgrade");
            Sensor.RedrawBuildLevels();
        }
        
        

        
        


    }
}