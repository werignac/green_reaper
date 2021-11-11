using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

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

    [SerializeField]
    private bool drawPaths = false;

    [SerializeField]
    private float minPathWidth;

    [SerializeField]
    private float maxPathWidth;

    // Start is called before the first frame update
    void Start()
    {
        int[,] intTempMap = weightGenerator.RunSimulation();
        weightMap = GaussianBlur.FloaterizeMap(ref intTempMap);

        for (int i = 0; i < blurIterations; i++)
            weightMap = GaussianBlur.BlurMap(ref weightMap);


        if (drawPaths)
        {
            GeneratePaths();
        }

        PaintTiles();
        PaintCorn();
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
            float randomAngle = UnityEngine.Random.Range(-15f, 15f);
            // Random position on the perimeter of each circle to end the path.
            Vector2 startPos = Path.RotateVector2(pathDirection, randomAngle) * center.radius + startDir;
            
            randomAngle = UnityEngine.Random.Range(-5f, 5f);
            Vector2 endPos = Path.RotateVector2(-pathDirection, randomAngle) * orbit.radius + endDir;

            // Radius of the path or half the width of the path. Used to mark tiles for clearing.
            float boundCheckRadius = UnityEngine.Random.Range(minPathWidth, maxPathWidth);
            paths.Add(new Path(startPos, startDir, endPos, endDir, boundCheckRadius));
        }

        RemoveByCircles(circles, weightMap);
        RemoveByPaths(paths, weightMap);
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
        Func<Vector2, bool> canRemove = (Vector2 pos) =>{
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
                IEnumerator<Vector2> enumerator = p.Iterate(0.95f);
                while(enumerator.MoveNext())
                {
                    Vector2 point = enumerator.Current;
                    if (new Circle(point, p.radius).InRange(pos))
                        return true;
                }
            }
            return false;
        };

        RemoveByCondition(canRemove, weightMap);
    }
}
