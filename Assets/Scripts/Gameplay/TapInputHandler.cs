using Tapverse.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Tapverse.Gameplay
{
    public class TapInputHandler : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            ServiceLocator.Resolve<CurrencyManager>().RegisterTap();
        }
    }
}
