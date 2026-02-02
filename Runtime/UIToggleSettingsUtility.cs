using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIToggleSettingsUtility : UISettingsUtility
{
	[Header("Toggle Components")]
	public Toggle toggle;

	bool toggleValueChanging = false;

    private void OnEnable()
	{
		if (settingsBehavior == null)
			return;


		if (toggle != null)
		{
			int isActive = settingsBehavior.GetInt();

			toggle.isOn = isActive.ToBool();
		}
	}

	private void Start()
	{
		if (settingsBehavior == null)
			return;

		settingsBehavior.onIntChanged.AddListener(SettingsBehavior_onIntChanged);
		toggle.onValueChanged.AddListener(Toggle_onValueChanged);
	}

    private void SettingsBehavior_onIntChanged(int value)
    {
        if (toggleValueChanging)
            return;

        toggle.isOn = value.ToBool();
    }

    private void OnDestroy()
	{
		if (settingsBehavior == null)
			return;

		settingsBehavior.onIntChanged.RemoveListener(SettingsBehavior_onIntChanged);

		toggle.onValueChanged.RemoveListener(Toggle_onValueChanged);
	}


	private void Toggle_onValueChanged(bool value)
	{
		if (settingsBehavior == null)
			return;

		toggleValueChanging = true;
		settingsBehavior.SetValue(value.ToInt());
		toggleValueChanging = false;
    }
}