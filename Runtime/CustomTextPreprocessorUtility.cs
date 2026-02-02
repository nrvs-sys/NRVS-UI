using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class CustomTextProcessorUtility : MonoBehaviour
{
    private void Awake()
    {
        var tmp = GetComponent<TMP_Text>();
        tmp.textPreprocessor = new CustomTextPreprocessor();
    }
}
