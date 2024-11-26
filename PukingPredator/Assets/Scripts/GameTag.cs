public static class GameTag
{
    /// <summary>
    /// Used on objects that can open locks.
    /// </summary>
    public static string key { get; } = "Key";

    /// <summary>
    /// The main camera that follows the player.
    /// </summary>
    public static string mainCamera { get; } = "MainCamera";

    /// <summary>
    /// Only the player object will have this tag.
    /// </summary>
    public static string player { get; } = "Player";
}


