using UnityEngine;

/// <summary>
/// Causes the game object to point towards the velocity of the given rigidbody
/// (not necessarily the objects own rigidbody).
/// </summary>
public class AlignWithVelocity : MonoBehaviour
{
	/// <summary>
	/// The rigidbody to get the velocity from.
	/// </summary>
	public Rigidbody rb;



	void Update()
	{
		if (rb.velocity.magnitude <= 0.1f) { return; }

        // Calculate the rotation to face the velocity direction
        Quaternion targetRotation = Quaternion.LookRotation(rb.velocity);
        // Smoothly rotate towards the target direction
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
    }
}

