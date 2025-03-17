using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class AimVisualizer : InputBehaviour
{
    private float baseAlpha;

    private bool isAiming = false;

    /// <summary>
    /// The property in the shader to be modified.
    /// </summary>
    private string lerpPropertyName = "_alpha";

    /// <summary>
    /// Used to fade the alpha gradually.
    /// </summary>
    private Coroutine lerpRoutine;

    /// <summary>
    /// Speed of alpha fading.
    /// </summary>
    public float lerpSpeed => isAiming ? 8 : 20;

    /// <summary>
    /// The material of the visual object to be modified to change its alpha.
    /// </summary>
    private Material material;

    private float materialAlpha
    {
        get => material.GetFloat(lerpPropertyName);
        set => material.SetFloat(lerpPropertyName, value);
    }

    /// <summary>
    /// The player component.
    /// </summary>
    private Player player;

    /// <summary>
    /// The max distance something can be to aim at it.
    /// </summary>
    private float range => InteractablePicker.BASE_RANGE * transform.localScale.y;

    /// <summary>
    /// The object used to visualize the aim range.
    /// </summary>
    public GameObject visualObject;



    void Start()
    {
        player = GetComponent<Player>();

        material = visualObject.GetComponent<Renderer>().material;
        baseAlpha = materialAlpha;
        materialAlpha = 0;

        SetSize(range);

        Subscribe(InputEvent.onAim, () => {
            isAiming = true;
            SetVisible(true);
            player.SetState(PlayerState.aiming);
        });
        Subscribe(InputEvent.onEat, () => {
            isAiming = false;
            SetVisible(false);
            if (player.state == PlayerState.aiming) { player.SetState(PlayerState.standing); }
        });
    }


    /// <summary>
    /// Updates the x and z scale of the range visualizer
    /// </summary>
    /// <param name="radius"></param>
    private void SetSize(float radius)
    {
        var scale = visualObject.transform.localScale;
        scale.x = radius * 2; // 2x because its diameter, not radius
        scale.z = scale.x;
        visualObject.transform.localScale = scale;
    }

    public void SetVisible(bool value)
    {
        if (lerpRoutine != null)
        {
            StopCoroutine(lerpRoutine);
        }

        var targetAlpha = value ? baseAlpha : 0;
        var delay = (value && materialAlpha == 0) ? GameInput.minHoldTime * 2 : 0;
        lerpRoutine = StartCoroutine(FadeAlpha(targetAlpha, delay));
    }

    private IEnumerator FadeAlpha(float targetAlpha, float delay)
    {
        var remainingDelay = delay;
        while (remainingDelay > 0)
        {
            remainingDelay -= Time.deltaTime;
            yield return null;
        }

        float currentValue = materialAlpha;

        while (Mathf.Abs(currentValue - targetAlpha) >= 0.05)
        {
            currentValue = Mathf.Lerp(currentValue, targetAlpha, lerpSpeed * Time.deltaTime);
            materialAlpha = currentValue;
            yield return null;
        }

        materialAlpha = targetAlpha;
        Debug.Log($"finished, set alpha to {materialAlpha}");
    }

}
