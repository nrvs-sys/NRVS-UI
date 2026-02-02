using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using NRVS.Settings;

/// <summary>
/// Used for initializing UI elements from settings and updating settings from UI changes
/// </summary>
public abstract class UISettingsUtility : MonoBehaviour
{
	[Header("Setting"), Expandable]
	public SettingsBehavior settingsBehavior;
}