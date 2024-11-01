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

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();


    protected override void Awake()
    {
        base.Awake();

        // Loads sfx into a dictionary
        foreach (AudioClip clip in sfxClips)
        {
            sfxDictionary[clip.name] = clip;
        }

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

    public void PlaySFX(string clipName, float volume = 1.0f)
    {
        // sample usage AudioManager.Instance.PlaySFX("vomit", 1.0f);
        if (sfxDictionary.ContainsKey(clipName))
        {
            AudioClip clip = sfxDictionary[clipName];
            if (sfxSource.clip == clip) sfxSource.Stop();
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        backgroundSource.volume = volume;
    }
}
