using UnityEngine;

public class Wobble : MonoBehaviour
{
    [SerializeField]
    private float multiplierX = 1f;
    [SerializeField]
    private float multiplierY = 1f;
    [SerializeField]
    private float multiplierZ = 1f;
    [SerializeField]
    private float offsetX = 0f;
    [SerializeField]
    private float offsetY = 0f;
    [SerializeField]
    private float offsetZ = 0f;
    private WobbleData wobbleX;
    private WobbleData wobbleY;
    private WobbleData wobbleZ;

    private class WobbleData
    {
        private float amplitude;
        private float timeScale;
        private float timeOffset;

        public WobbleData()
        {
            amplitude = Random.Range(0.01f, 0.015f);
            timeScale = Random.Range(0.9f, 1.2f) * 3f;
            timeOffset = Random.Range(0f, 20f);
        }

        public float GetOffset()
        {
            return amplitude * Mathf.Sin(timeScale * (Time.time - timeOffset));
        }
    }



    private void Start()
    {
        wobbleX = new();
        wobbleY = new();
        wobbleZ = new();
    }

    void Update()
    {
        transform.localPosition = new Vector3(
            wobbleX.GetOffset() * multiplierX + offsetX,
            wobbleY.GetOffset() * multiplierY + offsetY,
            wobbleZ.GetOffset() * multiplierZ + offsetZ
        );
    }
}
