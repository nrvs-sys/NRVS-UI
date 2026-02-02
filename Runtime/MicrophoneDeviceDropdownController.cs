using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using Dissonance;
using NRVS.Settings;

public class MicrophoneDeviceDropdownController : MonoBehaviour
{
    [SerializeField] SettingsBehavior transmissionDeviceSettingsBehavior;

    DissonanceComms dissonanceComms;

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

    private IEnumerator Start()
    {
        VoipManager voipManager;

        while (!Ref.TryGet(out voipManager))
            yield return null;

        while (!voipManager.isInitialized)
            yield return null;

        dissonanceComms = voipManager.dissonanceComms;

        RebuildOptions();
        RefreshValue();
    }

    void OnDestroy()
    {
        if (_dropdown)
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    void RebuildOptions()
    {
        _namesBuffer.Clear();

        dissonanceComms.GetMicrophoneDevices(_namesBuffer);

        _dropdown.ClearOptions();
        _dropdown.AddOptions(_namesBuffer);
    }

    void RefreshValue()
    {
        int idx = _namesBuffer.IndexOf(dissonanceComms.MicrophoneName);

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
        if (_suppressCallback || _namesBuffer.Count <= 0)
            return;

        var deviceName = _namesBuffer[value];
        transmissionDeviceSettingsBehavior.SetValue(deviceName);

        _lastValue = value;
    }
}