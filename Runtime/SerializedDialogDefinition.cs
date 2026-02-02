using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "UI_ Dialog_ New", menuName = "UI/Dialog")]
public class SerializedDialogDefinition : ScriptableObject
{
    [Serializable]
    public class SerializedDialogButtonDefinition
    {
        [SerializeField]
        LocalizedString text;

        [SerializeField]
        UnityEvent onClick;

        [SerializeField]
        [Tooltip("If true, the dialog will close when this button is clicked.")]
        bool hideOnClick = true;

        public async Task<DialogController.ButtonDefinition> GetButtonDefinition()
        {
            var buttonText = await text.GetLocalizedStringAsync().Task;
            return new DialogController.ButtonDefinition(buttonText, onClick.Invoke, hideOnClick);
        }
    }

    [SerializeField]
    LocalizedString title = new LocalizedString();

    [SerializeField]
    LocalizedString message = new LocalizedString();

    public SerializedDialogButtonDefinition[] buttons = Array.Empty<SerializedDialogButtonDefinition>();

    public async Task<DialogController.DialogDefinition> GetDialogDefinition()
    {
        var titleText = await title.GetLocalizedStringAsync().Task;
        var messageText = await message.GetLocalizedStringAsync().Task;
        var buttonDefinitions = new DialogController.ButtonDefinition[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null)
            {
                Debug.LogWarning($"SerializedDialogDefinition: Button at index {i} is null, skipping.");
                continue;
            }

            buttonDefinitions[i] = await buttons[i].GetButtonDefinition();
        }
        return new DialogController.DialogDefinition(titleText, messageText, buttonDefinitions);
    }
}
