using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    public Vector3? LastCheckpointPosition { get; set; } = null;

    /// <summary>
    /// The ids of the collectables that the player has collected prior to the checkpoint
    /// </summary>
    public List<int> previousCollected;

    /// <summary>
    /// The ids pf the collectables that the player has collected
    /// </summary>
    private List<int> collected;

    public void Start()
    {
        collected = new List<int>();
        previousCollected = new List<int>();
    }

    public void AddCollectable(int id)
    {
        collected.Add(id);
    }

    /// <summary>
    /// Clears out the collectables and previous checkpoint when replaying the level
    /// </summary> 
    public void Clear()
    {
        LastCheckpointPosition = null;
        collected.Clear();
        previousCollected.Clear();
    }

    public void SetCheckpoint(Vector3 position)
    {
        if (LastCheckpointPosition == position) return;
        LastCheckpointPosition = position;
        previousCollected = new List<int> (collected);
    }

    /// <summary>
    /// Resets collectables collected after checkpoint
    /// </summary> 
    public void ResetCollected()
    {
        collected = new List<int>  (previousCollected);
    }
}
