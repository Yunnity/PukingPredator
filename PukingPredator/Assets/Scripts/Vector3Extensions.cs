using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 HorizontalProjection(this Vector3 v)
    {
        var hp = v;
        hp.y = 0;
        return hp;
    }
}
