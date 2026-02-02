using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;
using FMOD.Studio;

[RequireComponent(typeof(Slider))]
public class SliderAudioUtility : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[Header("Settings")]
	public EventReference slideSound;

	[Tooltip("Delay after value change stops to fade out sound")]
	public float stopDelay = 0.1f;


	private Slider slider;
	private EventInstance sliderInstance;
	private float lastValue;
	private float idleTimer = 0f;
	private bool isDown = false;

	private const string scrollingMovingParameterName = "Scrollbar Moving";


	private void Awake()
	{
		slider = GetComponent<Slider>();
	}

	private void Start()
	{
		lastValue = slider.value;
	}

	private void OnDisable()
	{
		if (sliderInstance.isValid())
		{
			StopSound();
		}
	}

	void Update()
	{
		// Detect movement by comparing current and previous slider values.
		bool isMoving = slider.value != lastValue;

		if (isMoving || isDown)
		{
			idleTimer = 0f;

			// Start the audio if it's not already playing.
			if (!sliderInstance.isValid())
			{
				sliderInstance = RuntimeManager.CreateInstance(slideSound);
				sliderInstance.start();
			}
		}
		else
		{
			idleTimer += Time.deltaTime;

			if (idleTimer >= stopDelay && sliderInstance.isValid())
			{
				StopSound();
			}
		}

		// Update FMOD parameter for scrollbar moving with a discrete value (1 if moving, 0 if idle)
		if (sliderInstance.isValid())
		{
			sliderInstance.setParameterByName(scrollingMovingParameterName, isMoving.ToInt());
		}

		lastValue = slider.value;
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
		if (sliderInstance.isValid())
		{
			sliderInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			sliderInstance.release();
			sliderInstance.clearHandle();
		}
	}
}