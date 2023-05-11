using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public Canvas pauseCanvas;
    public Button buttonCanvas;

    public Button resume;
    public Button restart;
    public Button sound;
    public Button menu;


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        resume.onClick.AddListener(() => SetPause(false));
        buttonCanvas.onClick.AddListener(() => SetPause(false));

        restart.onClick.AddListener(() =>SceneManager.LoadScene(gameObject.scene.name));
        menu.onClick.AddListener(() => SceneManager.LoadScene("Menu"));
    }

    public void SetPause(bool status)
    {
        pauseCanvas.gameObject.SetActive(status);
    }
}
