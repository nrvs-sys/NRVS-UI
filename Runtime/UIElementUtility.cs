using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIElementUtility : MonoBehaviour
{
	private Selectable selectable;

	private void Awake()
	{
		selectable = GetComponent<Selectable>();
	}


	public void SetInteractable(bool interactable) => selectable.interactable = interactable;
	public void SetInteractableFromInt(int boolAsInt) => SetInteractable(boolAsInt.ToBool());
}