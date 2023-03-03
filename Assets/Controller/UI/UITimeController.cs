using Bserg.Controller.Core;
using Bserg.Model.Units;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    /// <summary>
    /// Controls the ui for time
    /// </summary>
    public class UITimeController
    {
        // View 
        private VisualElement gameSpeedPart;
        private Button gameSpeedButton;
        private Label gameTimeLabel;

        public UITimeController(TickController tickController, VisualElement timeUI)
        {
            // View
            gameTimeLabel = timeUI.Q<Label>("time");
            gameSpeedButton = timeUI.Q<Button>("speed");
            gameSpeedPart = timeUI.Q<VisualElement>("part");

            gameSpeedButton.RegisterCallback<ClickEvent>(e => tickController.ToggleGameRunning());
        }

        
        /// <summary>
        /// Update the ui showing the speed of the game
        /// </summary>
        /// <param name="running"></param>
        /// <param name="speed"></param>
        public void UpdateGameSpeed(bool running, int speed)
        {
            if (!running)
                gameSpeedButton.text = "---- II ----";
            else
                gameSpeedButton.text = new string('/', speed + 1);
        }

        private static readonly string[] MonthToText = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

        /// <summary>
        /// Update time UI to include date
        /// </summary>
        /// <param name="time">Tick time</param>
        public void UpdateGameTime(int time)
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