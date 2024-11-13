using UnityEngine;

public class DestroyOnLowVelocity : MonoBehaviour
{
    /// <summary>
    /// The minimum velocity magnitude before it gets destroyed before.
    /// </summary>
    [SerializeField]
    private float minVelocity;

    /// <summary>
    /// The rigidbody to get the velocity from.
    /// </summary>
    public Rigidbody rb;

    /// <summary>
    /// Used to skip the first frame.
    /// </summary>
    private bool skipFrame = true;



    void Update()
	{
        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        if (rb.velocity.magnitude <= minVelocity) { Destroy(gameObject);  }
	}
}

