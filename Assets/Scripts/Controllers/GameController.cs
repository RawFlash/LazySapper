using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public TileController tilePrefab;

    public Canvas tileCanvas;

    public static Difficulty difficulty;

    static TileController[,] tiles;

    public int MinesRemaining
    {
        get => _minesRemaining;
        set
        {
            _minesRemaining = value;
            SetupBombsCount(_minesRemaining);
        }
    }

    private int _minesRemaining;

    public ActionController actionController;

    public PauseController pauseController;

    public ResultGameController resultGameController;

    public ADSActionController aDSActionController;

    public Button pause, showADSAction;

    public static UnityAction<bool> EndGame;

    public TMP_Text bombsCount;

    public bool isFirstOpen;

    /// <summary>
    /// Высота и ширина одной ячейки
    /// </summary>
    static float tileWidth;
    static float tileHeight;

    void Start()
    {
        instance = this;
        Init();
    }

    public void Init()
    {
        difficulty ??= Difficulty.Easy;

        tileHeight = tileCanvas.GetComponent<RectTransform>().rect.height / difficulty.countLine;
        tileWidth = tileCanvas.GetComponent<RectTransform>().rect.width / difficulty.countColumn;

        CreateTiles();

        MinesRemaining = difficulty.countMines;

        TileController.needOpenTiles = new();

        pause.onClick.AddListener(() => pauseController.SetPause(true));

        SetupBombsCount(difficulty.countMines);

        EndGame += (status) =>
        {
            if (!status)
            {
                OpenBombs();
            }
        };

        EndGame += (status) => StartCoroutine(OpenResultGame());
        EndGame += (status) => pause.interactable = false;
        EndGame += resultGameController.SetText;

        isFirstOpen = true;

        showADSAction.onClick.AddListener(aDSActionController.Show);

        ADSController.instance.InitRewardAds();
    }

    public void ActiveRewardButton()
    {
        if (!isFirstOpen && IronSource.Agent.isRewardedVideoAvailable() )
        {
            showADSAction.gameObject.SetActive(true);
        }
    }

    void CreateTiles()
    {
        tiles = new TileController[difficulty.countColumn, difficulty.countLine];

        float xOffset = 0;
        float yOffset = 0;

        for (int tileIndexX = 0; tileIndexX < difficulty.countColumn; tileIndexX++)
        {
            for (int tileIndexY = 0; tileIndexY < difficulty.countLine; tileIndexY++)
            {
                TileController newTile = Instantiate(tilePrefab, new Vector3(), new Quaternion(), tileCanvas.transform);

                newTile.Init();

                newTile.tile.position = new Vector2(tileIndexX, tileIndexY);

                newTile.GetComponent<RectTransform>().anchoredPosition = new Vector3(tileWidth / 2 + xOffset, tileHeight / 2 + yOffset, 0);

                newTile.GetComponent<RectTransform>().sizeDelta = new Vector2(tileWidth, tileHeight);

                tiles[tileIndexX, tileIndexY] = newTile;

                yOffset += tileHeight;
            }

            yOffset = 0;
            xOffset += tileWidth;
        }
    }

    public void AssignMines(TileController tileController)
    {
        bool isFirstTileIsEmprty = false;

        while (!isFirstTileIsEmprty)
        {
            ClearAllBombs();

            for (int mineIndex = 0; mineIndex < difficulty.countMines; mineIndex++)
            {
                Vector2 position = new(UnityEngine.Random.Range(0, difficulty.countColumn), UnityEngine.Random.Range(0, difficulty.countLine));

                while (tiles[(int)position.x, (int)position.y].tile.isMined)
                {
                    position = new Vector2(UnityEngine.Random.Range(0, difficulty.countColumn), UnityEngine.Random.Range(0, difficulty.countLine));
                }

                tiles[(int)position.x, (int)position.y].tile.isMined = true;
            }

            isFirstTileIsEmprty = tileController.CalculateMines() == 0;
        }

        GameController.instance.isFirstOpen = false;
    }

    private void ClearAllBombs()
    {
        foreach (TileController tileController in tiles)
        {
            tileController.tile.isMined = false;
        }
    }

    private void OpenBombs()
    {
        //Раскрываем все нераскрытые бомбы
        foreach (TileController currentTile in tiles)
        {
            if (currentTile.tile.isMined)
            {
                currentTile.SetStatusButton(false);
                currentTile.ShowOnlyFlag(TileFlag.bomb);
            }
        }
    }

    /// <summary>
    /// Возвращает ячейки по соседству по перпендикуляру и по диагонали
    /// </summary>
    /// <param name="currentTile"></param>
    /// <param name="onlyPerpendicular">Только по перпендикудяру(горизонтали и вертикали)</param>
    /// <returns></returns>
    public List<TileController> GetAdjacentTiles(Tile currentTile, bool onlyPerpendicular = false)
    {
        List<TileController> tiles = new();

        for (int offsetX = -1; offsetX < 2; offsetX++)
        {
            for (int offsetY = -1; offsetY < 2; offsetY++)
            {
                if (onlyPerpendicular)
                {
                    if (offsetX + offsetY == 1 || offsetX + offsetY == -1)
                    {
                        try
                        {
                            tiles.Add(GetTileController(new Vector2(currentTile.position.x + offsetX, currentTile.position.y + offsetY)));
                        }
                        catch { }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    try
                    {
                        tiles.Add(GetTileController(new Vector2(currentTile.position.x + offsetX, currentTile.position.y + offsetY)));
                    }
                    catch { }
                }
            }
        }

        return tiles;
    }

    private TileController GetTileController(Vector2 position)
    {
        return tiles[(int)position.x, (int)position.y];
    }

    public void Action(TileController controller)
    { 
        actionController.Show(controller);
    }

    private IEnumerator OpenResultGame()
    {
        yield return new WaitForSeconds(2);
        resultGameController.SetResultGame(true);
    }

    public void CheckWin()
    {
        bool isWin = false;

        if (IsAllBombsFlagged() && !IsHaveWrongFlagg())
        {
            isWin = true;
        }

        if (isWin)
        {
            EndGame?.Invoke(true);
        }

        ActiveRewardButton();
    }

    private bool IsAllBombsFlagged()
    {
        bool ret = true;
        foreach (TileController tile in tiles)
        {
            if (tile.tile.isMined && tile.GetTileState() != TileState.flagged)
            {
                ret = false;
            }
        }
        return ret;
    }

    private bool IsHaveWrongFlagg()
    {
        bool ret = false;
        foreach (TileController tile in tiles)
        {
            if (!tile.tile.isMined && tile.GetTileState() == TileState.flagged)
            {
                ret = true;
            }
        }
        return ret;
    }

    private void OnDestroy()
    {
        EndGame = null;
    }

    private void SetupBombsCount(int count)
    {
        bombsCount.text = count.ToString();
    }

    public void RewardAds()
    {
        TileController findedTile = null;
        foreach (TileController tile in tiles)
        {
            if (tile.tile.isMined && !tile.tile.isFlag && tile.tileButton.GetComponent<Image>().color != Color.red)
            {
                findedTile = tile;
                break;
            }
        }

        if (findedTile != null)
        {
            findedTile.tileButton.GetComponent<Image>().color = Color.red;
        }
    }
}

public class Difficulty
{
    public int countTiles;
    public int countColumn;
    public int countLine;
    public int countMines;

    /// <summary>
    /// Время на игру в секундах
    /// </summary>
    public int timeOnGame;

    public Difficulty(int countColumn, int countLine, int countMines, int timeOnGame)
    {
        countTiles = countColumn * countLine;
        this.countColumn = countColumn;
        this.countLine = countLine;
        this.countMines = countMines;
        this.timeOnGame = timeOnGame;
    }

    public static Difficulty Easy => new(6, 13, 10, 600);

    public static Difficulty Meduim => new(12, 21, 40, 600);

    public static Difficulty Hard => new(18, 28, 99, 600);
}