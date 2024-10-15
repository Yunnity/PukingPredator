using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> sfxClips;

    [SerializeField]
    private AudioClip backgroundTrack;

    private AudioSource backgroundSource;
    private AudioSource sfxSource;

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Loads sfx into a dictionary
            foreach (AudioClip clip in sfxClips)
            {
                sfxDictionary[clip.name] = clip;
            }

            backgroundSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
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
            sfxSource.Stop();
            sfxSource.PlayOneShot(sfxDictionary[clipName], volume);
        }
    }

    public void SetMusicVolume(float volume)
    {
        backgroundSource.volume = volume;
    }
}
