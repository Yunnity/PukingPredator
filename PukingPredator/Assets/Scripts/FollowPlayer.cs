using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    /// <summary>
    /// If the object should copy changes to the players scale.
    /// </summary>
    [SerializeField]
    private float copyPlayerScaleFactor = 0;

    /// <summary>
    /// The initial scale of the player object.
    /// </summary>
    private float initialPlayerScaleMagnitude;

    /// <summary>
    /// The initial scale of this object.
    /// </summary>
    private Vector3 initialScale;

    /// <summary>
    /// The relative position to the player.
    /// </summary>
    private Vector3 OFFSET = new Vector3(0, 0.15f, 0);

    /// <summary>
    /// Player object to follow
    /// </summary>
    [SerializeField]
    private GameObject player;

    

    void Start()
    {
        player ??= GameObject.FindGameObjectsWithTag("Player")[0];


        initialPlayerScaleMagnitude = player.transform.localScale.magnitude;
        initialScale = transform.localScale;
    }

    void Update()
    {
        var relativePlayerScale = player.transform.localScale.magnitude / initialPlayerScaleMagnitude;
        //copy "copyPlayerScaleFactor" percent of the scale CHANGE
        var multiplier = 1 + (relativePlayerScale - 1) * copyPlayerScaleFactor;

        transform.position = player.transform.position + OFFSET * multiplier;

        transform.localScale = initialScale * multiplier;
    }
}
