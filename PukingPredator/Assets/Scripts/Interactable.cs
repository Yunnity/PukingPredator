using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour
{
    /// <summary>
    /// Collider attached to the instance.
    /// </summary>
    protected Collider hitbox;

    /// <summary>
    /// If the object can be targetted for interaction.
    /// </summary>
    public abstract bool isInteractable { get; }

    /// <summary>
    /// Size of the outline visual.
    /// </summary>
    private const float OUTLINE_RADIUS = 2.2f;

    /// <summary>
    /// Settings for the outline of the consumable (toggled on via enable when player is near)
    /// </summary>
    public Outline outline;

    /// <summary>
    /// The color of the outline when close to the player.
    /// </summary>
    private Color outlineColor = Color.white;

    /// <summary>
    /// Invoked when the object is highlighted (outline enabled).
    /// </summary>
    public event Action Highlighted;

    /// <summary>
    /// Invoked when the object is unhighlighted (outline disabled).
    /// </summary>
    public event Action Unhighlighted;


    protected virtual void Awake()
	{
        ConfigureOutline();
        hitbox = GetComponent<Collider>();
    }



    /// <summary>
    /// Gets the closest point on the collider to the reference position.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector3 ClosestPoint(Vector3 pos)
    {
        return hitbox.ClosestPoint(pos);
    }

    /// <summary>
    /// sets the outline and lets subsribers know the interactable is outlined
    /// <returns></returns>
    public void broadcastEnabled(bool enable)
    {
        outline.enabled = enable;
        if (enable)
        {
            Highlighted?.Invoke();
        } else
        {
            Unhighlighted?.Invoke();
        }
    }


    private void ConfigureOutline()
    {
        outline = gameObject.GetComponent<Outline>();
        if (outline == null) { outline = gameObject.AddComponent<Outline>(); }

        outline.OutlineWidth = OUTLINE_RADIUS;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineColor = outlineColor;
        outline.enabled = false;
    }
}

