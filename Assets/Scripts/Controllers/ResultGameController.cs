using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultGameController : MonoBehaviour
{
    public Canvas resultGameCanvas;

    public TMP_Text result;

    public Button replay;
    public Button toMenu;

    private bool statusGame;

    void Start() => Init();

    private void Init()
    {
        replay.onClick.AddListener(() => SceneManager.LoadScene("Game"));
        toMenu.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }

    public void SetResultGame(bool status)
    {

        SoundController.SoundClip soundClip = statusGame ? SoundController.SoundClip.Win : SoundController.SoundClip.Lose;
        SoundController.instance.PlaySound(soundClip);

        resultGameCanvas.gameObject.SetActive(status);
    }

    public void SetText(bool statusGame)
    {
        this.statusGame = statusGame;
        result.text = statusGame ? "Победа" : "Поражение";
    }
}
