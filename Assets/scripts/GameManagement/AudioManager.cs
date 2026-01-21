using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    [Header("Audio sources")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    
    [Header("music Audio clips")]
    public AudioClip[] musicClip;

    [Header("SFX Audio clips")]
    public AudioClip startSound;
    public AudioClip endSound;
    public AudioClip coin;
    public AudioClip croa;
    void Awake()
    {
        if (instance == null) 
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayRandomMusic()
    {
        musicSource.PlayOneShot(musicClip[Random.Range(0, musicClip.Length)]);
    }

    public void PlaySound(AudioClip clip)
    {
        SFXSource.pitch = 1f;
        SFXSource.PlayOneShot(clip);
    }

    public void PlaySound(AudioClip clip, float volume)
    {
        SFXSource.pitch = 1f;
        SFXSource.PlayOneShot(clip, volume);
    }
    
    public void PlaySoundRandomPitch(AudioClip clip, float pitchMin, float pitchMax)
    {
        SFXSource.pitch = Random.Range((float)pitchMin, (float)pitchMax);
        SFXSource.PlayOneShot(clip);
    }
    
    public void PlaySoundRandomPitch(AudioClip clip,float volume, float pitchMin, float pitchMax)
    {
        SFXSource.pitch = Random.Range(pitchMin, pitchMax);
        SFXSource.PlayOneShot(clip, volume);
    }

    public void PlaySoundRandowPitch(AudioClip clip)
    {
        SFXSource.pitch = Random.Range(0.8f, 1.2f);
        SFXSource.PlayOneShot(clip);
    }

    public void PlaySoundRandowPitch(AudioClip clip, float volume)
    {
        SFXSource.pitch = Random.Range(0.8f, 1.2f);
        SFXSource.PlayOneShot(clip, volume);

    }
}