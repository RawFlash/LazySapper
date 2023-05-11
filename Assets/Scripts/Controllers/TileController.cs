using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileController : MonoBehaviour
{
    public Tile tile;

    public Image image;
    public TMP_Text text;

    public Sprite flag;
    public Sprite bomb;

    public Button tileButton;

    //Лист для рекурсивного раскрытия ячеек, сначала добавляем сюда все ячейки которые надо раскрыть, потом раскрываем
    public static List<TileController> needOpenTiles;

    public void Init()
    {
        tile = new();
        tileButton.onClick.AddListener(ShowVariantAction);
    }

    public void ShowVariantAction()
    {
        ActionController.tileAction += Action;

        GameController.instance.Action(this);
    }

    public void Action(TypeAction action)
    {
        switch (action)
        {
            case TypeAction.open:

                if (GameController.instance.isFirstOpen)
                {
                    GameController.instance.AssignMines(this);
                }

                if (tile.isMined)
                {
                    Explode();
                    tile.state = TileState.explosion;
                }
                else
                {
                    TryOpenTile();
                    SetStatusButton(false);
                    tile.state = TileState.open;
                }
                break;

            case TypeAction.flag:
                if (GetTileState() == TileState.flagged)
                {
                    tile.state = TileState.close;
                    ShowOnlyFlag(TileFlag.none);
                    GameController.instance.MinesRemaining += 1;
                    SetAlphaButton(true);
                }
                else
                {
                    tile.state = TileState.flagged;
                    ShowOnlyFlag(TileFlag.flag);
                    GameController.instance.MinesRemaining -= 1;
                    SetAlphaButton(false);
                }
                break;
        }

        GameController.instance.CheckWin();
    }

    public void ShowOnlyFlag(TileFlag typeflag)
    {
        ClearFlags();

        switch (typeflag)
        {
            case TileFlag.flag:
                image.gameObject.SetActive(true);
                image.sprite = flag;
                break;

            case TileFlag.bomb:
                image.gameObject.SetActive(true);
                image.sprite = bomb;
                break;

            case TileFlag.text:
                text.gameObject.SetActive(true);
                text.text = CalculateMines().ToString();
                break;
        }
    }

    private void ClearFlags()
    {
        image.gameObject.SetActive(false);
        text.gameObject.SetActive(false);
    }

    /// <summary>
    /// Подсчитвыает кол-во мин вокруг ячейки
    /// </summary>
    public int CalculateMines()
    {
        tile.adjacentMines = 0;

        foreach (TileController currentTile in GameController.instance.GetAdjacentTiles(tile))
        {
            if (currentTile.tile.isMined)
            {
                tile.adjacentMines += 1;
            }
        }

        return tile.adjacentMines;
    }

    void TryOpenTile()
    {
        if (!tile.isMined)
        {
            SoundController.instance.PlaySound(SoundController.SoundClip.Click);

            if (tile.adjacentMines == 0)
            {
                RecursiveAddToNeedOpenList();
            }

        }
        else
        {
            Explode();
        }
    }

    void Explode()
    {
        SoundController.instance.PlaySound(SoundController.SoundClip.Bomb);
        ShowOnlyFlag(TileFlag.bomb);
        GameController.EndGame?.Invoke(false);
    }

    public void RecursiveAddToNeedOpenList()
    {
        if (needOpenTiles.Contains(this))
        {
            return;
        }

        if (!tile.isMined)
        {
            needOpenTiles.Add(this);

            if (CalculateMines() == 0)
            {
                List<TileController> tileControllers = GameController.instance.GetAdjacentTiles(tile);

                foreach (TileController tileController in tileControllers)
                {

                    if (tileController.CalculateMines() > 0)
                    {
                        tileController.OpenTile();
                    }
                    else
                    {
                        tileController.RecursiveAddToNeedOpenList();
                    }
                }
            }
        }

        if (needOpenTiles.Find(tileController => !needOpenTiles.Contains(tileController)) != null)
        {
            needOpenTiles.Find(tileController => !needOpenTiles.Contains(tileController)).RecursiveAddToNeedOpenList();
        }
        else
        {
            foreach (TileController tileController in needOpenTiles)
            {
                tileController.OpenTile();
            }
        }
    }

    private void OpenTile()
    {
        SetStatusButton(false);

        if (CalculateMines() == 0)
        {
            ShowOnlyFlag(TileFlag.none);
        }
        else
        {
            ShowOnlyFlag(TileFlag.text);
        }
    }

    public void SetStatusButton(bool status) => tileButton.gameObject.SetActive(status);

    private void SetAlphaButton(bool isVisible)
    {
        var colors = tileButton.colors;
        int newAlpha = isVisible ? 1 : 0;
        colors.normalColor = new Color(colors.normalColor.r, colors.normalColor.g, colors.normalColor.b, newAlpha);
        tileButton.colors = colors;
    }

    public bool IsButtonActive() => tileButton.gameObject.activeSelf;

    public TileState GetTileState() => tile.state;
}

[Serializable]
public class Tile
{
    public Vector2 position;
    public bool isMined;
    public bool isFlag;
    public TileState state;

    public int adjacentMines;
}

public enum TileState
{
    close,
    flagged,
    open,
    explosion
}

public enum TileFlag
{
    none,
    flag,
    bomb,
    text
}

public enum TypeAction
{
    open,
    flag,
    nothing
}