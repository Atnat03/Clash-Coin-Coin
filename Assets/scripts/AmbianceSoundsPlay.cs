using UnityEngine;

public class AmbianceSoundsPlay : MonoBehaviour
{
    public AudioClip ambiancePond;
    public AudioClip ambianceFrog;
    public AudioClip ambianceDuck;
    public AudioSource ambiancePondSource;
    public AudioSource ambianceFrogSource;
    public AudioSource ambianceDuckSource;
    void Start()
    {
        PlaySound(ambiancePond,0.6f,ambiancePondSource);
        PlaySound(ambianceFrog,0.2f,ambianceFrogSource);
        PlaySound(ambianceDuck,0.2f,ambianceDuckSource);
    }

    public void PlaySound(AudioClip clip, float volume, AudioSource source)
    {
        source.pitch = 1f;
        source.PlayOneShot(clip, volume);
    }
}
