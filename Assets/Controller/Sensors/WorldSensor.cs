using Bserg.Controller.UI;
using Bserg.Controller.World;
using Bserg.Model.Core;
using UnityEngine.UIElements;

namespace Bserg.Controller.Sensors
{
    public class WorldSensor : GameSensor
    {
        // Renders
        public PlanetRenderer PlanetRenderer;
        
        public WorldSensor(PlanetRenderer planetRenderer)
        {
            PlanetRenderer = planetRenderer;
        }

        public override void OnTick()
        {
        }
    }
}
