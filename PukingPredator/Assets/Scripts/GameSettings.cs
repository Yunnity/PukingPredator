public static class GameSettings
{
    //TODO: we should probably make a way to reset the settings in the future.

    public static float cameraSensitivity = 1f;

    private static float _volumeMaster = 0.5f;
    public static float volumeMaster
    {
        get => _volumeMaster;
        set
        {
            _volumeMaster = value;
            AudioManager.UpdateMusicVolume();
        }
    }
    private static float _volumeMusic = 0.5f;
    public static float volumeMusic
    {
        get => _volumeMusic;
        set
        {
            _volumeMusic = value;
            AudioManager.UpdateMusicVolume();
        }
    }
    public static float volumeSFX = 0.5f;

}

