using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerspectiveUtilities
{
    private static Vector2 perspectiveWarp = new Vector2(1,0.75f);

    public static Vector2 NormalSpaceToPerspective(Vector2 normalSpaceVec)
    {
        return new Vector2(normalSpaceVec.x * perspectiveWarp.x, normalSpaceVec.y * perspectiveWarp.y);
    }

    public static Vector2 PerspectiveSpaceToNormalSpace(Vector2 perspectiveSpaceVec)
    {
        return new Vector2(perspectiveSpaceVec.x / perspectiveWarp.x, perspectiveSpaceVec.y / perspectiveWarp.y);
    }
}
