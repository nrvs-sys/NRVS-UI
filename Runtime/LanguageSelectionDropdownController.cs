using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

// IMPORTANT: this document requires encoding "UTF-8 with signature" to properly display the glyphs in the Autonym dictionary
public class LanguageSelectionDropdownController : MonoBehaviour
{

    TMP_Dropdown _dropdown;

    // optional: restrict to the locales we actually ship
    // (leave empty to show all project locales)
    string[] allowedIetfTags = {
        //"ja","zh-Hans","ko","es","fr","de","pt-BR","ru","it","en"
    };

    // autonyms (native names). fallback = LocaleName from Unity if missing.
    static readonly Dictionary<string, string> Autonym = new()
    {
        { "en", "English" },
        { "ja", "日本語" },
        { "zh-Hans", "中文（简体）" },
        { "ko", "한국어" },
        { "es", "Español" },
        { "fr", "Français" },
        { "de", "Deutsch" },
        { "pt-BR", "Português (Brasil)" },
        { "ru", "Русский" },
        { "it", "Italiano" }
    };

    List<Locale> _locales = new();

    int _lastValue = -1;
    bool _suppressCallback;

    void Awake()
    {
        _dropdown = GetComponentInChildren<TMP_Dropdown>(true);
        _dropdown.onValueChanged.AddListener(OnDropdownChanged);
    }

    private void Start()
    {
        RebuildOptions();
        RefreshValue();
    }

    void OnDestroy()
    {
        if (_dropdown)
            _dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
    }

    void RebuildOptions()
    {
        _dropdown.ClearOptions();

        // Add locales to the names buffer
        var all = LocalizationSettings.AvailableLocales.Locales;
        _locales = (allowedIetfTags is { Length: > 0 }
            ? all.Where(l => allowedIetfTags.Contains(l.Identifier.Code))
            : all).ToList();

        _dropdown.options = _locales
            .Select(l =>
			{
                var code = l.Identifier.Code;
                var label = Autonym.TryGetValue(code, out var n) ? n : l.LocaleName;
                return new TMP_Dropdown.OptionData(label);
			})
            .ToList();

        _dropdown.RefreshShownValue();
    }

    void RefreshValue()
    {
        // Set the dropdown value to the active language silently
        int idx = _locales.FindIndex(l => l == LocalizationSettings.SelectedLocale);

        _lastValue = idx;
        _suppressCallback = true;
        _dropdown.SetValueWithoutNotify(idx);
        _dropdown.RefreshShownValue();
        _suppressCallback = false;
    }

    void OnDropdownChanged(int value)
    {
        if (_suppressCallback || value < 0 || value >= _locales.Count)
            return;

        // Set the locale/language and serialze the selection
        LocalizationSettings.SelectedLocale = _locales[value];

        _lastValue = value;
    }
}