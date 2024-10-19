using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
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



    protected virtual void Awake()
	{
        ConfigureOutline();
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

