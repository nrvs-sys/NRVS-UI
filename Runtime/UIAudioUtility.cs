using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FMODUnity;

public class UIAudioUtility : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler
{
    [Header("FMOD Audio Clips")]
    public EventReference hoverSound;
    public EventReference clickSound;
    public EventReference selectSound;
    public EventReference deselectSound;

    private Selectable selectable;
    private int pointerCount = 0;

    // Checks if the selectable is interactable (or absent)
    private bool isInteractable => selectable == null || selectable.interactable;

    private void Awake()
    {
        selectable = GetComponent<Selectable>();
        if (selectable == null)
        {
            Debug.LogWarning("UIAudioUtility requires a Selectable component on the same GameObject.");
        }
    }

	private void OnDisable()
	{
        pointerCount = 0;
	}

	// Only plays hoverSound when the first pointer enters.
	public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        if (pointerCount == 0 && !hoverSound.IsNull)
        {
            RuntimeManager.PlayOneShot(hoverSound, transform.position);
        }
        pointerCount++;
    }

    // Decrement the pointer count when a pointer exits.
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        pointerCount--;
        if (pointerCount < 0)
            pointerCount = 0;
    }

    // Plays the click sound when the UI element is clicked.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isInteractable)
            return;

        if (!clickSound.IsNull)
        {
            RuntimeManager.PlayOneShot(clickSound, transform.position);
        }
    }

    // Plays the selection sound when the element is selected.
    public void OnSelect(BaseEventData eventData)
    {
        if (!selectSound.IsNull)
        {
            RuntimeManager.PlayOneShot(selectSound, transform.position);
        }
    }

    // Plays the deselection sound when the element is deselected.
    public void OnDeselect(BaseEventData eventData)
    {
        if (!deselectSound.IsNull)
        {
            RuntimeManager.PlayOneShot(deselectSound, transform.position);
        }
    }
}