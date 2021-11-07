using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

/// <summary>
/// Tilemap generator based on Conway's Game Of Life.
/// </summary>
public class TileAutomation : MonoBehaviour
{
    [Range(0, 100)]
    public int initialChance;

    [Range(1, 8)]
    public int birthLimit;

    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 10)]
    public int numberOfRepititons;

    private int[,] terrainMap;
    public Vector3Int tileMapSize;

    public Tilemap foreground;
    public Tilemap background;
    public Tile foregroundTile;
    public Tile backgroundTile;

    private int tmHeight;
    private int tmWidth;


    public void RunSimulation()
    {
        ClearMap();
        tmWidth = tileMapSize.x;
        tmHeight = tileMapSize.y;

        if (terrainMap == null)
        {
            terrainMap = new int[tmWidth, tmHeight];
            initialPosition(tmWidth, tmHeight);
        }

        for (int i = 0; i < numberOfRepititons; ++i)
            terrainMap = GenerateTilePositions();

        for (int x = 0; x < tmWidth; ++x)
        {

            for (int y = 0; y < tmHeight; ++y)
            {
                // The x and y positions need to be centered to the middle of the tileMap.
                Vector3Int centeredPosition = new Vector3Int(-x + tmWidth / 2, -y + tmHeight / 2, 0);

                // If the tile is alive, place foreground tile.
                if (terrainMap[x, y] == 1)
                    foreground.SetTile(centeredPosition, foregroundTile);

                // Fill the background with the background tile.
                background.SetTile(centeredPosition, backgroundTile);
            }
        }
    }

    private void initialPosition(int width, int height)
    {
        for (int x = 0; x < width; ++x)
        {

            for (int y = 0; y < height; ++y)
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


    private int[,] GenerateTilePositions()
    {
        int[,] newMap = new int[tmWidth, tmHeight];
        int neighbors;
        BoundsInt myBounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < tmWidth; ++x)
        {

            for (int y = 0; y < tmHeight; ++y)
            {

                neighbors = 0;
                foreach (var boundary in myBounds.allPositionsWithin)
                {
                    // Current position is not a neighbor therefore it is excluded.
                    if (boundary.x == 0 && boundary.y == 0) continue;

                    // Exclude the values outside of the map. Only considers values within the bounds of the tile map.
                    if (x + boundary.x >= 0 && x + boundary.x < tmWidth && y + boundary.y >= 0 && y + boundary.y < tmHeight)
                    {
                        neighbors += terrainMap[x + boundary.x, y + boundary.y];
                    }

                    // Consider the border of the tilemap as a neighbor.
                    else
                    {
                        neighbors++;
                    }

                }

                if (terrainMap[x, y] == 1)
                {
                    if (neighbors < deathLimit)
                        newMap[x, y] = 0;
                    else
                        newMap[x, y] = 1;
                }

                if (terrainMap[x, y] == 0)
                {
                    if (neighbors < deathLimit)
                        newMap[x, y] = 1;
                    else
                        newMap[x, y] = 0;
                }
            }

        }

        return newMap;
    }

    public void ClearMap()
    {
        foreground.ClearAllTiles();
        background.ClearAllTiles();
        terrainMap = null;
    }
}
