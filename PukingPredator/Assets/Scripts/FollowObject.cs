using UnityEngine;

public class FollowObject : MonoBehaviour
{
    /// <summary>
    /// If the object should copy changes to the players scale.
    /// </summary>
    [SerializeField]
    private float copyTargetScaleFactor = 0;

    /// <summary>
    /// The initial scale of this object.
    /// </summary>
    private Vector3 initialScale;

    /// <summary>
    /// The initial scale of the player object.
    /// </summary>
    private float initialTargetScaleMagnitude;

    /// <summary>
    /// The relative position to the player.
    /// </summary>
    private Vector3 offset;

    /// <summary>
    /// Player object to follow
    /// </summary>
    [SerializeField]
    public GameObject target;



    protected virtual void Start()
    {
        offset = transform.position - target.transform.position;

        initialTargetScaleMagnitude = target.transform.localScale.magnitude;
        initialScale = transform.localScale;
    }

    void Update()
    {
        var relativeTargetScale = target.transform.localScale.magnitude / initialTargetScaleMagnitude;
        //copy "copyTargetScaleFactor" percent of the scale CHANGE
        var multiplier = 1 + (relativeTargetScale - 1) * copyTargetScaleFactor;

        transform.position = target.transform.position + offset * multiplier;

        transform.localScale = initialScale * multiplier;
    }
}

