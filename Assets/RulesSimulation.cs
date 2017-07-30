using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulesSimulation
{
    public static void Setup()
    {
        tileButton.ClickedTile += OnTileClicked;
    }

    protected static void OnTileClicked(int x, int y)
    {
        ProcessTileClicked(x, y, Level.Active, false);
    }

    public static void ProcessTileClicked(int x, int y, Level level, bool simulating)
    {
        TileInfo clicked = level.Tiles[x, y];

        if (clicked.ContainsPowerPlant || clicked.ContainsActiveLine || clicked.ContainsBoulder)
        {
            return;
        }

        if (level.ToolSelectionIndex == 0 && level.PlantsLeft > 0 && !clicked.ContainsBuilding) // Power Plant
        {
            int powerDelivered = level.PlantsLeft;
            clicked.ContainsPowerPlant = true;
            level.PlantsLeft--;

            bool[,] touchedTiles = new bool[6, 5];
            RecursivePowerFlow(x, y, touchedTiles, level);
            ApplyPower(powerDelivered, touchedTiles, level, simulating);
            if (!simulating)
            {
                BoltController.FireBolts(x, y, touchedTiles);
            }

            if (clicked.ContainsInactiveLine)
            {
                clicked.ContainsInactiveLine = false;
                level.LinesLeft++;
            }
            if (clicked.ContainsSun)
            {
                clicked.ContainsSun = false;
                level.PowerLeft++;
            }
        }
        else if (level.ToolSelectionIndex == 1 && level.LinesLeft > 0 && !clicked.ContainsBuilding) // Power Line
        {
            clicked.ContainsActiveLine = true;
            level.LinesLeft--;
        }
        else if (level.ToolSelectionIndex == 2 && level.PowerLeft > 0 && clicked.ContainsBuilding) // Sun
        {
            clicked.RemainingMagnitude--;
            level.PowerLeft--;
        }

        if (!simulating)
        {
            GameOver.CheckGameOver();
        }
    }

    private static void RecursivePowerFlow(int x, int y, bool[,] touchedTiles, Level level)
    {
        if (x < 0 || x >= 6 || y < 0 || y >= 5 || touchedTiles[x, y])
        {
            return;
        }

        touchedTiles[x, y] = true;

        TileInfo tileInfo = level.Tiles[x, y];
        if (tileInfo.ContainsActiveLine || tileInfo.ContainsPowerPlant)
        {
            RecursivePowerFlow(x - 1, y, touchedTiles, level);
            RecursivePowerFlow(x + 1, y, touchedTiles, level);
            RecursivePowerFlow(x, y - 1, touchedTiles, level);
            RecursivePowerFlow(x, y + 1, touchedTiles, level);
        }
    }

    private static void ApplyPower(int powerMagnitude, bool[,] touchedTiles, Level level, bool simulating)
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo tileInfo = level.Tiles[x, y];
                if (touchedTiles[x, y] && tileInfo.ContainsBuilding && (simulating || tileInfo.RemainingMagnitude != 0))
                {
                    tileInfo.RemainingMagnitude -= powerMagnitude;
                }
            }
        }
    }
}