using System;
using UnityEngine;

public enum SurfaceType
{
    opaque,
    transparent,
}
public static class MaterialExtensions
{
    /*
     * NOTE: if the SetToOpaque and SetToTransparent aren't sufficient, look at
     * the following link for another version that might work:
     * https://discussions.unity.com/t/change-surface-type-with-lwrp/221034/3
     */

    /// <summary>
    /// Gets the current alpha.
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    public static float GetAlpha(this Material material)
    {
        return material.color.a;
    }

    /// <summary>
    /// Gets the current surface type.
    /// </summary>
    /// <param name="material"></param>
    /// <returns></returns>
    public static SurfaceType GetSurfaceType(this Material material)
    {
        switch(material.GetFloat("_Surface"))
        {
            case 0:
                return SurfaceType.opaque;
            case 1:
                return SurfaceType.transparent;
            default:
                throw new NotSupportedException("Invalid surface type.");
        }
    }

    /// <summary>
    /// Sets the alpha.
    /// </summary>
    /// <param name="material"></param>
    /// <param name="alpha"></param>
    public static void SetAlpha(this Material material, float alpha)
    {
        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }

    /// <summary>
    /// Sets the surface type to the given type.
    /// </summary>
    /// <param name="material"></param>
    /// <param name="surfaceType"></param>
    public static void SetSurfaceType(this Material material, SurfaceType surfaceType)
    {
        switch(surfaceType)
        {
            case SurfaceType.opaque:
                material.SetSurfaceTypeOpaque();
                break;
            case SurfaceType.transparent:
                material.SetSurfaceTypeTransparent();
                break;
        }
    }

    /// <summary>
    /// Sets the surface type to opaque.
    /// </summary>
    /// <param name="material"></param>
    public static void SetSurfaceTypeOpaque(this Material material)
    {
        // Set the rendering mode to Opaque
        material.SetFloat("_Surface", 0); // 0 = Opaque

        // Disable blending
        material.SetOverrideTag("RenderType", "Opaque");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        material.SetInt("_ZWrite", 1); // Enable depth writing for opaque objects

        // Set the render queue to Opaque
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        // Enable the opaque keyword
        material.EnableKeyword("_SURFACE_TYPE_OPAQUE");
        material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    /// <summary>
    /// Sets the surface type to transparent.
    /// </summary>
    /// <param name="material"></param>
    public static void SetSurfaceTypeTransparent(this Material material)
    {
        // Set the rendering mode to Transparent
        material.SetFloat("_Surface", 1); // 1 = Transparent, 0 = Opaque

        // Enable blending for transparency
        material.SetOverrideTag("RenderType", "Transparent");
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0); // Disable depth writing for transparency

        // Change the render queue to Transparent
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Enable the transparency keyword
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_SURFACE_TYPE_OPAQUE");
    }
    
}

