using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CornCoordinator : MonoBehaviour
{
    
    public TileAutomation TA;
    public GameObject Corn1;
    public GameObject Corn2;
    public GameObject Corn3;
    public float TileMapScaleFactor;
    public Tilemap cornTileMap;

    private int[,] cornMap;

    // Start is called before the first frame update
    void Start()
    {
        cornMap = TA.RunSimulation();
        PlaceCorn();
    }

    void PlaceCorn()
    {
        for (int x = 0; x < TA.tmWidth; x++)
        {
            for (int y = 0; y < TA.tmHeight; y++)
            {
                int neighbors = TA.CountNeighbors(x, y, ref cornMap);


                // Used to index the tilemap to find the world position of the cell.
                Vector3 worldCoordinate = cornTileMap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                // Cast to ints because tilemap expects a Vector3Int.
                //int xPos = (int)(-worldCoordinate.x + TA.tmWidth / 2);
                //int yPos = (int)(-worldCoordinate.y + TA.tmHeight / 2);

                int xPos = (int)(-(worldCoordinate.x / TileMapScaleFactor) + TA.tmWidth / 2);
                int yPos = (int)(-(worldCoordinate.y / TileMapScaleFactor) + TA.tmHeight / 2);

                // The x and y positions need to be centered to the middle of the tileMap.
                Vector3Int centeredPosition = new Vector3Int(xPos, yPos, 0);
                if (cornMap[x,y] == 1)
                    Instantiate(Corn1, cornTileMap.GetCellCenterWorld(centeredPosition), Quaternion.identity);
            }
        }
    }
}
