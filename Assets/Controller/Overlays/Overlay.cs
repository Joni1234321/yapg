using Bserg.Model.Core;

namespace Bserg.Controller.Overlays
{
    /// <summary>
    /// Interface that is used to handle logic for when different menus are activated, such as trading view. Then click on planet means destination and not show planet
    /// </summary>
    public abstract class Overlay
    {
        public abstract void Enable(Game game);
        public abstract void Disable();

        // Call one of these three each update
        public virtual void OnTick(Game game, int hoverPlanetID, int selectedPlanetID) {}
        public virtual void UpdateSelected(Game game, int selectedPlanetID) { }

        public virtual void UpdateHover(Game game, int hoverPlanetID)
        {
        }

        public virtual void UpdateHoverAndSelected(Game game, int hoverPlanetID, int selectedPlanetID)
        {
        }

        public virtual void UpdateFocusNone()
        {
        }


        public virtual void PlanetHoverEnter(Game game, int hoverPlanetID, int selectedPlanetID)
        {
        }

        public virtual void PlanetHoverExit(Game game, int oldHoverPlanetID, int selectedPlanetID)
        {
        }

        public virtual void PlanetSelectedEnter(Game game, int selectedPlanetID)
        {
        }

        public virtual void PlanetSelectedExit(Game game, int oldSelectedPlanetID)
        {
        }
    }
}
