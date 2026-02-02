using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayDropdownController : MonoBehaviour
{
    [SerializeField] 
    DisplayUtility displayUtility;

    TMP_Dropdown _dropdown;

    // small reusable buffer to avoid ToList/allocs
    readonly List<string> _namesBuffer = new List<string>(8);

    int _lastValue = -1;
    bool _suppressCallback;

    void Awake()
    {
        _dropdown = GetComponentInChildren<TMP_Dropdown>(true);
        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    void Start()
    {
        RebuildOptions();
        RefreshValue();

        if (displayUtility != null)
            displayUtility.onDisplayChanged.AddListener(OnDisplayChanged);
    }

    void OnDestroy()
    {
        if (_dropdown) 
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        if (displayUtility != null)
            displayUtility.onDisplayChanged.RemoveListener(OnDisplayChanged);
    }

    void OnDisplayChanged()
    {
        RebuildOptions();
        RefreshValue();
    }

    void RebuildOptions()
    {
        _namesBuffer.Clear();
        var displays = displayUtility.GetDisplays();

        for (int i = 0; i < displays.Count; i++)
            _namesBuffer.Add(displays[i].name);

        _dropdown.ClearOptions();
        _dropdown.AddOptions(_namesBuffer);
    }

    void RefreshValue()
    {
        int idx = displayUtility.GetCurrentDisplayIndex();

        if (idx == _lastValue) 
            return;

        _lastValue = idx;
        _suppressCallback = true;
        _dropdown.SetValueWithoutNotify(idx);
        _dropdown.RefreshShownValue();
        _suppressCallback = false;
    }

    void OnDropdownChanged(int value)
    {
        if (_suppressCallback) 
            return;

        displayUtility.SetDisplay(value);
        _lastValue = value;
    }
}
