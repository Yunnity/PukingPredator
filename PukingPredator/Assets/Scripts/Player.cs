using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    //TODO: the purpose of this class (for now) is now to handle the game state, specifically reaching end of level or falling off cliffs

    /// <summary>
    /// The rigidbody of the player.
    /// </summary>
    private Rigidbody rb;



    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        
    }


}