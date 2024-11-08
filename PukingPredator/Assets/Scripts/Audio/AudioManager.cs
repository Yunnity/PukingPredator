using UnityEngine;
using System.Collections.Generic;

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField]
    private List<AudioClip> sfxClips;

    [SerializeField]
    private AudioClip backgroundTrack;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    public enum ClipName
    {
        LevelUp,
        Rainfall,
        Villian,
        Puking,
        Eating,
        Puke,
        PukeForce,
    }
    public Dictionary<ClipName, float> relativeVolumes = new()
    {
        { ClipName.LevelUp, 8f },
        { ClipName.Rainfall, 1f },
        { ClipName.Villian, 1f },
        { ClipName.Puking, 1f },
        { ClipName.Eating, 2f },
        { ClipName.Puke, 1f },
        { ClipName.PukeForce, 8f },
    };

    protected override void Awake()
    {
        base.Awake();

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }


    public void PlayBackground(bool loop = true)
    {
        if (backgroundSource.isPlaying) { return; }

        backgroundSource.clip = backgroundTrack;
        backgroundSource.loop = loop;
        backgroundSource.Play();
    }

    public void StopMusic()
    {
        backgroundSource.Stop();
        sfxSource.Stop();
    }

    public void PlaySFX(ClipName name)
    {
        // sample usage AudioManager.Instance.PlaySFX(ClipName.Eating);

        var volume = relativeVolumes.ContainsKey(name) ? relativeVolumes[name] : 1f;
        volume *= GameSettings.volumeMaster * GameSettings.volumeSFX;
        if (volume <= 0) { return; }
        
        AudioClip clip = sfxClips[(int) name];
        if (sfxSource.clip == clip) { sfxSource.Stop(); }
        sfxSource.PlayOneShot(clip, volume);
    }

    public static void UpdateMusicVolume()
    {
        Instance.backgroundSource.volume = GameSettings.volumeMaster * GameSettings.volumeMusic;
    }
}
