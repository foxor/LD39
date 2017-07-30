using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    public bool ContainsBuilding;
    public bool ContainsPowerPlant;
    public int RemainingMagnitude;
    public bool ContainsActiveLine;
    public bool ContainsInactiveLine;
    public bool ContainsBoulder;
    public bool ContainsSun;
    public bool SuppressPowerPlant;

    public static TileInfo Parse(string source)
    {
        TileInfo tileInfo = new TileInfo();
        if (source == "l")
        {
            tileInfo.ContainsInactiveLine = true;
        }
        else if (source == "L")
        {
            tileInfo.ContainsActiveLine = true;
        }
        else if (source == "b")
        {
            tileInfo.ContainsBoulder = true;
        }
        else if (source == "p")
        {
            tileInfo.ContainsSun = true;
        }
        else
        {
            tileInfo.RemainingMagnitude = int.Parse(source);
            tileInfo.ContainsBuilding = tileInfo.RemainingMagnitude > 0;
        }
        return tileInfo;
    }
}

public class Level
{
    public int Number;
    public int PlantsLeft;
    public int LinesLeft;
    public int PowerLeft;
    public int ToolSelectionIndex;
    public TileInfo[,] Tiles = new TileInfo[6, 5];

    public static Level Active;

    public Level()
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Tiles[x, y] = new TileInfo();
            }
        }
    }

    public static Level ReadLevel(int index)
    {
        Level level = null;
        if (index > 11)
        {
            level = ProcedeuralGeneration.Generate();
        }
        else
        {
            level = new Level();
            TextAsset levelText = Resources.Load<TextAsset>("lvl" + index);
            string[] lines = levelText.text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string[] metadata = lines[0].Split(new char[] { ' ' });
            level.PlantsLeft = int.Parse(metadata[0]);
            level.LinesLeft = int.Parse(metadata[1]);

            for (int y = 0; y < 5; y++)
            {
                string[] lineInfo = lines[y + 1].Split(new char[] { ' ' });
                for (int x = 0; x < 6; x++)
                {
                    level.Tiles[x, y] = TileInfo.Parse(lineInfo[x]);
                }
            }
        }

        level.Number = index;
        Active = level;

        return level;
    }
}