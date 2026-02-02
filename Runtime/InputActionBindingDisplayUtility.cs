using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputActionBindingDisplayUtility : MonoBehaviour
{
	[Header("References")]
	public InputDisplayOverrides inputDisplayOverrides;

	[Header("Dependencies")]
	public Input.InputManager inputManager;


	#region Input Action Bindings
	// Application
	public string Start => GetBindingDisplayString(inputManager.actions.Application.Start);
	public string ShowPlayerList => GetBindingDisplayString(inputManager.actions.Application.ShowPlayerList);
	public string ToggleHUD => GetBindingDisplayString(inputManager.actions.Application.ToggleHUD);

	// Movement
	
	public string Movement => GetBindingDisplayString(inputManager.actions.Movement.Movement);
	public string Look => GetBindingDisplayString(inputManager.actions.Movement.Look);
    public string Rise => GetBindingDisplayString(inputManager.actions.Movement.Rise);
    public string Fall => GetBindingDisplayString(inputManager.actions.Movement.Fall);

    // Ability
    public string Jump => GetBindingDisplayString(inputManager.actions.Ability.Jump);
    public string Dash => GetBindingDisplayString(inputManager.actions.Ability.Dash);
    public string Sprint => GetBindingDisplayString(inputManager.actions.Ability.Sprint);
    public string Stomp => GetBindingDisplayString(inputManager.actions.Ability.Stomp);
    public string Ability => GetBindingDisplayString(inputManager.actions.Ability.Ability);

	// Guns (Left Hand)
	public string PrimaryFireLeft => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.PrimaryFire);
	public string AlternateFireLeft => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.AlternateFire);
	public string SelectWeaponLeft => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.SelectWeapon);
	public string NextWeaponLeft => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.NextWeapon);
	public string SetWeapon1Left => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.SetWeapon1);
	public string SetWeapon2Left => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.SetWeapon2);
	public string SetWeapon3Left => GetBindingDisplayString(inputManager.actions.Guns_LeftHand.SetWeapon3);

	// Guns (Right Hand)
	public string PrimaryFireRight => GetBindingDisplayString(inputManager.actions.Guns_RightHand.PrimaryFire);
	public string AlternateFireRight => GetBindingDisplayString(inputManager.actions.Guns_RightHand.AlternateFire);
	public string SelectWeaponRight => GetBindingDisplayString(inputManager.actions.Guns_RightHand.SelectWeapon);
	public string NextWeaponRight => GetBindingDisplayString(inputManager.actions.Guns_RightHand.NextWeapon);
	public string SetWeapon1Right => GetBindingDisplayString(inputManager.actions.Guns_RightHand.SetWeapon1);
	public string SetWeapon2Right => GetBindingDisplayString(inputManager.actions.Guns_RightHand.SetWeapon2);
	public string SetWeapon3Right => GetBindingDisplayString(inputManager.actions.Guns_RightHand.SetWeapon3);
	public string Combine => GetBindingDisplayString(inputManager.actions.Guns_RightHand.Combine);

	// Interaction
	public string Interact => GetBindingDisplayString(inputManager.actions.Interaction.Interact);

	#endregion


	private string GetBindingDisplayString(InputAction action)
	{
		var activeControlSchemeName = inputManager.GetActiveControlSchemeName();
		//Debug.LogWarning($"checking action: {action.name}, binding count: {action.bindings.Count}");
		// Loop through each binding for the action
		for (int i = 0; i < action.bindings.Count; i++)
		{
			var binding = action.bindings[i];
			//Debug.LogWarning($"checking action: {action.name}, binding: {binding.effectivePath}, isComposite: {binding.isComposite}, isPartOfComposite:{binding.isPartOfComposite}");

			// If this is a composite binding, iterate through the subsequent composite parts
			if (binding.isComposite)
			{
				string compositeBindingDisplayString = "";
				bool hasCompositeParts = false;

				for (i = i + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
				{
					var compositeBinding = action.bindings[i];

					if (compositeBinding.groups.Contains(activeControlSchemeName))
					{
						compositeBindingDisplayString += GetBindingDisplayStringFormatted(action, i);
						hasCompositeParts = true;
					}
				}

				// If there were composite parts, return the composite binding display string. Otherwise, continue checking for bindings
				if (hasCompositeParts)
				{
					return compositeBindingDisplayString;
				}
			}
			
			// Check if this binding belongs to the active control scheme
			if (binding.groups.Contains(activeControlSchemeName))
			{
				return GetBindingDisplayStringFormatted(action, i);
			}
		}

		// In case no binding matched the active control scheme, return NULL or a default message
		Debug.LogError($"There was no binding found for the active control scheme! Setting binding display string to NULL.");
		return $"<g1>NULL</g1>";
	}

	private string GetBindingDisplayStringFormatted(InputAction action, int bindingIndex)
	{
		var binding = action.bindings[bindingIndex];

		// Extract the control name
		var controlName = InputControlPath.ToHumanReadableString(binding.effectivePath);

		// Find a display override with a matching control name
		if (inputDisplayOverrides.TryGetOverride(controlName, out string displayOverride))
		{
			return displayOverride;
		}
		else
		{
			// If no display override is found, fallback to displaying the binding's display string

			// Display the first binding's display string (for the matched active control scheme binding)
			var bindingDisplayString = action.GetBindingDisplayString(bindingIndex, InputBinding.DisplayStringOptions.DontIncludeInteractions);

			Debug.LogWarning($"Could not find a display override for the action: {action.name}, with control name: {controlName}. Defaulting to a display string instead: {bindingDisplayString}.");

			return $"<g1>{bindingDisplayString}</g1>";
		}
	}
}