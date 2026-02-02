using Services;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityAtoms.BaseAtoms;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Networking;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StatusDisplayFormatted : MonoBehaviour
{
	[Header("Settings")]
	public LocalizedString dateLabelString;
	public LocalizedString timeLabelString;
	public LocalizedString networkStateOnlineString;
	public LocalizedString networkStateOfflineString;

	[Header("References")]
	public ApplicationVersion applicationVersion;
	public NetworkStateVariable networkStateVariable;
	public PlatformErrorData platformErrorData;

	private string dateLabel;
	private string timeLabel;
	private string networkStateLabel;

	private Network.NetworkState _networkState;
	private Network.NetworkState networkState
	{
		get => _networkState;
		set
		{
			if (_networkState == value)
				return;

			_networkState = value;

			UpdateNetworkStateLabel();
		}
	}

	private readonly Dictionary<string, string> lastLoginStates = new();
	private readonly Dictionary<string, string> localizedErrorMessages = new();

	private TextMeshProUGUI statusText;

	private void Awake()
	{
		statusText = GetComponent<TextMeshProUGUI>();
	}

	private void OnEnable()
	{
		// Update immediately on enable
		Update();
	}

	private void Start()
	{
		dateLabelString.StringChanged += SetDateLabel;
		timeLabelString.StringChanged += SetTimeLabel;
		networkStateOnlineString.StringChanged += SetOnlineLabel;
		networkStateOfflineString.StringChanged += SetOfflineLabel;
	}

	private void OnDestroy()
	{
		dateLabelString.StringChanged -= SetDateLabel;
		timeLabelString.StringChanged -= SetTimeLabel;
		networkStateOnlineString.StringChanged -= SetOnlineLabel;
		networkStateOfflineString.StringChanged -= SetOfflineLabel;
	}

	private void Update()
	{
		UpdatePlatformErrorStatus("Player", Ref.Get<IPlayerPlatform>());
		UpdatePlatformErrorStatus("Leaderboard", Ref.Get<ILeaderboardPlatform>());
		UpdatePlatformErrorStatus("Lobby", Ref.Get<ILobbyPlatform>());
		UpdatePlatformErrorStatus("Relay", Ref.Get<IRelayPlatform>());


		networkState = networkStateVariable.Value;


		statusText.text =
			$"{dateLabel}///{GetDate()}" +                        // date
			$"{timeLabel}///{GetTime()}" +                        // time
			$"\n" +
			$"{networkStateLabel}\n";             // online status

		// append all cached platform error messages
		foreach (var msg in localizedErrorMessages.Values)
			statusText.text += msg + "\n";
	}

	private void UpdatePlatformErrorStatus(string key, IPlatform platform)
	{
		if (platform == null || !platform.hasLoginError)
		{
			// Optionally clear the message if the platform recovered
			localizedErrorMessages.Remove(key);
			return;
		}

		string currentLoginState = platform.loginState ?? "";

		// if login state didn't change, skip
		if (lastLoginStates.TryGetValue(key, out var last) && last == currentLoginState)
			return;

		lastLoginStates[key] = currentLoginState;

		var entry = platformErrorData.GetEntry(key, currentLoginState);
		entry.localizedError.GetLocalizedStringAsync().Completed += handle =>
		{
			var platformName = platform.GetType().Name.Replace("Manager", "");
			var color = "red";

			string display = $"{GetColorString(platformName, color)}\n{handle.Result}";
			localizedErrorMessages[key] = display;
		};
	}

	// deprecated
	private string GetPlatformStatus(IPlatform platform)
	{
		string statusText = "";
		var platformName = "";
		var hasError = false;
		var isLoggedIn = false;
		var loginState = "";
		var displayName = "";

		if (platform != null)
		{
			// Platform is determined at runtime using the loaded IPlayerPlatform instance
			platformName = $"{ platform.GetType().Name.Replace("Manager", "")}"; // Removes "Manager" from the class name, if present
			displayName = $"{platform.displayName}" ?? "--";
			loginState = platform.loginState;
			isLoggedIn = platform.isLoggedIn;
			hasError = platform.hasLoginError;
		}
		else
		{
			return $"{GetColorString("No player platform found!", "red")}\n";
		}

		if (isLoggedIn)
		{
			statusText =
				$"{GetColorString(platformName, "green")}\n" +
				$"{displayName}\n";
		}
		else if (hasError)
		{
			statusText =
				$"{GetColorString(platformName, "red")}\n" +
				$"{loginState}\n" +
				$"{displayName}\n";
		}
		else
		{
			statusText =
				$"{platformName}\n" +
				$"{displayName}\n";
		}

		return statusText;
	}

	private string GetDate()
	{
		var culture = System.Globalization.CultureInfo.CurrentCulture;
		var now = System.DateTime.Now;

		// Use the short date pattern (e.g., MM/dd/yyyy or dd.MM.yyyy depending on locale)
		string date = now.ToString(culture.DateTimeFormat.ShortDatePattern, culture);
		return $"{date}\n";
	}

	private string GetTime()
	{
		var culture = System.Globalization.CultureInfo.CurrentCulture;
		var now = System.DateTime.Now;

		// Use the long time pattern (e.g., 9:31:26 AM or 21:31:26 depending on locale)
		string time = now.ToString(culture.DateTimeFormat.LongTimePattern, culture);
		return $"{time}\n";
	}

	private string GetColorString(string value, string colorName) => value = $"<color={colorName}>{value}</color>";

	private void SetDateLabel(string value) => dateLabel = value;
	private void SetTimeLabel(string value) => timeLabel = value;
	private void SetOnlineLabel(string value)
	{
		if (_networkState == Network.NetworkState.Online)
			networkStateLabel = value;
	}
	private void SetOfflineLabel(string value)
	{
		if (_networkState != Network.NetworkState.Online)
			networkStateLabel = value;
	}

	private void UpdateNetworkStateLabel()
	{
		var label = _networkState == Network.NetworkState.Online ? networkStateOnlineString : networkStateOfflineString;

		label.GetLocalizedStringAsync().Completed += handle =>
		{
			networkStateLabel = handle.Result;
		};
	}
}