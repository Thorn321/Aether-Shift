using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup sfxGroup;

    private void Awake()
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

    public void PlaySound(AudioClip clip, float pitch = 1f, float volume = 1f)
    {
        if (clip == null)
            return;

        GameObject soundObject = new GameObject("SFX_" + clip.name);

        AudioSource source = soundObject.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;

        source.outputAudioMixerGroup = sfxGroup;

        source.Play();

        Destroy(soundObject, clip.length / Mathf.Abs(pitch));
    }

    public void PlaySoundRandomPitch(AudioClip clip, float minPitch, float maxPitch, float volume = 1f)
    {
        float randomPitch = Random.Range(minPitch, maxPitch);

        PlaySound(clip, randomPitch, volume);
    }
}