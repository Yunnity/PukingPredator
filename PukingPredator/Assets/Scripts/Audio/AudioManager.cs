using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

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
        Eating
    }

    protected override void Awake()
    {
        base.Awake();

        backgroundSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
    }


    public void PlayBackground(float volume = 1.0f, bool loop = true)
    {
        backgroundSource.clip = backgroundTrack;
        backgroundSource.volume = volume;
        backgroundSource.loop = loop;
        backgroundSource.Play();
    }

    public void StopMusic()
    {
        backgroundSource.Stop();
        sfxSource.Stop();
    }

    public void PlaySFX(ClipName name, float volume = 1.0f)
    {
        // sample usage AudioManager.Instance.PlaySFX("vomit", 1.0f);
        AudioClip clip = sfxClips[(int) name];
        if (sfxSource.clip == clip) sfxSource.Stop();
        sfxSource.PlayOneShot(clip, volume);
    }

    public void SetMusicVolume(float volume)
    {
        backgroundSource.volume = volume;
    }
}
