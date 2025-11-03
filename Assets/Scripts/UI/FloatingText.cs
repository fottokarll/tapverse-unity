using System.Collections;
using TapVerse.Core;
using TapVerse.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace TapVerse.UI
{
    [RequireComponent(typeof(Text))]
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private float lifetime = 1f;
        [SerializeField] private Vector3 drift = new Vector3(0f, 60f, 0f);

        private Text _text;
        private Coroutine _routine;

        private void Awake()
        {
            _text = GetComponent<Text>();
        }

        public void Show(string value)
        {
            _text.text = value;
            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            var rect = (RectTransform)transform;
            var start = rect.anchoredPosition;
            var end = start + (Vector2)drift;
            float elapsed = 0f;
            while (elapsed < lifetime)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / lifetime;
                rect.anchoredPosition = Vector2.Lerp(start, end, t);
                _text.canvasRenderer.SetAlpha(1f - t);
                yield return null;
            }

            _text.canvasRenderer.SetAlpha(0f);
            ServiceLocator.Resolve<FeedbackManager>().ReturnFloatingText(gameObject);
        }
    }
}
