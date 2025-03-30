using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannonball : MonoBehaviour
{
    [SerializeField] float lifespan;
    [SerializeField] float alphaCutoff;

    Timer timer;
    Renderer renderer;
    Color materialColor;
    bool isFading;

    void Start()
    {
        timer = gameObject.AddComponent<Timer>();
        timer.onTimerComplete += StartDestroying;
        renderer = GetComponent<Renderer>();
        materialColor = renderer.material.color;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(collision.gameObject);
        }
        timer.StartTimer(lifespan);
    }

    void StartDestroying()
    {
        isFading = true;
    }

    void Update()
    {
        if (renderer.material.color.a <= alphaCutoff)
        {
            isFading = false;
            Destroy(gameObject);
        }

        if (isFading)
        {
            renderer.material.color = Color.Lerp(renderer.material.color, new Color(materialColor.r, materialColor.g, materialColor.b, 0f), Time.deltaTime * 0.55f);
            Debug.Log(renderer.material.color);
        }

    }
}
