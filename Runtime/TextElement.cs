using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "Text Element_ New", menuName = "UI/Text Element")]
    public class TextElement : ManagedObject
    {
        [NonSerialized]
        List<TextController> controllers = new();

        protected override void Initialize()
        {
            controllers.Clear();
        }

        protected override void Cleanup()
        {
            controllers.Clear();
        }

        public void RegisterController(TextController controller)
        {
            if (!controllers.Contains(controller))
                controllers.Add(controller);
        }

        public void UnregisterController(TextController controller)
        {
            if (controllers.Contains(controller))
                controllers.Remove(controller);
        }

        public void SetText(string text)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].SetText(text);
        }

        public void SetFontSize(float fontSize)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].SetFontSize(fontSize);
        }

        public void SetColor(Color color)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].SetColor(color);
        }

        public void SetAlpha(float alpha)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].SetAlpha(alpha);
        }

        public void Fade(float duration, float startAlpha, float endAlpha)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].Fade(duration, startAlpha, endAlpha);
        }

        public void FadeIn(float duration)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].FadeIn(duration);
        }

        public void FadeOut(float duration)
        {
            for (var i = 0; i < controllers.Count; i++)
                controllers[i].FadeOut(duration);
        }

        public TextController[] GetControllers() => controllers.ToArray();
    }
}