using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : SingletonMonobehaviour<CheckpointManager>
{
    public Vector3? LastCheckpointPosition { get; set; } = null;

    /// <summary>
    /// The collectables that the player has collected prior to the checkpoint
    /// </summary>
    public List<Collectable> previousCollected;

    /// <summary>
    /// The collectables that the player has collected
    /// </summary>
    private List<Collectable> collected;

    public void Start()
    {
        collected = new List<Collectable>();
        previousCollected = new List<Collectable>();
    }

    public void AddCollectable(Collectable collectable)
    {
        collected.Add(collectable);
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
        previousCollected = new List<Collectable> (collected);
    }

    public void Respawn()
    {
        collected = new List<Collectable>  (previousCollected);
    }
}
