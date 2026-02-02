using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Scrollbar))]
public class ScrollbarAudioUtility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Settings")]
	public EventReference scrollSound;

	[Tooltip("Delay after scrolling stops to fade out sound")]
	public float stopDelay = 0.1f;


	private Scrollbar scrollbar;
	private EventInstance scrollInstance;
	private float lastValue;
	private float idleTimer = 0f;
	private bool isDown = false;

	private const string scrollingMovingParameterName = "Scrollbar Moving";


	private void Awake()
	{
		scrollbar = GetComponent<Scrollbar>();
	}

	private void Start()
	{
		lastValue = scrollbar.value;
	}

	private void OnDisable()
	{
		if (scrollInstance.isValid())
		{
			StopSound();
		}
	}

	void Update()
	{
		// Detect movement by comparing current and previous scrollbar values.
		bool isMoving = scrollbar.value != lastValue;

		if (isMoving || isDown)
		{
			idleTimer = 0f;

			// Start the audio if it's not already playing.
			if (!scrollInstance.isValid())
			{
				scrollInstance = RuntimeManager.CreateInstance(scrollSound);
				scrollInstance.start();
			}
		}
		else
		{
			idleTimer += Time.deltaTime;

			if (idleTimer >= stopDelay && scrollInstance.isValid())
			{
				StopSound();
			}
		}

		// Update FMOD parameter for scrollbar moving with a discrete value (1 if moving, 0 if idle)
		if (scrollInstance.isValid())
		{
			scrollInstance.setParameterByName(scrollingMovingParameterName, isMoving.ToInt());
		}

		lastValue = scrollbar.value;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		isDown = true;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		isDown = false;
	}


	private void StopSound()
	{
		if (scrollInstance.isValid())
		{
			scrollInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			scrollInstance.release();
			scrollInstance.clearHandle();
		}
	}
}