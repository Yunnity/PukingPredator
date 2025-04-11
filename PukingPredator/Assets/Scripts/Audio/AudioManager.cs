using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// The audio IDs that should be used outside of the audio manager to
/// reference a specific audio event.
/// </summary>
public enum AudioID
{
    Eat,
    Puke,
    ChargePuke,
    Jump,
    Walk,
    GotCollectable,
    Death,
    BecameLighter,
    BecameHeavier,
    MaxInventory,
    CannotEat,
    Portal,
    /// <summary>
    /// Played when entering a portal.
    /// </summary>
    TeleportIn,
    /// <summary>
    /// Played when exiting a portal.
    /// </summary>
    TeleportOut,
}

public enum MusicID
{
    Title,
    Game
}

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField]
    private List<AudioClip> sounds;

    [SerializeField]
    private List<AudioClip> backgroundTracks;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    private AudioID currentSfx;

    

    public Dictionary<AudioID, float> relativeVolumes = new()
    {
        { AudioID.Eat, 1f },
    };

    protected override void Awake()
    {
        base.Awake();

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }


    public void PlayBackground(MusicID music, bool loop = true)
    {
        if (backgroundSource.isPlaying) { return; }
        backgroundSource.Stop();
        backgroundSource.clip = backgroundTracks[(int) music];
        backgroundSource.loop = loop;
        backgroundSource.Play();
    }

    public void StopMusic()
    {
        backgroundSource.Stop();
        sfxSource.Stop();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="audioID"></param>
    /// <param name="wait">If it should wait for other instances of the same
    /// sound to stop before playing it.</param>
    public void PlaySFX(AudioID audioID, bool wait = false)
    {
        var volume = relativeVolumes.ContainsKey(audioID) ? relativeVolumes[audioID] : 1f;
        volume *= GameSettings.volumeMaster * GameSettings.volumeSFX;
        if (volume <= 0) { return; }
        
        // Waits for the sound effect to finish
        if (wait && sfxSource.isPlaying && currentSfx == audioID) { return; }
        if (currentSfx == audioID) { sfxSource.Stop(); }

        AudioClip clip = sounds[(int)audioID];
        sfxSource.PlayOneShot(clip, volume);
        currentSfx = audioID;
    }

    public static void UpdateMusicVolume()
    {
        Instance.backgroundSource.volume = GameSettings.volumeMaster * GameSettings.volumeMusic;
    }
}
