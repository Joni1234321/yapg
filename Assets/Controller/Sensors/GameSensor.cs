using Bserg.Controller.UI;
using UnityEngine.UIElements;

namespace Bserg.Controller.Sensors
{
    /// <summary>
    /// Reads information from systems, and calls the UI function when the data changes
    /// Contains an OnTick that gets called every tick, (since that is when data often is changed)
    /// </summary>
    public abstract class GameSensor : UIClass
    {
        public abstract void OnTick();

        protected GameSensor() : base(new VisualElement())
        {
        }
    }
}