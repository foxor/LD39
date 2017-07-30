using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver
{
    public static event Action Win = () => { };
    public static event Action<string> Lose = (_) => { };

    public static void CheckGameOver()
    {
        bool anyUnpowered = false;
        bool lost = false;

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo tile = Level.Active.Tiles[x, y];
                anyUnpowered |= tile.ContainsBuilding && tile.RemainingMagnitude > 0;
                lost |= tile.ContainsBuilding && tile.RemainingMagnitude < 0;
            }
        }

        if (lost)
        {
            Lose("Wasting extra power on a building!");
        }
        else if (!anyUnpowered)
        {
            Win();
        }
        else if (Level.Active.PlantsLeft == 0 && Level.Active.PowerLeft == 0)
        {
            Lose("Ran out of power!");
        }
    }
}