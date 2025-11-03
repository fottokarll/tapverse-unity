using TapVerse.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TapVerse.Gameplay
{
    public class TapInputHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            ServiceLocator.Resolve<CurrencyManager>().RegisterTap();
        }
    }
}
