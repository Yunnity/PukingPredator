using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    public AudioSource backgroundMusic;
    public AudioSource sfxSource;
    public List<AudioClip> sfxClips;

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        // Loads sfx into a dictionary
        foreach (AudioClip clip in sfxClips)
        {
            sfxDictionary[clip.name] = clip;
        }
    }


    public void PlayBackground(AudioClip clip, float volume = 1.0f, bool loop = true)
    {
        backgroundMusic.clip = clip;
        backgroundMusic.volume = volume;
        backgroundMusic.loop = loop;
        backgroundMusic.Play();
    }

    public void StopMusic()
    {
        backgroundMusic.Stop();
        sfxSource.Stop();
    }

    public void PlaySFX(string clipName, float volume = 1.0f)
    {
        if (sfxDictionary.ContainsKey(clipName))
        {
            sfxSource.PlayOneShot(sfxDictionary[clipName], volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        backgroundMusic.volume = volume;
    }
}
