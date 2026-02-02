using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes a UI Dropdown from settings and updates settings from Dropdown changes
/// </summary>
public class UIDropdownSettingsUtility : UISettingsUtility
{
	[Header("Dropdown Components")]
	public Dropdown dropdown;


	private void OnEnable()
	{
		if (settingsBehavior == null)
			return;


		if (dropdown != null)
			dropdown.value = settingsBehavior.GetInt();
	}

	private void Start()
	{
		if (settingsBehavior == null)
			return;


		if (dropdown != null)
			dropdown.onValueChanged.AddListener(Dropdown_onValueChanged);
	}

	private void OnDestroy()
	{
		if (settingsBehavior == null)
			return;


		if (dropdown != null)
			dropdown.onValueChanged.RemoveListener(Dropdown_onValueChanged);
	}


	private void Dropdown_onValueChanged(int value) => settingsBehavior?.SetValue(value);
}