using UnityEngine;
using UnityEngine.UI;

public class UILayoutUtility : MonoBehaviour
{
    public void RefreshLayout()
    {
        if (TryGetComponent(out RectTransform target))
            target = transform as RectTransform;

        LayoutRebuilder.ForceRebuildLayoutImmediate(target);
    }
}