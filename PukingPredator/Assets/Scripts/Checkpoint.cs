using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTag.player))
        {
            CheckpointManager.Instance.SetCheckpoint(transform.position);
        }
    }
}
