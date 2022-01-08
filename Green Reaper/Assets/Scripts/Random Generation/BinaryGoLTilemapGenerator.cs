using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class BinaryGoLTilemapGenerator : MonoBehaviour
{
    [Range(0, 100)]
    public int initialChance;

    [Range(1, 8)]
    public int birthLimit;

    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 10)]
    public int numberOfRepititons;


    public Vector3Int tileMapSize;
    public Tilemap foreground;
    public Tilemap background;

    public int tmHeight { get; private set; }
    public int tmWidth { get; private set; }


    /// <summary>
    /// Runs the simulation using the public variables to populate the given tilemaps.
    /// </summary>
    /// <returns>The tilemap generated in the process.</returns>
    public int[,] RunSimulation()
    {
        ClearMap();

        tmWidth = tileMapSize.x;
        tmHeight = tileMapSize.y;

        int[,] terrainMap = new int[tmWidth, tmHeight];
        initialPosition(ref terrainMap);


        for (int i = 0; i < numberOfRepititons; ++i)
            terrainMap = GenerateTilePositions(ref terrainMap);

        return terrainMap;
    }

    /// <summary>
    /// Use the public data on the script to configure the intial settings for the map.
    /// </summary>
    /// <param name="terrainMap">Tilemap to intialize.</param>
    private void initialPosition(ref int[,] terrainMap)
    {
        for (int x = 0; x < tmWidth; ++x)
        {
            for (int y = 0; y < tmHeight; ++y)
            {
                int randomValue = Random.Range(1, 101);
                bool alive = randomValue < initialChance;

                if (alive)
                {
                    terrainMap[x, y] = 1;
                }
                else
                {
                    terrainMap[x, y] = 0;
                }
            }
        }
    }

    /// <summary>
    /// Creates a new tilemap with the given tilemap as a starting point.
    /// </summary>
    /// <param name="oldMap">Tilemap to be used as a starting point.</param>
    /// <returns>Next iteration of the tilemap.</returns>
    private int[,] GenerateTilePositions(ref int[,] oldMap)
    {
        int[,] newMap = new int[tmWidth, tmHeight];
        int neighbors = 0;

        for (int x = 0; x < tmWidth; ++x)
        {
            for (int y = 0; y < tmHeight; ++y)
            {
                neighbors = CountNeighbors(x, y, ref oldMap);
                SetAliveStatusInMap(x, y, neighbors, ref newMap, ref oldMap);
            }
        }

        return newMap;
    }

    /// <summary>
    /// Counts the number of neighbors around the current tilemap.
    /// </summary>
    /// <param name="x">X index of the tile.</param>
    /// <param name="y">Y index of the tile.</param>
    /// <param name="oldMap">Copy of the old map.</param>
    /// <returns>Number of neighbors around the tile.</returns>
    public int CountNeighbors(int x, int y, ref int[,] oldMap)
    {
        BoundsInt myBounds = new BoundsInt(-1, -1, 0, 3, 3, 1);
        int neighbors = 0;

        foreach (var boundary in myBounds.allPositionsWithin)
        {
            // Current position is not a neighbor therefore it is excluded.
            if (boundary.x == 0 && boundary.y == 0) continue;

            // Exclude the values outside of the map. Only considers values within the bounds of the tile map.
            if (x + boundary.x >= 0 && x + boundary.x < tmWidth && y + boundary.y >= 0 && y + boundary.y < tmHeight)
                neighbors += oldMap[x + boundary.x, y + boundary.y];           
        }

        return neighbors;
    }

    /// <summary>
    /// Does the Conway's Game Of Life calculation for each tile in the map.
    /// </summary>
    /// <param name="x">X index of the tile.</param>
    /// <param name="y">Y index of the tile.</param>
    /// <param name="neighborCount">Number of neighbors surrounding the tile.</param>
    /// <param name="newMap">Reference to the next iteration of the map.</param>
    /// <param name="oldMap">Copy of the old map.</param>
    private void SetAliveStatusInMap(int x, int y, int neighborCount, ref int[,] newMap, ref int[,] oldMap)
    {
        if (oldMap[x, y] == 1)
        {
            if (neighborCount < deathLimit)
                newMap[x, y] = 0;
            else
                newMap[x, y] = 1;
        }

        if (oldMap[x, y] == 0)
        {
            if (neighborCount > birthLimit)
                newMap[x, y] = 1;
            else
                newMap[x, y] = 0;
        }
    }

    /// <summary>
    /// Clears the tilemaps of all tiles.
    /// </summary>
    public void ClearMap()
    {
        foreground.ClearAllTiles();
        background.ClearAllTiles();
    }
}

