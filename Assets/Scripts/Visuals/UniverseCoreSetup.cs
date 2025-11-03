using TapVerse.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TapVerse.Visuals
{
    [RequireComponent(typeof(Image))]
    public class UniverseCoreSetup : MonoBehaviour
    {
        private void Start()
        {
            var image = GetComponent<Image>();
            image.sprite = ProceduralSprites.GetUniverseCoreSprite();
            image.preserveAspect = true;

            if (!image.raycastTarget)
            {
                image.raycastTarget = true;
            }

            EnsureTapInputHandler();
        }

        private void EnsureTapInputHandler()
        {
            if (!TryGetComponent<TapInputHandler>(out _))
            {
                gameObject.AddComponent<TapInputHandler>();
            }
        }
    }
}
