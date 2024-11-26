using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFlicker : MonoBehaviour
{
    /// <summary>
    /// Percentage change allowed.
    /// </summary>
    [SerializeField]
    [Range(0, 1)]
    private float amplitude;

    private float baseIntensity;

    private Light l;

    /// <summary>
    /// If the flickering should only reduce the light value.
    /// </summary>
    [SerializeField]
    private bool shouldOnlySubtract = true;

    private List<FlickerData> flickerData = new();
    private class FlickerData
    {
        private float timeScale;
        private float timeOffset;

        public FlickerData(float timeScale, float timeOffset)
        {
            this.timeScale = timeScale;
            this.timeOffset = timeOffset;
        }

        public float GetAmplitude()
        {
            return Mathf.Sin(timeScale * (Time.time - timeOffset));
        }
    }



    void Start()
    {
        l = GetComponent<Light>();
        baseIntensity = l.intensity;

        flickerData.Add(new FlickerData(
            timeScale: Random.Range(1.2f, 1.6f),
            timeOffset: Random.Range(0, 5000f)
        ));
        flickerData.Add(new FlickerData(
            timeScale: Random.Range(1.2f, 1.6f),
            timeOffset: Random.Range(0, 5000f)
        ));
        flickerData.Add(new FlickerData(
            timeScale: Random.Range(4.2f, 1.6f),
            timeOffset: Random.Range(0, 5000f)
        ));
    }

    void Update()
    {
        var a = flickerData.Select(fd => fd.GetAmplitude()).Sum() / flickerData.Count;
        if (shouldOnlySubtract) { a -= 0.5f; }
        l.intensity = (1 + a * amplitude) * baseIntensity;
    }
}
