﻿using Bserg.Controller.WorldRenderer;

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
