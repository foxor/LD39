using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

public struct Coordinate
{
    public int x;
    public int y;
}

public class ProcedeuralGeneration
{
    /* Steps:
     * 
     * 1) Randomly place 3-6 buildings (with undefined magnitudes)
     * 2) Follow regular rules, placing 0-5 roads and 3-7 power plants, randomly interleaved.  During this process, count how much power each building gets.  Place both either next to other power plant/roads or buildings exclusively.
     * 3) Invert building magnitudes (they went negative while placing power plants)
     * 4) If any building got 0, remove it
     * 5) Randomly place 0-10 boulders
     * 6) For each road, choose randomly to: leave it on the map, put it in the inventory, or place it under a power plant
     * 7) Choose randomly 0-2 plants and place a sun token under them.  Increase the magnitude of a random building when you do this.
     * 8) Remove the power plants
     * 9) (Optional) try all possible solutions to verify there is only one.  If there are multiple, restart.
     * 
     * */

    private static Coordinate FindRandomEmptyTile(Level level)
    {
        for (int i = 0; i < 20; i++)
        {
            int x = Random.Range(0, 6);
            int y = Random.Range(0, 5);
            TileInfo chosen = level.Tiles[x, y];
            if (!chosen.ContainsBuilding && !chosen.ContainsActiveLine && !chosen.ContainsBoulder && !chosen.ContainsPowerPlant)
            {
                return new Coordinate() { x = x, y = y };
            }
        }
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo chosen = level.Tiles[x, y];
                if (!chosen.ContainsBuilding && !chosen.ContainsActiveLine && !chosen.ContainsBoulder && !chosen.ContainsPowerPlant)
                {
                    return new Coordinate() { x = x, y = y };
                }
            }
        }
        throw new System.Exception("You filled up the board!");
    }

    private static IEnumerable<Coordinate> AdjacentTiles(Level level, Coordinate position)
    {
        if (position.x > 0) yield return new Coordinate() { x = position.x - 1, y = position.y };
        if (position.y > 0) yield return new Coordinate() { x = position.x, y = position.y - 1 };
        if (position.x < 5) yield return new Coordinate() { x = position.x + 1, y = position.y };
        if (position.y < 4) yield return new Coordinate() { x = position.x, y = position.y + 1 };
    }

    private static IEnumerable<Coordinate> BuildingAdjacentTiles(Level level)
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Coordinate coordinate = new Coordinate() { x = x, y = y };
                if (AdjacentTiles(level, coordinate)
                    .Where(adjacent =>
                    {
                        TileInfo adjacentTile = level.Tiles[adjacent.x, adjacent.y];
                        return adjacentTile.ContainsBuilding || adjacentTile.ContainsActiveLine || adjacentTile.ContainsPowerPlant;
                    })
                    .Any())
                {
                    yield return coordinate;
                }
            }
        }
    }

    private static IEnumerable<TileInfo> FindArbitrary(Level level, Func<TileInfo, bool> qualificationTest)
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo tile = level.Tiles[x, y];
                if (qualificationTest(tile))
                {
                    yield return tile;
                }
            }
        }
    }

    private static void DoSomethingWithLine(Level level, TileInfo line)
    {
        float roll = Random.Range(0f, 1f);
        if (roll < 0.33f) // leave it on the map
        {
            return;
        }
        else if (roll < 0.66f) // return to inventory
        {
            line.ContainsActiveLine = false;
            level.LinesLeft++;
        }
        else
        {
            IEnumerable<TileInfo> validPlacements = FindArbitrary(level, tile => tile.ContainsPowerPlant && !tile.ContainsInactiveLine);
            if (validPlacements.Any()) // If there aren't any, just leave this line as an active line on the map
            {
                line.ContainsActiveLine = false;
                validPlacements.Random().ContainsInactiveLine = true;
            }
        }
    }

    public static Level Generate()
    {
        Level level = new Level();

        for (int i = Random.Range(3, 7); i > 0; i--)
        {
            Coordinate emptySpot = FindRandomEmptyTile(level);
            level.Tiles[emptySpot.x, emptySpot.y].ContainsBuilding = true;
        }

        level.LinesLeft = Random.Range(0, 5);
        level.PlantsLeft = Random.Range(3, 7);

        while (level.LinesLeft > 0 || level.PlantsLeft > 0)
        {
            if (level.PlantsLeft == 0)
            {
                level.LinesLeft = 0;
                continue;
            }
            Coordinate place = BuildingAdjacentTiles(level).Random();
            float plantCeiling = level.PlantsLeft / ((float)(level.PlantsLeft + level.LinesLeft));
            if (Random.Range(0f, 1f) <= plantCeiling)
            {
                level.ToolSelectionIndex = 0;
            }
            else
            {
                level.ToolSelectionIndex = 1;
            }
            RulesSimulation.ProcessTileClicked(place.x, place.y, level, true);
        }
        level.ToolSelectionIndex = 0;

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo tile = level.Tiles[x, y];
                if (tile.ContainsBuilding)
                {
                    if (tile.RemainingMagnitude == 0)
                    {
                        tile.ContainsBuilding = false;
                    }
                    tile.RemainingMagnitude *= -1;
                }
            }
        }

        for (int i = Random.Range(0, 11); i > 0; i--)
        {
            Coordinate place = FindRandomEmptyTile(level);
            level.Tiles[place.x, place.y].ContainsBoulder = true;
        }

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                TileInfo tile = level.Tiles[x, y];
                if (tile.ContainsActiveLine)
                {
                    DoSomethingWithLine(level, tile);
                }
            }
        }

        IEnumerable<TileInfo> Buildings = FindArbitrary(level, tile => tile.ContainsBuilding);
        for (int i = Random.Range(0, 3); i > 0; i--)
        {
            IEnumerable<TileInfo> PowerPlants = FindArbitrary(level, tile => tile.ContainsPowerPlant && !tile.ContainsInactiveLine && !tile.ContainsSun);
            if (PowerPlants.Any())
            {
                PowerPlants.Random().ContainsSun = true;
                Buildings.Random().RemainingMagnitude++;
            }
        }

        foreach (TileInfo tile in FindArbitrary(level, tile => tile.ContainsPowerPlant))
        {
            tile.ContainsPowerPlant = false;
            level.PlantsLeft++;
        }

        return level;
    }
}