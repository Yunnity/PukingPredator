using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);

        if (collision.gameObject.tag == "Key")
        {
            Debug.Log("Key hit door");

            // Destroy the key
            Destroy(collision.gameObject);

            // Make the door consumable now that it is unlocked
            var consumable = GetComponent<Consumable>();
            consumable.isConsumable = true;
        }
    }
}
