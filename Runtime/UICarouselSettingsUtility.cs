using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes a carousel from settings and updates settings from carousel changes
/// </summary>
public class UICarouselSettingsUtility : UISettingsUtility
{
	[Header("Carousel Components")]
	public Carousel carousel;


	private void OnEnable()
	{
		if (settingsBehavior == null)
			return;


		if (carousel != null)
		{
			int item = settingsBehavior.GetInt();

			carousel.item = item;
		}
	}

	private void Start()
	{
		if (settingsBehavior == null)
			return;


		carousel.onItemChanged.AddListener(Carousel_onItemChanged);
	}

	private void OnDestroy()
	{
		if (settingsBehavior == null)
			return;


		carousel.onItemChanged.RemoveListener(Carousel_onItemChanged);
	}


	private void Carousel_onItemChanged(int value)
	{
		settingsBehavior?.SetValue(value);
	}
}