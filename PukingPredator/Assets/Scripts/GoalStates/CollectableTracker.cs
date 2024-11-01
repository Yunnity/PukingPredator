using System.Collections.Generic;
using UnityEngine;

public class CollectableTracker : MonoBehaviour
{
    private AudioManager audioManager;

    /// <summary>
    /// How many collectables the player got.
    /// </summary>
    public int collectedCount => totalCollectables - remainingCollectables;

    /// <summary>
    /// The prefab for the slot in the collectables UI
    /// </summary>
    [SerializeField]
    private GameObject collectableSlotPrefab;

    /// <summary>
    /// The UI slot component for the slots that still havent been filled.
    /// </summary>
    private List<CollectableSlotUI> emptySlots = new();

    /// <summary>
    /// The prefab for the particle effect when collecting an item
    /// </summary>
    [SerializeField]
    private GameObject particleEffect;

    /// <summary>
    /// The particle system for playing the particle effect
    /// </summary>
    private ParticleSystem ps;

    /// <summary>
    /// The number of collectables in the level still.
    /// </summary>
    private int remainingCollectables => emptySlots.Count;

    /// <summary>
    /// The number of collectables in total.
    /// </summary>
    private int totalCollectables;



    void Start()
    {
        audioManager = AudioManager.Instance;

        // find all collectables
        var collectables = FindObjectsOfType<Collectable>();
        totalCollectables = collectables.Length;
        foreach (var collectable in collectables)
        {
            collectable.tracker = this;

            var slotObject = Instantiate(collectableSlotPrefab, parent: transform);
            emptySlots.Add(slotObject.GetComponent<CollectableSlotUI>());
        }

        GameObject particleSystem = Instantiate(particleEffect, transform.position, Quaternion.identity);
        ps = particleSystem.GetComponent<ParticleSystem>();
    }



    /// <summary>
    /// Called when a collectable has been collected.
    /// </summary>
    public void CollectOne()
    {
        if (audioManager != null) { audioManager.PlaySFX(AudioManager.ClipName.LevelUp, 8.0f); }

        var targetSlot = emptySlots[0];
        emptySlots.RemoveAt(0);

        targetSlot.TriggerCollected();
    }

    /// <summary>
    /// Emits particles at the given position.
    /// </summary>
    /// <param name="position"></param>
    public void EmitParticles(Vector3 position)
    {
        ps.transform.position = position;
        ps.Play();
    }
}
