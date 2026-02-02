using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class DialogController : UIController
{
    #region Types

    [Serializable]
    public struct ButtonDefinition
    {
        public string text;
        public UnityAction onClick;
        public bool hideOnClick;

        public ButtonDefinition(string text, UnityAction onClick, bool hideOnClick = true)
        {
            this.text = text;
            this.onClick = onClick;
            this.hideOnClick = hideOnClick;
        }
    }

    [Serializable]
    public struct DialogDefinition
    {
        public string title;
        public string message;
        public ButtonDefinition[] buttons;

        public DialogDefinition(string title, string message, params ButtonDefinition[] buttons)
        {
            this.title = title;
            this.message = message;
            this.buttons = buttons ?? Array.Empty<ButtonDefinition>();
        }
    }
    #endregion

    [Header("Components")]

    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text messageText;
    [SerializeField] Transform buttonContainer;

    [Header("References")]

    [SerializeField] GameObject buttonPrefab;

    // Runtime state
    DialogDefinition definition;
    readonly List<Button> _spawnedButtons = new();

    public void SetDialogDefinition(DialogDefinition options)
    {
        definition = options;
        RefreshDialog();
        Show();
    }

    void RefreshDialog()
    {

        titleText?.SetText(definition.title);
        messageText?.SetText(definition.message);

        // Clear previous buttons
        foreach (var b in _spawnedButtons)
            Destroy(b.gameObject);
        _spawnedButtons.Clear();

        if (buttonContainer == null || buttonPrefab == null)
        {
            if (definition.buttons.Length > 0)
                Debug.LogWarning("DialogController: Button container or prefab is not set, buttons will not be displayed.");

            return;
        }

        // Rebuild
        for (int i = 0; i < definition.buttons.Length; i++)
        {
            var def = definition.buttons[i];
            var go = Instantiate(buttonPrefab, buttonContainer);
            var btn = go.GetComponent<Button>();
            var txt = go.GetComponentInChildren<TMP_Text>();

            if (txt) txt.text = def.text;
            if (btn)
            {
                int index = i; // capture for closure
                btn.onClick.AddListener(() => OnButtonClicked(index));
                _spawnedButtons.Add(btn);
            }
        }

        // Auto‑select first button (for game‑pads).
        if (_spawnedButtons.Count > 0)
            _spawnedButtons[0].Select();
    }

    void OnButtonClicked(int index)
    {
        if (index < 0 || index >= definition.buttons.Length)
        {
            Debug.LogError($"Dialog button index {index} out of range.");
            return;
        }

        var def = definition.buttons[index];

        try { def.onClick?.Invoke(); }
        catch (Exception e) { Debug.LogException(e, this); }

        if (def.hideOnClick)
            Hide();
    }
}
