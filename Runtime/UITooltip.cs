using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityAtoms.BaseAtoms;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

public class UITooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Components")]

    public LocalizedString tooltipString;

    [Header("Events")]

    public UnityEvent<UITooltip> onHoverEnter;
    public UnityEvent<UITooltip> onHoverExit;


    public string text => cachedText;

    private bool isHovered = false;
    private string cachedText = string.Empty;


    public void OnPointerEnter(PointerEventData eventData) => HandleHoverEnter();
    public void OnPointerExit(PointerEventData eventData) => HandleHoverExit();


    private void Awake()
    {
        if (tooltipString != null)
        {
            tooltipString.StringChanged += OnLocalizedStringChanged;
            FetchLocalizedString();
        }
    }

    private void OnDestroy()
    {
        if (tooltipString != null)
        {
            tooltipString.StringChanged -= OnLocalizedStringChanged;
        }
    }

    private void FetchLocalizedString()
    {
        if (tooltipString != null &&
        tooltipString.TableReference.ReferenceType != TableReference.Type.Empty &&
        tooltipString.TableEntryReference.ReferenceType != TableEntryReference.Type.Empty)
        {
            tooltipString.GetLocalizedStringAsync().Completed += handle =>
            {
                cachedText = handle.Result;
            };
        }
    }

    private void OnLocalizedStringChanged(string newText)
    {
        cachedText = newText;
    }

    private void HandleHoverEnter()
    {
        if (!isHovered)
        {
            isHovered = true;
            onHoverEnter?.Invoke(this);
        }
    }

    private void HandleHoverExit()
    {
        if (isHovered)
        {
            isHovered = false;
            onHoverExit?.Invoke(this);
        }
    }
}