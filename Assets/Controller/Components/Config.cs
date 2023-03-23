using TMPro;
using Unity.Entities;

namespace Bserg.Controller.Components
{
    public struct Config : IComponentData
    {
        public Entity PlanetPrefab;
        public Entity PlanetUIPrefab;
    }

    public class TextMeshProUGUI
    {
        public TMPro.TextMeshProUGUI Value;
    } 

}