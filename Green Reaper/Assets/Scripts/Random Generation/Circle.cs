using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle
{
    public Vector2 center;
    public float radius;

    public Circle (Vector2 c, float r)
    {
        center = c;
        radius = r;
    }

    public bool InRange(Vector2 point)
    {
        return Vector2.Distance(center, point) < radius;
    }

}
