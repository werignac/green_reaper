using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

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

    [SerializeField]
    private bool drawPaths = true;
    [SerializeField]
    private float minPathWidth;

    [SerializeField]
    private float maxPathWidth;

    // Start is called before the first frame update
    void Start()
    {
        int [,] intTempMap = weightGenerator.RunSimulation();
        weightMap = GaussianBlur.FloaterizeMap(ref intTempMap);

        for (int i = 0; i < blurIterations; i++)
            weightMap = GaussianBlur.BlurMap(ref weightMap);

        if (drawPaths)
        {
            GeneratePaths();
        }

        PlaceVisualizers();
    }

    private void GeneratePaths()
    {
        // Center circle always around player start.
        Circle center = new Circle(new Vector2(weightMap.GetLength(0), weightMap.GetLength(1)) / 2f, 4.5f);
        List<Circle> circles = new List<Circle>();

        int orbits = UnityEngine.Random.Range(3, 10);

        for (int i = 0; i < orbits; i++)
        {
            circles.Add(new Circle(new Vector2(weightMap.GetLength(0) * UnityEngine.Random.value, weightMap.GetLength(1) * UnityEngine.Random.value), UnityEngine.Random.Range(2f, 6f)));
        }

        //Center must come after orbits to not mess with path generation.
        circles.Add(center);

        List<Path> paths = new List<Path>();

        // For each of the orbits, create and store a path. 
        for (int i = 0; i < orbits; i++)
        {
            Circle orbit = circles[i];

            // Center of each circle to connect the path to. Used to direct path when connecting to either circle's perimeter.
            Vector2 startDir = center.center;
            Vector2 endDir = orbit.center;

            // Direction from start to end.
            Vector2 pathDirection = (endDir - startDir).normalized;

            // Random angle to offset the end point on the permiter of each circle. Generally prevents paths from becoming straight. 
            float randomAngle = UnityEngine.Random.Range(-15f, 15f) * Mathf.Deg2Rad;
            // Random position on the perimeter of each circle to end the path.
            Vector2 startPos = Path.RotateVector2(pathDirection, randomAngle) * center.radius + startDir;

            randomAngle = UnityEngine.Random.Range(-15f, 15f) * Mathf.Deg2Rad;
            Vector2 endPos = Path.RotateVector2(-pathDirection, randomAngle) * orbit.radius + endDir;

            // Radius of the path or half the width of the path. Used to mark tiles for clearing.
            float boundCheckRadius = UnityEngine.Random.Range(minPathWidth, maxPathWidth);
            paths.Add(new Path(startPos, startDir, endPos, endDir, boundCheckRadius));
        }

        RemoveByCircles(circles, weightMap);
        RemoveByPaths(paths, weightMap);
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

    private static void RemoveByCondition(Func<Vector2, bool> canRemove, float[,] weightMap)
    {
        for (int x = 0; x < weightMap.GetLength(0); x++)
            for (int y = 0; y < weightMap.GetLength(1); y++)
                // weightMap[x, y] != 0, does not change the behavior, but does increase performance.
                if (weightMap[x, y] != 0 && canRemove(new Vector2(x, y)))
                    weightMap[x, y] = 0;
    }

    private static void RemoveByCircles(IEnumerable<Circle> circles, float[,] weightMap)
    {
        Func<Vector2, bool> canRemove = (Vector2 pos) => {
            foreach (Circle c in circles)
                if (c.InRange(pos))
                    return true;
            return false;
        };

        RemoveByCondition(canRemove, weightMap);
    }

    private static void RemoveByPaths(IEnumerable<Path> paths, float[,] weightMap)
    {
        Func<Vector2, bool> canRemove = (Vector2 pos) => {
            foreach (Path p in paths)
            {
                if (p.InRange(pos, 4))
                    return true;
            }
            return false;
        };

        RemoveByCondition(canRemove, weightMap);
    }
}
