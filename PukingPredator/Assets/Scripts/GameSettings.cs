using System;

public enum GameSettingFloat
{
    cameraSensitivity,
    volumeMaster,
    volumeMusic,
    volumeSFX,
}
public static class GameSettings
{
    //TODO: we should probably make a way to reset the settings in the future.

    /// <summary>
    /// Not an actual setting, just here to toggle it in the code.
    /// If true, you will be able to eat while full, but it will delete the
    /// first item eaten.
    /// If false, you just can't eat while full.
    /// </summary>
    public static bool canEatWhileFull = true;

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

    public static float GetFloatSetting(GameSettingFloat gs)
    {
        switch(gs)
        {
            case GameSettingFloat.cameraSensitivity:
                return cameraSensitivity;
            case GameSettingFloat.volumeMaster:
                return volumeMaster;
            case GameSettingFloat.volumeMusic:
                return volumeMusic;
            case GameSettingFloat.volumeSFX:
                return volumeSFX;
            default:
                throw new NotSupportedException("Invalid setting type.");
        }
    }
    public static void SetFloatSetting(GameSettingFloat gs, float value)
    {
        switch (gs)
        {
            case GameSettingFloat.cameraSensitivity:
                cameraSensitivity = value;
                break;
            case GameSettingFloat.volumeMaster:
                volumeMaster = value;
                break;
            case GameSettingFloat.volumeMusic:
                volumeMusic = value;
                break;
            case GameSettingFloat.volumeSFX:
                volumeSFX = value;
                break;
            default:
                throw new NotSupportedException("Invalid setting type.");
        }
    }
}

