using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class Carousel : UIBehaviour
{
    [Header("Settings")]
    [Tooltip("Obsolete - this is here only for reference. Use localizedItems instead")]
    public List<string> items = new List<string>(); // Retained for reference only, no functional use

    public List<LocalizedString> localizedItems = new List<LocalizedString>();

    public bool requireLocalizedText = true;

    public bool wrapping = true;

    [SerializeField]
    private int _item = -1;

    public int item
    {
        get => _item;
        set
        {
            var count = requireLocalizedText ? localizedItems.Count : items.Count;

            if (count == 0 || _item == value)
                return;

            if (value < 0 || value >= count)
            {
                if (wrapping)
                {
                    _item = (value + count) % count;
                }
            }
            else
            {
                _item = value;
            }

            UpdateItemText();

            onItemChanged?.Invoke(_item);
        }
    }

    [Header("Components")]
    public TextMeshProUGUI itemText;
    public Animator itemAnimator;
    public Button buttonPrevious;
    public Button buttonNext;

    [Header("Events")]
    public UnityEvent<int> onItemChanged;


    private bool _interactable = true;
    public bool interactable
	{
        get => interactable;
        set
		{
            _interactable = value;

            if (itemAnimator != null)
            {
                if (_interactable)
                    itemAnimator.SetTrigger("Normal");
                else
                    itemAnimator.SetTrigger("Disabled");
            }

            if (buttonPrevious != null)
                buttonPrevious.interactable = _interactable;

            if (buttonNext != null)
                buttonNext.interactable = _interactable;
        }
	}


    protected override void Awake()
    {
        base.Awake();
        if (_item >= 0)
            UpdateItemText();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    private void UpdateItemText()
    {
        if (_item >= 0)
        {
            if (_item < localizedItems.Count)
            {
                localizedItems[_item].GetLocalizedStringAsync().Completed += handle =>
                {
                    itemText.text = handle.Result;
                };
            }
            else if (requireLocalizedText)
            {
                Debug.LogError($"Item index {_item} is out of range for localized items list.");
            }
            else
            {
                itemText.text = items[_item];
            }
        }
    }

    public void Previous() => item--;
    public void Next() => item++;
    public void Set(int value) => item = value;

    private void OnLocaleChanged(Locale locale)
    {
        // re-apply formatting so the texts refresh in the new language
        UpdateItemText();
    }
}