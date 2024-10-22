using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraCulling : MonoBehaviour
{
    public Transform player;

    private List<GameObject> previousObstructed;
    List<GameObject> currentHits;

    private RaycastHit hit;

    private void Start()
    {
        previousObstructed = new List<GameObject>();
        currentHits = new List<GameObject>();
    }

    void LateUpdate()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceBetween = Vector3.Distance(player.position, transform.position);
        float cullingDistance =  distanceBetween - 0.1f;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, directionToPlayer, cullingDistance);
        currentHits.Clear();

        foreach (RaycastHit hit in hits)
        {

            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.tag == GameTag.player) continue;

            // Stores the currenthits in a list so it can easily be checked for non-hit previous objects
            currentHits.Add(hitObject);
            if (previousObstructed.Contains(hitObject)) continue;

            SeTransparencyRecursively(hitObject, 0.15f);
            previousObstructed.Add(hitObject);  
        }
        
        for (int i = previousObstructed.Count - 1; i >= 0; i--)
        {
            GameObject previous = previousObstructed[i];
            if (!currentHits.Contains(previous))
            {
                SeTransparencyRecursively(previous, 1);
                previousObstructed.RemoveAt(i);
            }
        }
    }

    void SeTransparencyRecursively(GameObject obj, float transparency)
    {
        MakeTransparent(obj, transparency);
        foreach (Transform child in obj.transform)
        {
            SeTransparencyRecursively(child.gameObject, transparency);
        }
    }

    void MakeTransparent(GameObject obj, float transparencyLevel)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        
        if (renderer != null)
        {
            foreach (Material mat in renderer.materials)
            {
                if (!mat.HasProperty("_Color")) continue;
                SetMaterialToTransparent(mat, transparencyLevel != 1);

                Color color = mat.color;
                color.a = transparencyLevel;
                mat.color = color;
            }
        }
    }


    // Taken from https://discussions.unity.com/t/change-surface-type-with-lwrp/221034/3
    private enum SurfaceType
    {
        Opaque,
        Transparent
    }

    private enum BlendMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply
    }


    private void SetMaterialToTransparent(Material mat, bool transparent)
    {
        if (transparent)
        {
            mat.SetFloat("_Surface", (float)SurfaceType.Transparent);
            mat.SetFloat("_Blend", (float)BlendMode.Alpha);
        }
        else
        {
            mat.SetFloat("_Surface", (float)SurfaceType.Opaque);
        }
        SetupMaterialBlendMode(mat);
    }

    void SetupMaterialBlendMode(Material material)
    {
        if (material == null)
            throw new ArgumentNullException("material");

        bool alphaClip = material.GetFloat("_AlphaClip") == 1;
        if (alphaClip)
            material.EnableKeyword("_ALPHATEST_ON");
        else
            material.DisableKeyword("_ALPHATEST_ON");

        SurfaceType surfaceType = (SurfaceType)material.GetFloat("_Surface");
        if (surfaceType == 0)
        {
            material.SetOverrideTag("RenderType", "");
            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            material.SetInt("_ZWrite", 1);
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = -1;
            material.SetShaderPassEnabled("ShadowCaster", true);
        }
        else
        {
            BlendMode blendMode = (BlendMode)material.GetFloat("_Blend");
            switch (blendMode)
            {
                case BlendMode.Alpha:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Premultiply:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Additive:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
                case BlendMode.Multiply:
                    material.SetOverrideTag("RenderType", "Transparent");
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.DstColor);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                    material.SetShaderPassEnabled("ShadowCaster", false);
                    break;
            }
        }
    }
}
