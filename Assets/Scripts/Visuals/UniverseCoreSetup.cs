using UnityEngine;
using UnityEngine.UI;

namespace Tapverse.Visuals
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

        }
    }
}
