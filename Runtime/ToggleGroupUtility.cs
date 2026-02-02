using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class ToggleGroupUtility : MonoBehaviour
{
	[Header("Settings")]
	public int initialActiveToggle = -1;

	[Header("Events")]
	public UnityEvent<int> onToggleSet;


	private ToggleGroup toggleGroup;
	private List<Toggle> toggles;


	private void Awake()
	{
		toggleGroup = GetComponent<ToggleGroup>();
		toggles = GetComponentsInChildren<Toggle>().ToList();
	}

	private void OnEnable()
	{
		for (int i = 0; i < toggles.Count; i++)
			toggles[i].isOn = i == initialActiveToggle;

		foreach (Toggle toggle in toggles)
			toggle.onValueChanged.AddListener(Toggle_onValueChanged);
	}

	private void OnDisable()
	{
		foreach (Toggle toggle in toggles)
			toggle.onValueChanged.RemoveListener(Toggle_onValueChanged);
	}


	public void AddToggle(Toggle toggle)
	{
		if (toggles == null)
			toggles = new List<Toggle>();

		if (toggleGroup == null)
			toggleGroup = GetComponent<ToggleGroup>();


		toggle.group = toggleGroup;
		toggleGroup.RegisterToggle(toggle);

		toggles.Add(toggle);

		if (enabled)
			toggle.onValueChanged.AddListener(Toggle_onValueChanged);
	}

	public void RemoveToggle(Toggle toggle)
	{
		toggle.group = null;
		toggleGroup.UnregisterToggle(toggle);

		toggles.Remove(toggle);

		if (enabled)
			toggle.onValueChanged.RemoveListener(Toggle_onValueChanged);
	}


	private void Toggle_onValueChanged(bool value)
	{
		if (value)
		{
			int activeToggleIndex = toggles.IndexOf(toggleGroup.GetFirstActiveToggle());

			onToggleSet?.Invoke(activeToggleIndex);
		}
		else if (!toggleGroup.AnyTogglesOn())
		{
			onToggleSet?.Invoke(-1);
		}
	}
}