using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes a toggles in a toggle group from settings and updates settings from toggle changes
/// Toggle base index is 1, where 0 is no toggles selected
/// </summary>
public class UIToggleGroupSettingsUtility : UISettingsUtility
{
	[Header("Toggle Group Components")]
	public ToggleGroup toggleGroup;


	private List<Toggle> toggles;


	private void Awake()
	{
		toggles = GetComponentsInChildren<Toggle>().ToList();
	}

	private void OnEnable()
	{
		if (settingsBehavior == null)
			return;


		if (toggleGroup != null)
		{
			int activeToggleIndex = settingsBehavior.GetInt();
			
			for (int i = 0; i < toggles.Count; i++)
				toggles[i].isOn = i + 1 == activeToggleIndex;
		}
	}

	private void Start()
	{
		if (settingsBehavior == null)
			return;


		foreach (Toggle toggle in toggles)
			toggle.onValueChanged.AddListener(Toggle_onValueChanged);
	}

	private void OnDestroy()
	{
		if (settingsBehavior == null)
			return;


		foreach (Toggle toggle in toggles)
			toggle.onValueChanged.RemoveListener(Toggle_onValueChanged);
	}


	private void Toggle_onValueChanged(bool value)
	{
		if (value)
		{
			int activeToggleIndex = toggles.IndexOf(toggleGroup.GetFirstActiveToggle()) + 1;
			
			settingsBehavior?.SetValue(activeToggleIndex);
		}
		else if (!toggleGroup.AnyTogglesOn())
		{
			settingsBehavior?.SetValue(0);
		}
	}
}