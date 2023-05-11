using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TMP_InputField fieldWight;
    public TMP_InputField fieldHeight;
    public TMP_InputField fieldBombs;

    public Button easy;
    public Button meduim;
    public Button hard;
    public Button custom;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        fieldWight.text = "6";
        fieldHeight.text = "13";
        fieldBombs.text = "10";

        easy.onClick.AddListener(() => StartGame(Difficulty.Easy));
        meduim.onClick.AddListener(() => StartGame(Difficulty.Meduim));
        hard.onClick.AddListener(() => StartGame(Difficulty.Hard));
        custom.onClick.AddListener(() => StartGame(new Difficulty(int.Parse(fieldWight.text), int.Parse(fieldHeight.text), int.Parse(fieldBombs.text), 600)));
    }

    public void StartGame(Difficulty difficulty)
    {
        GameController.difficulty = difficulty;
        SceneManager.LoadSceneAsync("Game");
    }
}