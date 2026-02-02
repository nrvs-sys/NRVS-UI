using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIController : MonoBehaviour
{
    enum StartBehavior
    {
        None,
        Show,
        Hide,
        HideImmediately
    }

    [Header("UI Controller References")]

    [SerializeField]
    UIElement element;

    [Header("UI Controller Settings")]

    [SerializeField]
    StartBehavior startBehavior = StartBehavior.None;
    [SerializeField, Tooltip("If true, this GameObject will set itself Active/Inactive on Show/Hide, respectively.")]
    bool setChildrenActiveOnShow = true;

    [Header("UI Controller Events")]

    public UnityEvent onShow;
    public UnityEvent onHide;
    public UnityEvent onHideImmediately;

    public bool isShowing { get; private set; } = false;

    Coroutine transitionCoroutine;

    void Awake()
    {
        if (element != null)
            element.RegisterController(this);
    }

    void Start()
    {
        switch (startBehavior)
        {
            case StartBehavior.Show:
                Show();
                break;
            case StartBehavior.Hide:
                Hide();
                break;
            case StartBehavior.HideImmediately:
                HideImmediately();
                break;
            case StartBehavior.None:
                // Do nothing
                break;
        }
    }

    void OnDestroy()
    {
        if (element != null)
            element.UnregisterController(this);
    }

    public bool Show()
    {
        if (transitionCoroutine != null)
            return false;

        transitionCoroutine = StartCoroutine(InvokeShow());

        return true;
    }

    public bool Hide()
    {
        if (transitionCoroutine != null)
            return false;

        transitionCoroutine = StartCoroutine(InvokeHide());

        return true;
    }

    public virtual void HideImmediately()
    {
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
            transitionCoroutine = null;
        }

        isShowing = false;

        if (setChildrenActiveOnShow)
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

        onHideImmediately.Invoke();
    }

    public bool Toggle() => isShowing ? Hide() : Show();

    IEnumerator InvokeShow()
    {
        isShowing = true;

        yield return DoShow();

        transitionCoroutine = null;

        if (setChildrenActiveOnShow)
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(true);

        onShow.Invoke();
    }

    IEnumerator InvokeHide()
    {
        isShowing = false;

        yield return DoHide();

        transitionCoroutine = null;

        if (setChildrenActiveOnShow)
            for (int i = 0; i < transform.childCount; i++)
                transform.GetChild(i).gameObject.SetActive(false);

        onHide.Invoke();
    }

    protected virtual IEnumerator DoShow()
    {
        yield break;
    }

    protected virtual IEnumerator DoHide()
    {
        yield break;
    }
}
