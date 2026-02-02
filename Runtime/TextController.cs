using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class TextController : MonoBehaviour
    {
        [SerializeField]
        TextElement element;

        TMPro.TMP_Text textMesh;

        void Awake()
        {
            textMesh = GetComponent<TMPro.TMP_Text>();

            if (element != null)
                element.RegisterController(this);
        }

        void OnDestroy()
        {
            if (element != null)
                element.UnregisterController(this);
        }

        public void SetText(string text)
        {
            textMesh.text = text;
        }

        public void SetFontSize(float fontSize)
        {
            textMesh.fontSize = fontSize;
        }

        public void SetColor(Color color)
        {
            textMesh.color = color;
        }

        public void SetAlpha(float alpha)
        {
            textMesh.color = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, alpha);
        }

        public void Fade(float duration, float startAlpha, float endAlpha)
        {
            gameObject.SetActive(true);

            StartCoroutine(FadeRoutine(duration, startAlpha, endAlpha));
        }

        public void FadeIn(float duration)
        {
            Fade(duration, 0, 1);
        }

        public void FadeOut(float duration)
        {
            Fade(duration, 1, 0);
        }

        IEnumerator FadeRoutine(float duration, float startAlpha, float targetAlpha)
        {
            float time = 0;

            while (time < duration)
            {
                time += Time.deltaTime;

                float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

                SetAlpha(alpha);

                yield return null;
            }

            SetAlpha(targetAlpha);

            if (targetAlpha == 0)
                gameObject.SetActive(false);
        }
    }
}
