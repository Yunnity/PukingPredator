using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AudioManager : SingletonMonobehaviour<AudioManager>
{
    [SerializeField]
    private List<AudioClip> sfxClips;

    [SerializeField]
    private List<AudioClip> backgroundTracks;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    private AudioSource walkingSource;

    public enum ClipName
    {
        LevelUp,
        Rainfall,
        Villian,
        Puking,
        Eating,
        Puke,
        PukeForce,
        Walking,
    }

    public enum MusicName
    {
        Title,
        Game
    }

    public Dictionary<ClipName, float> relativeVolumes = new()
    {
        { ClipName.LevelUp, 8f },
        { ClipName.Rainfall, 1f },
        { ClipName.Villian, 1f },
        { ClipName.Puking, 1f },
        { ClipName.Eating, 2f },
        { ClipName.Puke, 0.2f },
        { ClipName.PukeForce, 8f },
        { ClipName.Walking, 1f },
    };

    protected override void Awake()
    {
        base.Awake();

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        walkingSource = gameObject.AddComponent<AudioSource>();
        walkingSource.clip = sfxClips[(int) ClipName.Walking];
        walkingSource.loop = true;
        walkingSource.volume = 0.1f;
    }


    public void PlayBackground(MusicName music, bool loop = true)
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

    public void SetWalking(bool isWalking)
    {
        walkingSource.volume = sfxSource.isPlaying ? 0f : 0.1f;
        if (!walkingSource.isPlaying && isWalking)
        {
            walkingSource.Play();
        } else if (walkingSource.isPlaying && !isWalking)
        {
            walkingSource.Stop();
        }
    }

    public static void UpdateMusicVolume()
    {
        Instance.backgroundSource.volume = GameSettings.volumeMaster * GameSettings.volumeMusic;
    }
}
