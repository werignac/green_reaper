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
    public float TileMapXScaleFactor;
    public float TileMapYScaleFactor;
    public Tilemap cornTileMap;

    private int[,] cornMap;

    // Start is called before the first frame update
    void Start()
    {
        cornMap = TA.RunSimulation();
        PlaceCorn();
    }

    /// <summary>
    /// Places corn based on the cornMap.
    /// Corn is placed into the cornTileMap.
    /// </summary>
    void PlaceCorn()
    {
        for (int x = 0; x < TA.tmWidth; x++)
        {
            for (int y = 0; y < TA.tmHeight; y++)
            {
                int neighbors = TA.CountNeighbors(x, y, ref cornMap);


                // Used to index the tilemap to find the world position of the cell.
                Vector3 worldCoordinate = cornTileMap.GetCellCenterWorld(new Vector3Int(x, y, 0));

                // Not sure why I need these, but the position is off without them.
                // Has something to do with the x and y values of the tilemap.
                float xOffset = TA.tmWidth / 2 * TileMapXScaleFactor;
                float yOffset = TA.tmHeight / 2 * TileMapYScaleFactor;
                
                float xPos = -worldCoordinate.x + xOffset;
                float yPos = -worldCoordinate.y + yOffset;

                // The x and y positions need to be centered to the middle of the tileMap.
                Vector3 centeredPosition = new Vector3(xPos, yPos, 0);
                if (cornMap[x,y] == 1)
                {
                    if(neighbors > 7)
                    {
                        Instantiate(Corn1, centeredPosition, Quaternion.identity);
                    }

                    else if (neighbors > 4)
                    {
                        Instantiate(Corn2, centeredPosition, Quaternion.identity);
                    }
                    else 
                        Instantiate(Corn3, centeredPosition, Quaternion.identity);
                }  
            }
        }
    }
}
