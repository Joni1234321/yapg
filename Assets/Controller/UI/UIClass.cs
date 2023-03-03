using UnityEngine.UIElements;

namespace Bserg.Controller.UI
{
    public abstract class UIClass
    {
        public VisualElement UI;

        protected UIClass(VisualElement ui)
        {
            UI = ui;
        }

    }
}