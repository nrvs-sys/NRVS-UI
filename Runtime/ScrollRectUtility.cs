using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectUtility : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private bool isVertical = true;

	[SerializeField, Tooltip("Speed to scroll when using ScrollUp and ScrollDown methods")] 
	private float scrollSpeed = 1f;

	[Header("Components")]
	[Tooltip("Top button for vertical mode, left for horizontal")]
	public Button button1;
	[Tooltip("Bottom button for vertical mode, right for horizontal")]
	public Button button2;


	private ScrollRect scrollRect;
	private float currentScrollSpeed = 0;


	void Start()
	{
		scrollRect = GetComponent<ScrollRect>();

		LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRect.content);

		UpdateButtonInteractableStates();

		scrollRect.onValueChanged.AddListener(_ => UpdateButtonInteractableStates());
	}

	private void Update()
	{
		Scroll(currentScrollSpeed);
	}

	public void Scroll(float amount)
	{
		if (amount == 0)
			return;


		if (isVertical)
		{
			Vector2 newPosition = scrollRect.content.anchoredPosition + new Vector2(0, amount * Time.deltaTime);
			newPosition.y = Mathf.Clamp(newPosition.y, 0, scrollRect.content.rect.height - scrollRect.viewport.rect.height);
			scrollRect.content.anchoredPosition = newPosition;
		}
		else
		{
			Vector2 newPosition = scrollRect.content.anchoredPosition + new Vector2(amount * Time.deltaTime, 0);
			newPosition.x = Mathf.Clamp(newPosition.x, 0, scrollRect.content.rect.width - scrollRect.viewport.rect.width);
			scrollRect.content.anchoredPosition = newPosition;
		}

		UpdateButtonInteractableStates();
	}

	private void UpdateButtonInteractableStates()
	{
		const float tolerance = 0.001f;

		if (isVertical)
		{
			button1.interactable = scrollRect.verticalNormalizedPosition < 1f - tolerance;
			button2.interactable = scrollRect.verticalNormalizedPosition > tolerance;
		}
		else
		{
			button1.interactable = scrollRect.horizontalNormalizedPosition > tolerance;
			button2.interactable = scrollRect.horizontalNormalizedPosition < 1f - tolerance;
		}
	}

	public void ScrollUp() => Scroll(scrollSpeed);
	public void ScrollDown() => Scroll(-scrollSpeed);
	public void ScrollUpActivate() => currentScrollSpeed = scrollSpeed;
	public void ScrollDownActivate() => currentScrollSpeed = -scrollSpeed;
	public void ScrollDeactivate() => currentScrollSpeed = 0;
}