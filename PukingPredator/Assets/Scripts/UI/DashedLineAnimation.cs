using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DashedLineAnimation : MonoBehaviour
{
    public float scrollSpeed = 2.0f;
    private Material lineMaterial;

    void Start()
    {
        lineMaterial = GetComponent<LineRenderer>().material;
    }

    void Update()
    {
        float offset = Time.time * scrollSpeed * -1;
        lineMaterial.mainTextureOffset = new Vector2(offset, 0);
    }
}