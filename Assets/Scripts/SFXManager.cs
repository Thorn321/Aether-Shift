using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    public AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, float pitch)
    {
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip);
    }

    public void PlaySoundRandomPitch(AudioClip clip, float minPitch, float maxPitch)
    {
        float pitch = Random.Range(minPitch, maxPitch);
        PlaySound(clip, pitch);
    }
}