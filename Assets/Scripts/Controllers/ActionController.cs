using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ActionController : MonoBehaviour
{
    public static UnityAction<TypeAction> tileAction;

    [SerializeField]
    private Canvas actionPanel;

    public Button tryOpen;
    public Button setFlag;
    public Button cancel;

    public void Show(TileController controller)
    {
        HideAllActions();

        if (controller.GetTileState() == TileState.flagged)
        {
            SetFlagSetActive(true);
        }
        else
        {
            if (!GameController.instance.isFirstOpen)
            {
                SetFlagSetActive(true);
            }
            TryOpenSetActive(true);
        }

        cancel.onClick.AddListener(HideActionController);

        actionPanel.gameObject.SetActive(true);
    }

    private void TryOpenSetActive(bool status)
    {
        tryOpen.gameObject.SetActive(status);

        if (status)
        {
            tryOpen.onClick.AddListener(() => tileAction(TypeAction.open));
            tryOpen.onClick.AddListener(HideActionController);
        }
        else
        {
            tryOpen.onClick.RemoveAllListeners();
        }
    }

    private void SetFlagSetActive(bool status)
    {
        setFlag.gameObject.SetActive(status);

        if (status)
        {
            setFlag.onClick.AddListener(() => tileAction(TypeAction.flag));
            setFlag.onClick.AddListener(HideActionController);
            setFlag.onClick.AddListener(() => SoundController.instance.PlaySound(SoundController.SoundClip.Click));
        }
        else
        {
            setFlag.onClick.RemoveAllListeners();
        }
    }

    private void HideAllActions()
    {
        TryOpenSetActive(false);
        SetFlagSetActive(false);
    }

    public void HideActionController()
    {
        actionPanel.gameObject.SetActive(false);
        
        cancel.onClick.RemoveAllListeners();

        tileAction = null;
    }
}
