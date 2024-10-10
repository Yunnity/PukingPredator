using UnityEngine;

public class Lock : MonoBehaviour
{
    //TODO: should the lock have an awake/start event that sets the gameobject to be unconsumable and lock its motion

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with " + collision.gameObject.name);

        if (collision.gameObject.CompareTag(GameTag.key))
        {
            Debug.Log("Key hit door");

            // Destroy the key
            Destroy(collision.gameObject);

            // Make the instance consumable now that it is unlocked
            var consumable = GetComponent<Consumable>();
            consumable.isConsumable = true;

            //TODO: if the lock also locks motion, make sure it unlocks it here too
        }
    }
}
