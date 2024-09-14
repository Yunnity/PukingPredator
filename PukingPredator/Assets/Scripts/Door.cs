using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);

        if (collision.gameObject.tag == "Key")
        {
            Debug.Log("Key hit door");

            // Destroy the key and the door
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}
