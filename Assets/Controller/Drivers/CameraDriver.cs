using Bserg.Controller.Core;
using Bserg.Controller.World;

namespace Bserg.Controller.Drivers
{
    public class CameraDriver
    {
        private CameraRenderer cameraRenderer;
        
        public CameraDriver(CameraRenderer renderer)
        {
            cameraRenderer = renderer;
        } 
        
        public void EnterPlanetView()
        {
            if (!SelectionHelper.SelectedPlanetValid)
                return;

            cameraRenderer.ChangeFocus(SelectionHelper.SelectedPlanetID, 0.1f);
        }

        public void EnterSolarSystemView()
        {
            cameraRenderer.ChangeFocus(0, 10);
        }
    }
}