using Bserg.Controller.Overlays;

namespace Bserg.Controller.Core
{
    /// <summary>
    /// Contain information about the currently selected / hover planet
    /// </summary>
    public static class SelectionHelper
    {
        public static Overlay ActiveOverlay;
        
        private static int hoverPlanetID = -1, selectedPlanetID = -1;

        public static bool HoverPlanetValid { get; private set; } = false;
        public static bool SelectedPlanetValid { get; private set; } = false;

        public static int HoverPlanetID
        {
            get => hoverPlanetID;
            set
            {
                if (hoverPlanetID == value)
                    return;
                
                hoverPlanetID = value;
                HoverPlanetValid = value != -1;
            }
        }

        public static int SelectedPlanetID
        {
            get => selectedPlanetID;
            set
            {
                if (selectedPlanetID == value)
                    return;

                selectedPlanetID = value; 
                SelectedPlanetValid = value != -1;
                
                if (SelectedPlanetValid)
                    OnNewValidSelect?.Invoke(SelectedPlanetID);
            }
        }
        
        public delegate void OnClick(int planetID);
        public static event OnClick OnNewValidSelect;
    }
}