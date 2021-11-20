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
    public int mapBorderSize;


    [SerializeField]
    private uint blurIterations = 1;
    private float[,] weightMap;

    [SerializeField]
    private Tilemap cornTileMap;
    [SerializeField]
    private Tilemap background;
    [SerializeField]
    private Tilemap fenceTileMap;
    [SerializeField]
    private Tile foregroundTile;
    [SerializeField]
    private RuleTile backgroundTile;
    [SerializeField]
    private RuleTile fenceTile;
    [SerializeField]
    private GameObject house;
    [SerializeField]
    private Vector3 housePosition;

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

    [SerializeField, Range(0, 5)]
    private float minPathWidth;
    [SerializeField, Range(0, 5)]
    private float maxPathWidth;

    [SerializeField, Range(3, 10)]
    private int minOrbits;
    [SerializeField, Range(3, 10)]
    private int maxOrbits;

    [SerializeField, Range(0, 10)]
    private float minOrbitRadius = 4;
    [SerializeField, Range(0, 10)]
    private float maxOrbitRadius = 10;

    [SerializeField, Range(0, 1000)]
    private float minOrbitDistance = 50f;
    [SerializeField, Range(0, 1000)]
    private float maxOrbitDistance = 100f;


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
        PaintFences();
        PaintCorn();
        
        // Paint house.
        Instantiate(house, housePosition, Quaternion.identity);
    }

    private void GeneratePaths()
    {
        // Center circle always around player start.
        Circle center = new Circle(new Vector2(weightMap.GetLength(0), weightMap.GetLength(1)) / 2f, 5);
        List<Circle> circles = new List<Circle>();

        int orbits = UnityEngine.Random.Range(minOrbits, maxOrbits);

        float randomAngleFromCenter = UnityEngine.Random.Range(0, 2f * Mathf.PI);
        float meanAngleAdd = 2 * Mathf.PI / orbits;

        for (int i = 0; i < orbits; i++)
        {
            randomAngleFromCenter += UnityEngine.Random.Range(meanAngleAdd * 0.5f, meanAngleAdd * 1.5f);

            Vector2 orbitCenter = Vector2.up * UnityEngine.Random.Range(minOrbitDistance, maxOrbitDistance);
            orbitCenter = Path.RotateVector2(orbitCenter, randomAngleFromCenter);

            //Making the spawn region into an ellipse.
            float ratio = weightMap.GetLength(1) / (float)weightMap.GetLength(0);
            orbitCenter = new Vector2(orbitCenter.x, orbitCenter.y * ratio);

            circles.Add(new Circle(orbitCenter + center.center, UnityEngine.Random.Range(minOrbitRadius, maxOrbitRadius)));
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
            float boundCheckRadius = UnityEngine.Random.Range(minPathWidth, Math.Min(maxPathWidth, orbit.radius / 2));
            paths.Add(new Path(startPos, startDir, endPos, endDir, boundCheckRadius));
        }

        RemoveByCircles(circles, weightMap);
        RemoveByPaths(paths, weightMap);
    }

    private void PaintTiles()
    {
        // The x and y positions need to be centered to the middle of the tileMap.
        Vector3Int centerOffset = new Vector3Int(weightGenerator.tmWidth / 2, weightGenerator.tmHeight / 2, 0);

        // Paints the entire background including the border tiles.
        for (int x = -mapBorderSize; x < weightMap.GetLength(0) + mapBorderSize; ++x)
        {
            for (int y = -mapBorderSize; y < weightMap.GetLength(1) + mapBorderSize; ++y)
            {
                Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

                // Checks for and paints border tiles. Prevents index out of bounds on the weightMap.
                if (x < 0 || x >= weightMap.GetLength(0) || y < 0 || y >= weightMap.GetLength(1))
                {
                    background.SetTile(centeredPosition, backgroundTile);
                    continue;
                }

                // If the tile is alive, place foreground tile.
                if (weightMap[x, y] > sewThreshold)
                    background.SetTile(centeredPosition, foregroundTile);
                else// Fill the background with the background tile.
                    background.SetTile(centeredPosition, backgroundTile);
            }
        }
    }

    /// <summary>
    /// The coordinates of the tiles are flipped in both the x and y values.
    /// This is required for proper centering, but means that (0,0) is the top right of the tilemap.
    /// Comments describe which side they paint after the flipping has occured.
    /// </summary>
    private void PaintFences()
    {
        // The x and y positions need to be centered to the middle of the tileMap.
        Vector3Int centerOffset = new Vector3Int(weightGenerator.tmWidth / 2, weightGenerator.tmHeight / 2, 0);

        int x, y;

        // Paints the right side of the fence vertically.  
        for (y = -1, x = -1; y < weightMap.GetLength(1) + 1; ++y)
        {
            Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

            fenceTileMap.SetTile(centeredPosition, fenceTile);
        }

        // Paints the left side of the fence vertically.  
        for (y = -1, x = weightMap.GetLength(0); y < weightMap.GetLength(1) + 1; ++y)
        {
            Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

            fenceTileMap.SetTile(centeredPosition, fenceTile);
        }

        // Paints the top side of the fence Horizontally.  
        for (y = -1, x = -1; x < weightMap.GetLength(0) + 1; ++x)
        {
            Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

            fenceTileMap.SetTile(centeredPosition, fenceTile);
        }

        // Paints the top side of the fence Horizontally.  
        for (y = weightMap.GetLength(1), x = -1; x < weightMap.GetLength(0) + 1; ++x)
        {
            Vector3Int centeredPosition = new Vector3Int(-x, -y, 0) + centerOffset;

            fenceTileMap.SetTile(centeredPosition, fenceTile);
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
        Func<Vector2, bool> canRemove = (Vector2 pos) =>
        {
            foreach (Circle c in circles)
                if (c.InRange(pos))
                    return true;
            return false;
        };

        RemoveByCondition(canRemove, weightMap);
    }

    private static void RemoveByPaths(IEnumerable<Path> paths, float[,] weightMap)
    {
        Func<Vector2, bool> canRemove = (Vector2 pos) =>
        {
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
