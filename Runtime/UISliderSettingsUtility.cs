using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes a UI slider from settings and updates settings from slider changes
/// </summary>
public class UISliderSettingsUtility : UISettingsUtility
{
	[Header("Slider Components")]
	public Slider slider;

	bool sliderValueChanging = false;

	private void OnEnable()
	{
		if (settingsBehavior == null)
			return;

        if (slider != null)
			slider.value = settingsBehavior.GetFloat();
	}

    private void SettingsBehavior_onFloatChanged(float value)
    {
        if (sliderValueChanging) 
			return;
        
        slider.value = value;
    }

    private void Start()
	{
		if (settingsBehavior == null)
			return;

		settingsBehavior.onFloatChanged.AddListener(SettingsBehavior_onFloatChanged);

        if (slider != null)
			slider.onValueChanged.AddListener(Slider_onValueChanged);
	}

	private void OnDestroy()
	{
		if (settingsBehavior == null)
			return;

		settingsBehavior.onFloatChanged.RemoveListener(SettingsBehavior_onFloatChanged);

        if (slider != null)
			slider.onValueChanged.RemoveListener(Slider_onValueChanged);
	}


	private void Slider_onValueChanged(float value)
	{
		if (settingsBehavior == null)
			return;

		sliderValueChanging = true;

		settingsBehavior.SetValue(value);

		sliderValueChanging = false;
	}
}
