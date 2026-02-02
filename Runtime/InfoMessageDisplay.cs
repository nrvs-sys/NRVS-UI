using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Used to display messages from the <see cref="InfoMessage"></see> system
/// </summary>
public class InfoMessageDisplay : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField] private float fadeDuration = 0.5f;
	[SerializeField] private float initialDisplayDelay = 0.5f;

	[Header("Components")]
	[SerializeField] private TMP_Text messageText;
	[SerializeField] private TMP_Text progressText;
	[SerializeField] private Renderer backgroundRenderer;

	[Header("Events")]
	public UnityEvent onMessageConsumed;


	private const float defaultMessageDuration = 3f;
	private const float progressionCompleteDelay = 1f;

	readonly int baseColorProperty = Shader.PropertyToID("_BaseColor");


    private void OnEnable()
	{
		StartCoroutine(DisplayMessagesCoroutine());
	}

	private void OnDisable()
	{
		StopAllCoroutines();
		ResetTexts();
	}

	private void ResetTexts()
	{
		Color messageColor = messageText.color;
		Color progressColor = progressText.color;
		messageColor.a = 0f;
		progressColor.a = 0f;
		messageText.color = messageColor;
		progressText.color = progressColor;
		messageText.text = string.Empty;
		progressText.text = string.Empty;

		if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
		{
			Color bgColor = backgroundRenderer.material.GetColor(baseColorProperty);
			bgColor.a = 0f;
			backgroundRenderer.material.SetColor(baseColorProperty, bgColor);
		}
	}

	private IEnumerator DisplayMessagesCoroutine()
	{
		yield return WaitForSecondsInterruptible(initialDisplayDelay, null);

		while (gameObject.activeInHierarchy)
		{
			if (InfoMessage.TryConsume(out var message))
			{
				if (message.displayDelay > 0)
				{
					Debug.Log($"Info Message Display: Waiting for {message.displayDelay} seconds before consuming message...");

					yield return new WaitForSeconds(message.displayDelay);
				}

				Debug.Log($"Info Message Display: Consuming message: {message.messageText}");

				onMessageConsumed?.Invoke();

				messageText.text = message.messageText;
				progressText.text = message.totalProgress > 0
					? $"{message.currentProgress}/{message.totalProgress}"
					: string.Empty;

				UpdateBackgroundSize();

				//Debug.LogError($"showing message: {message.messageText}");
				yield return FadeInInterruptible(message);

				// Do message display behavior
				if (message.totalProgress > 0)
				{
					Debug.Log($"Info Message Display: Waiting for message progress to complete...");

					var checkDuration = message.duration > 0;
					var durationTimer = 0f;

					if (checkDuration)
						Debug.Log($"Info Message Display: Message has a duration of {message.duration} seconds to check against progress completion.");

                    // Wait for progress to be complete or until completeImmediately is true
                    while (message.currentProgress < message.totalProgress && !message.completeImmediately)
					{
                        // If there is a duration, increment the timer and check if time is up
						if (checkDuration)
                        {
							durationTimer += Time.deltaTime;
							if (durationTimer >= message.duration)
                            {
								Debug.Log($"Info Message Display: Message duration of {message.duration} seconds reached before progress complete, breaking wait loop...");
								break;
                            }
                        }
                        progressText.text = $"{message.currentProgress}/{message.totalProgress}";
						yield return null;
					}

					// Update progressText to final value
					if (!checkDuration)
						progressText.text = $"{message.totalProgress}/{message.totalProgress}";

					// When complete, wait a moment before progressing unless completeImmediately is true
					if (!message.completeImmediately)
					{
						Debug.Log($"Info Message Display: Waiting for {progressionCompleteDelay} seconds before completing message...");

						yield return WaitForSecondsInterruptible(progressionCompleteDelay, message);
					}

					Debug.Log($"Info Message Display: Message progress complete!");
				}
				else if (message.manualProgression)
				{
					Debug.Log($"Info Message Display: Waiting for message to be ready to progress...");

					// Wait for progression to be ready or until completeImmediately is true
					while (!message.readyToProgress && !message.completeImmediately)
						yield return null;

					// When complete, wait a moment before progressing unless completeImmediately is true
					if (!message.completeImmediately)
					{
						Debug.Log($"Info Message Display: Waiting for {progressionCompleteDelay} seconds before completing message...");

						yield return WaitForSecondsInterruptible(progressionCompleteDelay, message);
					}

					Debug.Log($"Info Message Display: Message progress complete!!");
				}
				else
				{
					// If there is no duration, set to a default duration
					if (message.duration < 0)
						message.duration = defaultMessageDuration;

					if (message.duration > 0)
					{
						Debug.Log($"Info Message Display: Waiting for {message.duration} seconds before completing message...");
						// Wait for the duration of the message
						yield return WaitForSecondsInterruptible(message.duration, message);
					}
				}

				yield return FadeOutInterruptible(message);

				Debug.Log($"Info Message Display: Completed message: {message.messageText}");

				message.CompleteCallback?.Invoke();
			}
			else
			{
				yield return null;
			}
		}
	}

	// Helper method to wait for seconds but can be interrupted
	private IEnumerator WaitForSecondsInterruptible(float duration, InfoMessage.Message message)
	{
		float timeElapsed = 0f;
		while (timeElapsed < duration)
		{
			if (message != null && message.completeImmediately)
				yield break;

			timeElapsed += Time.deltaTime;

			yield return null;
		}
	}

	private IEnumerator FadeInInterruptible(InfoMessage.Message message)
	{
		float currentTime = Mathf.InverseLerp(0f, 1f, messageText.color.a) * fadeDuration;
		Color messageColor = messageText.color;
		Color progressColor = progressText.color;
		Color bgColor = Color.clear;

		if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
			bgColor = backgroundRenderer.material.GetColor(baseColorProperty);

		Debug.Log($"Info Message Display: Started fading in message");

		while (currentTime < fadeDuration && !message.skipFades)
		{
			currentTime += Time.deltaTime;
			float alpha = Mathf.Lerp(0f, 1f, currentTime / fadeDuration);
			messageColor.a = alpha;
			progressColor.a = alpha;
			messageText.color = messageColor;
			progressText.color = progressColor;

			if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
			{
				bgColor.a = alpha * 0.8f;
				backgroundRenderer.material.SetColor(baseColorProperty, bgColor);
			}

			yield return null;
		}

		Debug.Log($"Info Message Display: Finished fading in message");

		messageColor.a = 1f;
		progressColor.a = 1f;
		messageText.color = messageColor;
		progressText.color = progressColor;

		if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
		{
			bgColor.a = 0.8f;
			backgroundRenderer.material.SetColor(baseColorProperty, bgColor);
		}

	}

	private IEnumerator FadeOutInterruptible(InfoMessage.Message message)
	{
		float currentTime = Mathf.InverseLerp(1f, 0f, messageText.color.a) * fadeDuration;
		Color messageColor = messageText.color;
		Color progressColor = progressText.color;
		Color bgColor = Color.clear;

		if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
			bgColor = backgroundRenderer.material.GetColor(baseColorProperty);

		Debug.Log($"Info Message Display: Started fading out message");

		while (currentTime < fadeDuration && !message.skipFades)
		{
			currentTime += Time.deltaTime;
			float alpha = Mathf.Lerp(1f, 0f, currentTime / fadeDuration);
			messageColor.a = alpha;
			progressColor.a = alpha;
			messageText.color = messageColor;
			progressText.color = progressColor;

			if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
			{
				bgColor.a = alpha * 0.8f;
				backgroundRenderer.material.SetColor(baseColorProperty, bgColor);
			}

			yield return null;
		}

		Debug.Log($"Info Message Display: Finished fading out message");

		messageColor.a = 0f;
		progressColor.a = 0f;
		messageText.color = messageColor;
		progressText.color = progressColor;

		if (backgroundRenderer != null && backgroundRenderer.material.HasProperty(baseColorProperty))
		{
			bgColor.a = 0f;
			backgroundRenderer.material.SetColor(baseColorProperty, bgColor);
		}
	}

	private void UpdateBackgroundSize()
	{
		// Calculate and set the background size
		float padding = 1f;
		float messageTextHeight = messageText.preferredHeight;
		float progressTextHeight = progressText.preferredHeight;
		float newHeight = messageTextHeight + progressTextHeight + padding;
		float minHeight = 2f;

		Vector3 newScale = backgroundRenderer.transform.localScale;
		newScale.y = Mathf.Max(newHeight, minHeight);
		backgroundRenderer.transform.localScale = newScale;

		// Offset the background position to include the progress text, with added spacing between the message and progress text
		float progressTextOffset = progressTextHeight * 0.5f;
		float paddingOffset = 0f;// progressTextHeight > 0 ? padding * 0.25f : 0f;
		float combinedOffset = progressTextOffset + paddingOffset;
		Vector3 offset = new Vector3(0, -combinedOffset, 0);

		backgroundRenderer.transform.localPosition = offset;
	}
}