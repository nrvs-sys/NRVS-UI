using UnityEngine;
using UnityEngine.Localization.Components;

public class PlayerCountDisplay : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizedEvent;

    private void OnEnable()
    {
        if (PresenceManager.Instance != null)
        {
            PresenceManager.Instance.OnOnlineCountChanged += HandleCountChanged;
            HandleCountChanged(PresenceManager.Instance.OnlineCount);

            PresenceManager.Instance.OnPresenceStateChanged += HandleStateChanged;

            // initial paint
            RefreshDisplay(PresenceManager.Instance);
        }
        else
        {
            ShowUnavailable(); // don’t show 0 by default
        }
    }

    private void OnDisable()
    {
        if (PresenceManager.Instance != null)
        {
            PresenceManager.Instance.OnOnlineCountChanged -= HandleCountChanged;
            PresenceManager.Instance.OnPresenceStateChanged -= HandleStateChanged;
        }
    }

    private void HandleCountChanged(int _)
    {
        RefreshDisplay(PresenceManager.Instance);
    }

    private void HandleStateChanged(PresenceManager.PresenceServiceState _)
    {
        RefreshDisplay(PresenceManager.Instance);
    }

    private void RefreshDisplay(PresenceManager pm)
    {
        if (localizedEvent == null)
            return;

        if (pm == null || !pm.OnlineCountValid)
        {
            ShowUnavailable(pm);
            return;
        }

        SetCount(pm.OnlineCount);
    }

    private void SetCount(int count)
    {
        var reference = localizedEvent.StringReference;
        reference.Arguments = new object[] { count };
        localizedEvent.RefreshString();
    }

    private void ShowUnavailable(PresenceManager pm = null)
    {
        // simplest: blank the label so you don’t lie
        // (alternatively: swap to a different localized entry key like "PlayersOnline_Unavailable")
        var reference = localizedEvent.StringReference;

        // if you want a “—” placeholder via smart string:
        // set argument 0 to null-ish sentinel and handle it in the table, OR just hardcode here.
        reference.Arguments = new object[] { "—" };
        localizedEvent.RefreshString();
    }

}
