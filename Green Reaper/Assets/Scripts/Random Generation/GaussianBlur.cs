using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GaussianBlur
{
    private static readonly float[,] gaussianCurve2D = new float[,] {
        {0.0036720629f,0.0146482639f,0.0232043125f,0.0146482639f,0.0036720629f},
        {0.0146482639f,0.0584335396f,0.0925645609f,0.0584335396f,0.0146482639f},
        {0.0232043125f,0.0925645609f,0.1466315063f,0.0925645609f,0.0232043125f},
        {0.0146482639f,0.0584335396f,0.0925645609f,0.0584335396f,0.0146482639f},
        {0.0036720629f,0.0146482639f,0.0232043125f,0.0146482639f,0.0036720629f}};

    private const int curveOffset = -3;

    public static float[,] FloaterizeMap(ref int[,] map)
    {
        float[,] floatMap = new float[map.GetLength(0), map.GetLength(1)];

        for (int tileX = 0; tileX < map.GetLength(0); tileX++)
        {
            for (int tileY = 0; tileY < map.GetLength(1); tileY++)
            {
                floatMap[tileX, tileY] = map[tileX, tileY];
            }
        }

        return floatMap;
    }

    public static float[,] BlurMap(ref float[,] map)
    {
        float[,] blurredMap = new float[map.GetLength(0), map.GetLength(1)];

        for (int tileX = 0; tileX < map.GetLength(0); tileX++)
        {
            for (int tileY = 0; tileY < map.GetLength(1); tileY++)
            {
                blurredMap[tileX, tileY] = BlurTile(tileX, tileY, ref map);
            }
        }

        return blurredMap;
    }

    private static float BlurTile(int x, int y, ref float[,] map)
    {
        float weight = 0;

        for (int filterX = 0; filterX < gaussianCurve2D.GetLength(0); filterX++)
        {
            for (int filterY = 0; filterY < gaussianCurve2D.GetLength(1); filterY++)
            {
                Vector2Int adjacent = new Vector2Int(filterX + 1 + curveOffset + x, filterY + 1 + curveOffset + y);

                //Default for out-of-bounds.
                float value = 0.1f;

                // Exclude the values outside of the map. Only considers values within the bounds of the tile map.
                if (adjacent.x >= 0 && adjacent.x < map.GetLength(0) && adjacent.y >= 0 && adjacent.y < map.GetLength(1))
                {
                    value = map[adjacent.x, adjacent.y];
                }

                float curveMultiplier = gaussianCurve2D[filterX, filterY];
                weight += value * curveMultiplier;
            }
        }

        return weight;
    }
}
