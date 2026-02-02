using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class FullscreenDropdownController : MonoBehaviour
{
    [SerializeField] DisplayUtility displayUtility;
    TMP_Dropdown _dropdown;

    int _lastValue = -1;
    bool _suppressCallback;

    void Awake()
    {
        _dropdown = GetComponentInChildren<TMP_Dropdown>(true);
        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }
    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }


    void Start()
    {
        RebuildOptions();
        RefreshValue();

        if (displayUtility != null)
            displayUtility.onFullscreenModeChanged.AddListener(OnModeChanged);
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnDestroy()
    {
        if (_dropdown) _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        if (displayUtility != null)
            displayUtility.onFullscreenModeChanged.RemoveListener(OnModeChanged);
    }

    void OnModeChanged()
    {
        // Names generally don’t change, but leaving this for localization updates
        RebuildOptions();
        RefreshValue();
    }

    void RebuildOptions()
    {
        _dropdown.ClearOptions();
        // This returns your prebuilt cache -> no LINQ/no alloc work here
        _dropdown.AddOptions(displayUtility.GetFullScreenModeNames());
    }

    void RefreshValue()
    {
        int idx = displayUtility.GetCurrentFullScreenModeIndex();
        if (idx == _lastValue) return;

        _lastValue = idx;
        _suppressCallback = true;
        _dropdown.SetValueWithoutNotify(idx);
        _dropdown.RefreshShownValue();
        _suppressCallback = false;
    }

    void OnDropdownChanged(int value)
    {
        if (_suppressCallback) return;
        displayUtility.SetFullScreenMode(value);
        _lastValue = value;
    }

    private void OnLocaleChanged(Locale locale)
    {
        displayUtility.InitializeFullScreenModeNameCache();

        // re-apply formatting so the texts refresh in the new language
        OnModeChanged();
    }
}
