using UnityEngine;

[RequireComponent(typeof(PhysicsBehaviour))]
public class CollapseIfHeavyPlayer : MonoBehaviour
{
    /// <summary>
    /// If player is greater than or equal to this mass when they collide with
    /// the structure, collapse the structure.
    /// </summary>
    [SerializeField]
    private float massThreshold = 5f;

    private PhysicsBehaviour pb;



    void Start()
    {
        pb = GetComponent<PhysicsBehaviour>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag(GameTag.player))
        {
            var player = collision.collider.GetComponent<Player>();
            if (IsPlayerHeavy(player))
            {
                pb.EnablePhysics();
            }
        }
    }



    /// <summary>
    /// Checks if the player is heavy enough to cause a collapse.
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private bool IsPlayerHeavy(Player player)
    {
        //TODO: should this be swapped to actually use the mass, or should it
        //      continue to use the item count to keep it intuitive
        return player.inventory.itemCount >= massThreshold;
    }
}
