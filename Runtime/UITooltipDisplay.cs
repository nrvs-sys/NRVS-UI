using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class UITooltipDisplay : MonoBehaviour
{
	[Header("Components")]

	[Tooltip("This object will be searched for UITooltip components to track")]
	public GameObject container;

	public TextMeshProUGUI textMesh;

	[Header("Events")]
	public UnityEvent onTooltipHoverEnter;
	public UnityEvent onTooltipHoverExit;


	private UITooltip[] tooltips = new UITooltip[0];
	private List<UITooltip> activeTooltips = new List<UITooltip>();


	private void Start()
	{
		// Could this search be done at edit/build time?
		tooltips = container.GetComponentsInChildren<UITooltip>(includeInactive: true);

		foreach (UITooltip tooltip in tooltips)
		{
			tooltip.onHoverEnter.AddListener(UITooltip_onHoverEnter);
			tooltip.onHoverExit.AddListener(UITooltip_onHoverExit);
		}
	}

	private void OnDestroy()
	{
		foreach (UITooltip tooltip in tooltips)
		{
			if (tooltip != null)
			{
				tooltip.onHoverEnter.RemoveListener(UITooltip_onHoverEnter);
				tooltip.onHoverExit.RemoveListener(UITooltip_onHoverExit);
			}
		}
	}


	public void SetToActiveTooltip()
	{
		var text = activeTooltips.Count > 0 ? activeTooltips[activeTooltips.Count - 1].text : string.Empty;
		textMesh.text = text;
	}


	private void UITooltip_onHoverEnter(UITooltip tooltip)
	{
		// Only show tooltips where the component is enabled
		if (!tooltip.enabled)
			return;


		activeTooltips.Add(tooltip);

		SetToActiveTooltip();

		onTooltipHoverEnter?.Invoke();
	}

	private void UITooltip_onHoverExit(UITooltip tooltip)
	{
		activeTooltips.Remove(tooltip);

		SetToActiveTooltip();

		onTooltipHoverExit?.Invoke();
	}
}