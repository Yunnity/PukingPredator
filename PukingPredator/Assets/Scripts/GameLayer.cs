using UnityEngine;

public static class GameLayer
{
    /// <summary>
    /// For consumable objects being shot back into the world.
    /// </summary>
    public static int beingPuked { get; } = LayerMask.NameToLayer("BeingPuked");

    /// <summary>
    /// For consumable objects in the world.
    /// </summary>
    public static int consumable { get; } = LayerMask.NameToLayer("Consumable");

    /// <summary>
    /// For things being consumed, being puked, or in the inventory.
    /// </summary>
    public static int consumed { get; } = LayerMask.NameToLayer("Consumed");

    /// <summary>
    /// Gets the layermask for a given layer.
    /// </summary>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int GetLayerMask(int layer)
    {
        int layerMask = 0;

        // Iterate through all layers (0 to 31 in Unity)
        for (int i = 0; i < 32; i++)
        {
            // Check if the layer `i` is set to collide with the object's layer in the physics collision matrix
            if (Physics.GetIgnoreLayerCollision(layer, i) == false)
            {
                layerMask |= (1 << i); // Add this layer to the mask
            }
        }

        return layerMask;
    }
}
