using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGradientVisualizer : MonoBehaviour
{
    public BinaryGoLTilemapGenerator weightGenerator;
    public ValueVisualizerBehaviour<float> visualizer;
    public float TileMapXScaleFactor;
    public float TileMapYScaleFactor;
    public Tilemap cornTileMap;

    [SerializeField]
    private uint blurIterations = 1;

    private float[,] weightMap;

    // Start is called before the first frame update
    void Start()
    {
        int [,] intTempMap = weightGenerator.RunSimulation();
        weightMap = GaussianBlur.FloaterizeMap(ref intTempMap);

        for (int i = 0; i < blurIterations; i++)
            weightMap = GaussianBlur.BlurMap(ref weightMap);

        PlaceVisualizers();
    }

    /// <summary>
    /// Places corn based on the cornMap.
    /// Corn is placed into the cornTileMap.
    /// </summary>
    void PlaceVisualizers()
    {
        for (int x = 0; x < weightGenerator.tmWidth; x++)
        {
            for (int y = 0; y < weightGenerator.tmHeight; y++)
            {
                // Used to index the tilemap to find the world position of the cell.
                Vector3 worldCoordinate = cornTileMap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                // Not sure why I need these, but the position is off without them.
                // Has something to do with the x and y values of the tilemap.
                float xOffset = weightGenerator.tmWidth / 2 * TileMapXScaleFactor;
                float yOffset = weightGenerator.tmHeight / 2 * TileMapYScaleFactor;

                float xPos = -worldCoordinate.x + xOffset;
                float yPos = -worldCoordinate.y + yOffset;

                // The x and y positions need to be centered to the middle of the tileMap.
                Vector3 centeredPosition = new Vector3(xPos, yPos, 0);

                ValueVisualizer<float> visual = Instantiate(visualizer.gameObject, centeredPosition, Quaternion.identity).GetComponent<ValueVisualizerBehaviour<float>>();

                float weight = weightMap[x, y];
                visual.Visualize(weight);
            }
        }
    }
}
