
using System;

public class PlayerState
{
    /// <summary>
    /// Triggered when entering this state.
    /// </summary>
    public Action onEnter;

    /// <summary>
    /// Triggered when exiting this state.
    /// </summary>
    public Action onExit;

    /// <summary>
    /// Triggered each update while in this state.
    /// </summary>
    public Action onUpdate;
}
