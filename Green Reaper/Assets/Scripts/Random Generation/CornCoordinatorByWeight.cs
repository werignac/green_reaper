using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CornCoordinatorByWeight : MonoBehaviour
{
    public BinaryGoLTilemapGenerator weightGenerator;
    public float TileMapXScaleFactor;
    public float TileMapYScaleFactor;
    public Tilemap cornTileMap;

    [SerializeField]
    private uint blurIterations = 1;

    private float[,] weightMap;


    [SerializeField]
    private Tilemap background;
    [SerializeField]
    private Tile foregroundTile;
    [SerializeField]
    private RuleTile backgroundTile;

    [SerializeField]
    private GameObject corn1;
    [SerializeField]
    private GameObject corn2;
    [SerializeField]
    private GameObject corn3;


    [SerializeField, Range(0, 1)]
    private float sewThreshold = 0.2f;
    [SerializeField, Range(0, 1)]
    private float corn1Threshold = 0.4f;
    [SerializeField, Range(0, 1)]
    private float corn2Threshold = 0.6f;
    [SerializeField, Range(0, 1)]
    private float corn3Threshold = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        int[,] intTempMap = weightGenerator.RunSimulation();
        weightMap = GaussianBlur.FloaterizeMap(ref intTempMap);

        for (int i = 0; i < blurIterations; i++)
            weightMap = GaussianBlur.BlurMap(ref weightMap);

        PaintTiles();
        PaintCorn();
    }

    private void PaintTiles()
    {
        // The x and y positions need to be centered to the middle of the tileMap.
        Vector3Int centerOffset = new Vector3Int(weightGenerator.tmWidth / 2, weightGenerator.tmHeight / 2, 0);

        for (int x = 0; x < weightMap.GetLength(0); ++x)
        {
            for (int y = 0; y < weightMap.GetLength(1); ++y)
            {
                Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

                // If the tile is alive, place foreground tile.
                if (weightMap[x, y] > sewThreshold)
                    background.SetTile(centeredPosition, foregroundTile);
                else// Fill the background with the background tile.
                    background.SetTile(centeredPosition, backgroundTile);
            }
        }
    }

    private void PaintCorn()
    {
        // Not sure why I need these, but the position is off without them.
        // Has something to do with the x and y values of the tilemap.
        float xOffset = weightMap.GetLength(0) / 2 * TileMapXScaleFactor;
        float yOffset = weightMap.GetLength(1) / 2 * TileMapYScaleFactor;

        for (int x = 0; x < weightMap.GetLength(0); x++)
        {
            for (int y = 0; y < weightMap.GetLength(1); y++)
            {
                // Used to index the tilemap to find the world position of the cell.
                Vector3 worldCoordinate = cornTileMap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                float xPos = -worldCoordinate.x + xOffset;
                float yPos = -worldCoordinate.y + yOffset;

                // The x and y positions need to be centered to the middle of the tileMap.
                Vector3 centeredPosition = new Vector3(xPos, yPos, 0);

                if (weightMap[x, y] > sewThreshold)
                {
                    if (weightMap[x, y] > corn3Threshold)
                        Instantiate(corn3, centeredPosition, Quaternion.identity);
                    else if (weightMap[x, y] > corn2Threshold)
                        Instantiate(corn2, centeredPosition, Quaternion.identity);
                    else if (weightMap[x, y] > corn1Threshold)
                        Instantiate(corn1, centeredPosition, Quaternion.identity);
                }
            }
        }
    }
}
