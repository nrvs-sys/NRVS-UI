using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UI Element_ New", menuName = "UI/UI Element")]
public class UIElement : ManagedObject
{
    [NonSerialized]
    List<UIController> controllers = new();

    protected override void Initialize()
    {
        controllers.Clear();
    }

    protected override void Cleanup()
    {
        controllers.Clear();
    }

    public void RegisterController(UIController controller)
    {
        if (!controllers.Contains(controller))
            controllers.Add(controller);
    }

    public void UnregisterController(UIController controller)
    {
        if (controllers.Contains(controller))
            controllers.Remove(controller);
    }

    [Button]
    public void Show()
    {
        foreach (var controller in controllers)
            if (!controller.isShowing)
                controller.Show();
    }

    [Button]
    public void Hide()
    {
        foreach (var controller in controllers)
            if (controller.isShowing)
                controller.Hide();
    }

    [Button]
    public void Toggle()
    {
        foreach (var controller in controllers)
            controller.Toggle();
    }

    public UIController[] GetControllers() => controllers.ToArray();
}
