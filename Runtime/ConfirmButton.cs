using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.InputSystem;

public class ConfirmButton : MonoBehaviour, IPointerClickHandler, IDeselectHandler
{
    [Header("Components")]
    public LocalizeStringEvent buttonText;

    [Header("Localized Strings")]
    public LocalizedString defaultText;
    public LocalizedString confirmText;

    [Header("Events")]
    public UnityEvent onConfirm;
    public UnityEvent onConfirmRequested;
    public UnityEvent onReset;
    public UnityEvent onResetSilent;

    private bool isConfirming = false;

    private void OnEnable()
    {
        ResetStateSilent();
    }

    private void OnDisable()
    {
        ResetStateSilent();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isConfirming)
        {
            onConfirm?.Invoke();
            ResetState();
        }
        else
        {
            isConfirming = true;
            buttonText.StringReference = confirmText;
            buttonText.RefreshString();

            onConfirmRequested?.Invoke();

            // Select this button so OnDeselect will fire if user clicks away
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // If we’re still in a confirmation state and something else gets selected,
        // reset the button.
        if (isConfirming)
        {
            ResetState();
        }
    }

    private void ResetState()
    {
        isConfirming = false;
        buttonText.StringReference = defaultText;
        buttonText.RefreshString();
        onReset?.Invoke();
    }

    private void ResetStateSilent()
	{
        isConfirming = false;
        buttonText.StringReference = defaultText;
        buttonText.RefreshString();
        onResetSilent?.Invoke();
    }
}