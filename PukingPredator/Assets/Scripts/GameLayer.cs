using UnityEngine;

public static class GameLayer
{
    /// <summary>
    /// For consumable objects in the world.
    /// </summary>
    public static int consumable { get; } = LayerMask.NameToLayer("Consumable");

    /// <summary>
    /// For things being consumed, being puked, or in the inventory.
    /// </summary>
    public static int consumed { get; } = LayerMask.NameToLayer("Consumed");
}
