using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public abstract class UIClass
    {
        public VisualElement UI;

        protected UIClass(VisualElement ui)
        {
            UI = ui;
            userInterfaces.Add(this);
        }
        
        /// <summary>
        /// Stores a reference to all interfaces
        /// </summary>
        private static List<UIClass> userInterfaces = new();


        /// <summary>
        /// Called whenever planet changes to a new one
        /// </summary>
        /// <param name="planetID"></param>
        public static void SetSelectedPlanet(int planetID)
        {
            int n = userInterfaces.Count;
            for (int i = 0; i < n; i++)
                userInterfaces[i].OnNewSelectedPlanet(planetID);
        }

        /// <summary>
        /// Called whenever no planet is selected
        /// </summary>
        public static void ClearSelectedPlanet()
        {
            int n = userInterfaces.Count;
            for (int i = 0; i < n; i++)
                userInterfaces[i].OnDeselectPlanet();
        }
        
        protected virtual void OnNewSelectedPlanet(int planetID)
        {
        }

        protected virtual void OnDeselectPlanet()
        {
        }


        private static bool visible = true;
        /// <summary>
        /// Hides all UIS
        /// </summary>
        public static void HideAll()
        {
            if (!visible)
                return;

            visible = false;
            
            int n = userInterfaces.Count;
            for (int i = 0; i < n; i++)
                userInterfaces[i].UI.style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Shows all uis
        /// </summary>
        public static void ShowAll()
        {
            if (visible)
                return;

            visible = true;

            int n = userInterfaces.Count;
            for (int i = 0; i < n; i++)
                userInterfaces[i].UI.style.display = DisplayStyle.Flex;
        }
        

    }
}