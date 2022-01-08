using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class CornCoordinatorByWeight : MonoBehaviour
{
    public BinaryGoLTilemapGenerator weightGenerator;
    //Instance of the menu manager that remains alive the entire life of the application.
    public static CornCoordinatorByWeight instance;


    [SerializeField]
    private uint blurIterations = 1;
    private float[,] weightMap;
    [SerializeField]
    private int mapBorderSize;

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
    private GameObject corn1;
    [SerializeField]
    private GameObject corn2;
    [SerializeField]
    private GameObject corn3;

    [SerializeField]
    private GameObject ghostPepper;
    [SerializeField]
    private GameObject zucchini;
    [SerializeField]
    private GameObject pumpkin;
    [SerializeField]
    private int maxNumberOfPowerUps;

    [SerializeField]
    private GameObject lightPost;
    [SerializeField, Range(1, 10)]
    private float maxLightPostDistance = 3;
    [SerializeField, Range(1, 10)]
    private float minLightPostDistance = 1;

    [SerializeField]
    private int numberOfRootMonsters;
    [SerializeField]
    private GameObject rootMonster;
    [SerializeField]
    private int numberOfScareCrows;
    [SerializeField]
    private GameObject scareCrow;

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

    [SerializeField, Range(0, 180)]
    private float maxPathAngle = 25f;



    /// <summary>
    /// TESTING PURPOSES ONLY REMOVE WHEN DOING THE FINAL BUILD.
    /// </summary>
    private int yellowCorn = 0;
    private int redCorn = 0;
    private int blueCorn = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        int[,] intTempMap = weightGenerator.RunSimulation();
        weightMap = GaussianBlur.FloaterizeMap(ref intTempMap);

        for (int i = 0; i < blurIterations; i++)
            weightMap = GaussianBlur.BlurMap(ref weightMap);

        if (drawPaths)
        {
            GeneratePaths(out List<Circle> circles, out List<Path> paths);
            PaintLightPosts(paths);
        }

        PaintFences();

        PaintTiles(); // Paint tiles must be called before the special plants.
        if (GameManager.instance != null)
            GeneratePowerups();
        else
            Debug.Log("No GameManager found, not generating powerups.");
        GenerateRootMonsters();
        GenerateScareCrows();
        PaintCorn(); // Always paint corn last.
        Debug.Log( "Red Corn: " + redCorn + ". Blue Corn: " + blueCorn + ". Yellow Corn: " + yellowCorn + ".");
    }

    private void GeneratePaths(out List<Circle> cutOutCircles, out List<Path> cutOutPaths)
    {
        // Center circle always around player start.
        Circle center = new Circle(new Vector2(weightMap.GetLength(0), weightMap.GetLength(1)) / 2f, 5);
        List<Circle> circles = new List<Circle>();

        int orbits = UnityEngine.Random.Range(minOrbits, maxOrbits);

        float randomAngleFromCenter = UnityEngine.Random.Range(0, 2f * Mathf.PI);
        float meanAngleAdd = 2 * Mathf.PI / orbits;

        // Generates circles and places them.
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
            float randomAngle = UnityEngine.Random.Range(-maxPathAngle, maxPathAngle) * Mathf.Deg2Rad;
            // Random position on the perimeter of each circle to end the path.
            Vector2 startPos = Path.RotateVector2(pathDirection, randomAngle) * center.radius + startDir;

            randomAngle = UnityEngine.Random.Range(-maxPathAngle, maxPathAngle) * Mathf.Deg2Rad;
            Vector2 endPos = Path.RotateVector2(-pathDirection, randomAngle) * orbit.radius + endDir;

            // Radius of the path or half the width of the path. Used to mark tiles for clearing.
            float boundCheckRadius = UnityEngine.Random.Range(minPathWidth, Math.Min(maxPathWidth, orbit.radius / 2));
            paths.Add(new Path(startPos, startDir, endPos, endDir, boundCheckRadius));
        }

        RemoveByCircles(circles, weightMap);
        RemoveByPaths(paths, weightMap);

        cutOutCircles = circles;
        cutOutPaths = paths;
    }

    /// <summary>
    /// Paints a border around play area.
    /// Paints foreground tiles where corn will be spawned and a background tile everywhere else.
    /// </summary>
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
                else // Fill the background with the background tile.
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

    /// <summary>
    /// Paints the Light  Posts onto the map.
    /// </summary>
    private void PaintLightPosts(List<Path> paths)
    {
        HashSet<Vector2Int> lightPositions = new HashSet<Vector2Int>();

        foreach (Path path in paths)
        {
            int lightPlacementType = UnityEngine.Random.Range((int)0, (int)2);

            if (path.radius <= 1.5)
                lightPlacementType = 1;


            float lightPostDistance = UnityEngine.Random.Range(minLightPostDistance, maxLightPostDistance);

            for (float currentPosition = 0 + UnityEngine.Random.Range(0.0f, lightPostDistance);
                currentPosition < path.XLength();
                currentPosition += lightPostDistance)
            {
                if (lightPlacementType % 2 == 0)
                {
                    Vector2 centerPosition = path.GetPositionAtX(currentPosition);

                    path.GetBorderPositionsAtX(currentPosition, out Vector2 position1, out Vector2 position2);

                    position1 = Vector2.Lerp(centerPosition, position1, 0.5f);
                    position2 = Vector2.Lerp(centerPosition, position2, 0.5f);

                    lightPositions.Add(new Vector2Int(Mathf.RoundToInt(position1.x), Mathf.RoundToInt(position1.y)));
                    lightPositions.Add(new Vector2Int(Mathf.RoundToInt(position2.x), Mathf.RoundToInt(position2.y)));
                }
                else
                {
                    Vector2 position = path.GetPositionAtX(currentPosition);
                    lightPositions.Add(new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y)));
                }
            }
        }

        foreach (Vector2Int gridPosition in lightPositions)
        {
            PlaceGameObjectOnTile(lightPost, gridPosition.x, gridPosition.y);
        }
    }

    /// <summary>
    /// Using the weight map the corn is painted.
    /// </summary>
    private void PaintCorn()
    {
        for (int x = 0; x < weightMap.GetLength(0); x++)
        {
            for (int y = 0; y < weightMap.GetLength(1); y++)
            {
                if (weightMap[x, y] > sewThreshold)
                {
                    if (weightMap[x, y] > corn3Threshold)
                    {
                        PlaceGameObjectOnTile(corn3, x, y);
                        redCorn++;
                    }
                    else if (weightMap[x, y] > corn2Threshold)
                    {
                        PlaceGameObjectOnTile(corn2, x, y);
                        blueCorn++;
                    }
                    else if (weightMap[x, y] > corn1Threshold)
                    {
                        PlaceGameObjectOnTile(corn1, x, y);
                        yellowCorn++;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Places the object at the location of the given tile coordinates.
    /// </summary>
    /// <param name="prefab">Object to place.</param>
    /// <param name="x">X index in the tilemap.</param>
    /// <param name="y">Y index in the tilemap.</param>
    private void PlaceGameObjectOnTile(GameObject prefab, int x, int y)
    {
        Vector3 centeredPosition = TileToWorldCoordinates(x, y);

        Instantiate(prefab, centeredPosition, Quaternion.identity);
    }

    /// <summary>
    /// This works for weight maps where both lengths are multiples of 10.
    /// I have no clue why I need to add or subtract depending on the apsect ratio of the weight map.
    /// </summary>
    /// <returns>The Center offset of the tilemap.</returns>
    private Vector3 CalculateCenterOffset()
    {
        int lastXIndex = weightMap.GetLength(0) + 1;
        int lastYIndex = weightMap.GetLength(1) + 1;

        // Get the position of the last tile in the map. Divide it's position by 2 to get the center index.
        Vector3 centerOffset = cornTileMap.CellToWorld(new Vector3Int(lastXIndex, lastYIndex, 0)) * 0.5f;
        return centerOffset;
    }

    private Vector3 TileToWorldCoordinates(int x, int y)
    {
        Vector3 centerOffset = CalculateCenterOffset();

        // Bounds checks.
        if (x < 0)
            x = 0;
        if (x > weightMap.GetLength(0) - 1)
            x = weightMap.GetLength(0) - 1;
        
        if (y < 0)
            y = 0;
        if (y > weightMap.GetLength(1) - 1)
            y = weightMap.GetLength(1) - 1;

        // Used to index the tilemap to find the world position of the cell.
        Vector3 worldCoordinate = cornTileMap.CellToWorld(new Vector3Int(x, y, 0)) * -1;

        Vector3 centeredPosition = worldCoordinate + centerOffset;
        return centeredPosition;
    }

    private Vector3Int WorldToTileCoordinates(Vector3 position)
    {
        Vector3 centerOffset = CalculateCenterOffset();
        Vector3 offSetPosition = position*-1 + centerOffset;

        return cornTileMap.WorldToCell(offSetPosition);
    }

    /// <summary>
    /// Chooses a random tile in the play area of the map.
    /// The tile will be ~distance tiles away from the player.
    /// The range is used for both the x and the y, which means that the magnitude of the distance is not constant.
    /// </summary>
    /// <param name="distance">Tile distance away from the player.</param>
    /// <param name="position">position to count as the origin.</param>
    /// <returns>Coordinates of the random tile.</returns>
    public Vector3 RandomTileDistanceAway(int distance, Vector3 position)
    {
        Vector3Int tilePositionOfPlayer = WorldToTileCoordinates(position);

        // deltaX will be within the range of [0, distance].
        int deltaX = UnityEngine.Random.Range(0, distance);

        // Subtract the value of deltaX from the desired tile distance to get the remaining distance to be traveled.
        int deltaY = distance - deltaX;

        // Randomly choose to go in the negative direction. the integers are 0 or 1.
        int xNegative = UnityEngine.Random.Range(0, 2);
        int yNegative = UnityEngine.Random.Range(0, 2);

        if (xNegative == 1)
            deltaX *= -1;

        if (yNegative == 1)
            deltaY *= -1;

        return TileToWorldCoordinates(tilePositionOfPlayer.x + deltaX, tilePositionOfPlayer.y + deltaY);
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

    /// <summary>
    /// Generates powerups and their locations. Alters the weight map so this must be called before corn generation.
    /// </summary>
    private void GeneratePowerups()
    {
        Dictionary<UpgradeHolder.UpgradeType, float> powerUps = new Dictionary<UpgradeHolder.UpgradeType, float>();

        float totalWeight = SumWeightsAndPopulateDictionary(powerUps);
        float randomWeight;

        UpgradeHolder upgrades = GameManager.instance.upgrades;

        // Number of powerups increases with upgrade levels.
        float sumOfLevels = upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.PEPPERPROBABILITY) +
            upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.PUMPKINPROBABILITY) +
            upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.ZUCCINNIPROBABILITY);

        // 9 total levels for the 3 types of plants. So when all powerups are the highest level the sumOfLevels = 9.
        // This means that all the plants need to have the maximum number of upgrades to reach the maxNumberOfPowerups.
        double scaledNumberOfPowerups = (maxNumberOfPowerUps * sumOfLevels / 9f);

        for (int i = 0; i < scaledNumberOfPowerups; i++)
        {
            randomWeight = UnityEngine.Random.Range(0, totalWeight);

            // This is the weighted value calculation.
            foreach (UpgradeHolder.UpgradeType upType in powerUps.Keys)
            {
                if (randomWeight <= powerUps[upType] && powerUps[upType] != 0)
                {
                    DeterminePowerupAndSpawn(upType);
                    break;
                }

                randomWeight -= powerUps[upType];
            }
        }

    }

    /// <summary>
    /// Sums the weights of all the spawnable powerups.
    /// Populates the dictionary with the powerup type as the key and weight as the value.
    /// </summary>
    /// <param name="powerUps">Dictionary to populate.</param>
    /// <returns>The total weight of all the spawnable powerup types.</returns>
    private float SumWeightsAndPopulateDictionary(Dictionary<UpgradeHolder.UpgradeType, float> powerUps)
    {
        float totalWeight = 0;

        //Get the weights for all powerups. 
        for (int i = 4; i < 7; i++)
        {
            // Determine type and weight, then add the weight to the total.
            UpgradeHolder.UpgradeType upgradeType = (UpgradeHolder.UpgradeType)i;
            float upgradeWeight = GameManager.instance.upgrades.GetMultiplier(upgradeType);
            totalWeight += upgradeWeight;

            powerUps.Add(upgradeType, upgradeWeight);
        }

        return totalWeight;
    }

    /// <summary>
    /// Determines the type of upgrade passed and spawns the cooresponding powerup.
    /// </summary>
    /// <param name="type">The type of upgrade to be spawned.</param>
    private void DeterminePowerupAndSpawn(UpgradeHolder.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeHolder.UpgradeType.PEPPERPROBABILITY:
                SpawnPrefab(ghostPepper);
                break;

            case UpgradeHolder.UpgradeType.ZUCCINNIPROBABILITY:
                SpawnPrefab(zucchini);
                break;

            case UpgradeHolder.UpgradeType.PUMPKINPROBABILITY:
                SpawnPrefab(pumpkin);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Spawns the given powerup.
    /// Alters the Weight map so that corn does not spawn on the same tile as a powerup.
    /// Only spawns powerups where corn would have spawned so that powerups are not found in the middle of nowhere.
    /// </summary>
    /// <param name="prefab">Powerup to be spawned.</param>
    private void SpawnPrefab(GameObject prefab)
    {
        int xPos = 0;
        int yPos = 0;

        // Randomize position until powerup replaces a corn. Prevents powerups from spawning in the middle of nowhere.
        while (weightMap[xPos, yPos] < sewThreshold)
        {
            xPos = UnityEngine.Random.Range(0, weightMap.GetLength(0));
            yPos = UnityEngine.Random.Range(0, weightMap.GetLength(1));
        }

        // Prevent corn from spawning on tile.
        weightMap[xPos, yPos] = 0f;

        // Creates the object at the random position.
        PlaceGameObjectOnTile(prefab, xPos, yPos);
    }

    /// <summary>
    /// Replaces corn with root monsters.
    /// Must be performed before corn is placed becausee the weight map is altered.
    /// </summary>
    private void GenerateRootMonsters()
    {
        for (int i = 0; i < numberOfRootMonsters; i++)
            SpawnPrefab(rootMonster);
    }

    /// <summary>
    /// Replaces corn with scare crows.
    /// Must be performed before corn is placed becausee the weight map is altered.
    /// </summary>
    private void GenerateScareCrows()
    {
        for (int i = 0; i < numberOfScareCrows; i++)
            SpawnPrefab(scareCrow);
    }
}
