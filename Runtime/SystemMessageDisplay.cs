using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(CanvasGroup))]
public class SystemMessageDisplay : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float messageDuration = 2f;
	[SerializeField] private float fadeDuration = 0.5f;
	[SerializeField] private float initialDisplayDelay = 0.5f;

	[Header("Events")]
	public UnityEvent onMessageConsume;

	private TextMeshProUGUI messageText;
	private CanvasGroup canvasGroup;

	private void Awake()
	{
		messageText = GetComponent<TextMeshProUGUI>();
		canvasGroup = GetComponent<CanvasGroup>();
	}

	private void OnEnable()
	{
		StartCoroutine(DisplayMessagesCoroutine());
	}

	private IEnumerator DisplayMessagesCoroutine()
	{
		yield return new WaitForSeconds(initialDisplayDelay);

		while (gameObject.activeInHierarchy)
		{
			if (SystemMessage.TryConsume(out var message))
			{
				messageText.text = message.messageText;

				onMessageConsume?.Invoke();

				yield return FadeIn();
				yield return new WaitForSeconds(messageDuration);
				yield return FadeOut();
			}
			else
			{
				yield return null; // Wait for the next frame
			}
		}
	}

	private IEnumerator FadeIn()
	{
		float currentTime = 0f;
		while (currentTime < fadeDuration)
		{
			currentTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
			yield return null;
		}
		canvasGroup.alpha = 1f; // Ensure it's fully visible
	}

	private IEnumerator FadeOut()
	{
		float currentTime = 0f;
		while (currentTime < fadeDuration)
		{
			currentTime += Time.deltaTime;
			canvasGroup.alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
			yield return null;
		}
		canvasGroup.alpha = 0f; // Ensure it's fully invisible
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		messageText.text = default;
		canvasGroup.alpha = 1f; // Reset the visibility
	}
}