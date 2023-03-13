using UnityEngine;
using UnityEngine.EventSystems;

namespace Bserg.Controller.Core
{
    public class PlanetIDScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public int planetID;
        public static int UISelectedID = -1;
        public static int UIHoverID = -1;

        public void OnPointerClick(PointerEventData eventData)
        {
            UISelectedID = planetID;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UIHoverID = planetID;
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            UIHoverID = -1;
        }



    }
}
