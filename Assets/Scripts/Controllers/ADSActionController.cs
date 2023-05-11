using UnityEngine;
using UnityEngine.UI;

public class ADSActionController : MonoBehaviour
{
    [SerializeField]
    private Canvas aDSActionPanel;

    public Button watchADS, notWatchADS, cancel;

    private void Start()
    {
        watchADS.onClick.AddListener(() =>
        {
            Hide();
            IronSource.Agent.showRewardedVideo();
            });

        notWatchADS.onClick.AddListener(Hide);
        cancel.onClick.AddListener(Hide);
    }

    public void Show()
    {
        aDSActionPanel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        aDSActionPanel.gameObject.SetActive(false);
    }
}