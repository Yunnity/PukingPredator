using UnityEngine;

public static class Vector3Extensions
{
    public static Vector3 HorizontalProjection(this Vector3 v)
    {
        var hp = v;
        hp.y = 0;
        return hp;
    }

    /// <summary>
    /// Rotates the vector about the Y axis by the given radians and returns
    /// the result.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="radians"></param>
    /// <returns></returns>
    public static Vector3 RotateAboutY(this Vector3 v, float radians)
    {
        // Create a Quaternion representing the rotation around the Y-axis
        Quaternion rotation = Quaternion.AngleAxis(radians * Mathf.Rad2Deg, Vector3.up);

        // Apply the rotation to the forward vector
        return rotation * v;
    }
}
