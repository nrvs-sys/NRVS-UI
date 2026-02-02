using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class ScrollViewFade : MonoBehaviour
{
	[Header("Settings")]
	public Edge edge = Edge.Top;
	public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);


	public enum Edge 
	{
		Top, 
		Bottom,
	}


	private CanvasGroup canvasGroup;
	private ScrollRect scrollRect;
	private RectTransform content, viewport;

	private const float edgeRange = 0.10f;


	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		scrollRect = GetComponentInParent<ScrollRect>();

		content = scrollRect.content;
		viewport = scrollRect.viewport;
	}


	public void SetFade(float value)
	{
		if (canvasGroup != null)
		{
			if (!IsScrollable())
			{
				canvasGroup.alpha = 0f;
				return;
			}


			float t = edge == Edge.Top
			? Mathf.InverseLerp(1f, 1f - edgeRange, value)
			: Mathf.InverseLerp(0f, edgeRange, value);

			canvasGroup.alpha = fadeCurve.Evaluate(t);
		}
	}

	public void SetFade(Vector2 value) => SetFade(value.y);


	bool IsScrollable() => content.rect.height - viewport.rect.height > 0.5f;
}