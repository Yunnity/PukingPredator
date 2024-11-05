public class LevelCompletionData
{
    /// <summary>
    /// If the level is done.
    /// </summary>
    public bool isDone;
    /// <summary>
    /// How many collectables were found.
    /// </summary>
    public int collectableCount;

    public LevelCompletionData(int collectableCount, bool isDone = true)
    {
        this.isDone = isDone;
        this.collectableCount = collectableCount;
    }
}