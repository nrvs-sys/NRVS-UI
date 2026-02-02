using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[CreateAssetMenu(fileName = "Input Display Overrides_ New", menuName = "Data/Input Display Overrides")]
public class InputDisplayOverrides : ScriptableObject
{
	[SerializeField]
	private List<InputDisplayOverride> overrides = new List<InputDisplayOverride>();

	[Serializable]
	public class InputDisplayOverride
	{
		[SerializeField, Tooltip("Control names are derived from the binding path of an action in the Input Actions asset")]
		public List<string> controlNames;
		[SerializeField, Tooltip("Sprite names are defined in sprite assets created for TextMesh Pro")]
		public string spriteName;
		[SerializeField, Tooltip("A text name to display if there is no sprite name set")]
		public string displayName;

		public string displayOverride
		{
			get
			{
				// Return sprite name if it's not empty
				if (!string.IsNullOrEmpty(spriteName))
				{
					// TODO: Select the appropriate sprite atlas for the active control scheme
					return $"<sprite=\"MAGE_ Input Sprites_ XR\" name=\"{spriteName}\">";
				}

				// Return display name if it's not empty
				if (!string.IsNullOrEmpty(displayName))
				{
					return $"<g1>{displayName}</g1>";
				}

				// Return the first control name if the list is not empty
				if (controlNames != null && controlNames.Count > 0)
				{
					return $"<g1>{controlNames[0]}</g1>";
				}

				// Default to "UNDEFINED" if none of the above are available
				return $"<g1>UNDEFINED</g1>";
			}
		}
	}

	public bool TryGetOverride(string controlName, out string displayOverride)
	{
		var inputDisplayOverride = overrides.Find(s => s.controlNames.Contains(controlName));

		if (inputDisplayOverride != null)
		{
			displayOverride = inputDisplayOverride.displayOverride;
			return true;
		}
		else
		{
			displayOverride = string.Empty;
			return false;
		}
	}
}