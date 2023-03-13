using Bserg.Model.Units;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI.Planet
{
    public class TimeUI : UIClass
    {
        // View 
        private readonly VisualElement gameSpeedPart;
        public readonly Button GameSpeedButton;
        private readonly Label gameTimeLabel;
        
        public TimeUI(VisualElement root) : base(root, true)
        {
            // View
            gameTimeLabel = root.Q<Label>("time");
            GameSpeedButton = root.Q<Button>("speed");
            gameSpeedPart = root.Q<VisualElement>("part");
        }
        
        
        /// <summary>
        /// Update the ui showing the speed of the game
        /// </summary>
        /// <param name="running"></param>
        /// <param name="speed"></param>
        public void DrawGameSpeed(bool running, int speed)
        {
            if (!running)
                GameSpeedButton.text = "---- II ----";
            else
                GameSpeedButton.text = new string('/', speed + 1);
        }
        
        private static readonly string[] MonthToText = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};
        
        /// <summary>
        /// Update time UI to include date
        /// </summary>
        /// <param name="time">Tick time</param>
        public void DrawGameTime(int time)
        {
            const int DAYS_IN_MONTH = 30;
            float days = DAYS_IN_MONTH * (float)(time % GameTick.TICKS_PER_MONTH) / GameTick.TICKS_PER_MONTH;
            int month = (time / GameTick.TICKS_PER_MONTH) % 12;
            int year = 2200 + time / GameTick.TICKS_PER_YEAR;
            gameTimeLabel.text = $"{days:00} {MonthToText[month]} {year}";
            gameSpeedPart.style.height = days * 16f / DAYS_IN_MONTH;
        }

    }
}