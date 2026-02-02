using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResolutionDropdownController : MonoBehaviour
{
    [SerializeField] DisplayUtility displayUtility;
    TMP_Dropdown _dropdown;

    // cache of formatted resolution strings (rebuilt only on display change)
    readonly List<string> _options = new List<string>(16);

    int _lastValue = -1;
    bool _suppressCallback;

    void Awake()
    {
        _dropdown = GetComponentInChildren<TMP_Dropdown>(true);
        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void Start()
    {
        RebuildOptions();   // once at start
        RefreshValue();

        if (displayUtility != null)
        {
            displayUtility.onDisplayChanged.AddListener(OnDisplayOrResolutionChanged);
            displayUtility.onResolutionChanged.AddListener(OnDisplayOrResolutionChanged);
            displayUtility.onFullscreenModeChanged.AddListener(OnDisplayOrResolutionChanged); // optional, in case supported modes differ
        }
    }

    void OnDestroy()
    {
        if (_dropdown) _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        if (displayUtility != null)
        {
            displayUtility.onDisplayChanged.RemoveListener(OnDisplayOrResolutionChanged);
            displayUtility.onResolutionChanged.RemoveListener(OnDisplayOrResolutionChanged);
            displayUtility.onFullscreenModeChanged.RemoveListener(OnDisplayOrResolutionChanged);
        }
    }

    void OnDisplayOrResolutionChanged()
    {
        RebuildOptions();
        RefreshValue();
    }

    void RebuildOptions()
    {
        _options.Clear();

        var res = displayUtility.GetResolutions();
        for (int i = 0; i < res.Length; i++)
        {
            // Light-weight format; new string per option but only on rebuild
            // e.g., "1920 x 1080 @ 90Hz"
            var hz = (float)res[i].refreshRateRatio.value;
            _options.Add($"{res[i].width} x {res[i].height} @ {hz}Hz");
        }

        _dropdown.ClearOptions();
        _dropdown.AddOptions(_options);

        _lastValue = 0;
    }

    void RefreshValue()
    {
        int idx = displayUtility.GetCurrentResolutionIndex();
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
        displayUtility.SetResolution(value);
        _lastValue = value;
    }
}
