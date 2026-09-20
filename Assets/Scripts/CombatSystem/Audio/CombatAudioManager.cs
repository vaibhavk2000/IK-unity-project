using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatAudioManager : MonoBehaviour
{
    public static CombatAudioManager Instance { get; private set; }

    [Header("Audio Source Pool Setup")]
    [SerializeField] private int poolSize = 16;
    private List<AudioSource> sourcePool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePool();
    }

    private void InitializePool()
    {
        sourcePool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject($"CombatAudioSource_{i}");
            obj.transform.SetParent(transform);
            
            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1.0f;
            source.minDistance = 2f;
            source.maxDistance = 25f;
            source.rolloffMode = AudioRolloffMode.Logarithmic;

            sourcePool.Add(source);
        }
    }

    private AudioSource GetAvailableAudioSource()
    {
        foreach (var source in sourcePool)
        {
            if (!source.isPlaying) return source;
        }

        GameObject newObj = new GameObject($"CombatAudioSource_{sourcePool.Count}");
        newObj.transform.SetParent(transform);
        AudioSource newSource = newObj.AddComponent<AudioSource>();
        newSource.spatialBlend = 1.0f;
        sourcePool.Add(newSource);
        return newSource;
    }

    public void Play3DSFX(SoundEffectSO sfxConfig, Vector3 position)
    {
        if (sfxConfig == null) return;

        AudioClip primaryClip = sfxConfig.GetRandomPrimaryClip();
        if (primaryClip != null)
        {
            AudioSource primarySource = GetAvailableAudioSource();
            ConfigureAndPlaySource(primarySource, primaryClip, sfxConfig, position);
        }

        AudioClip secondaryClip = sfxConfig.GetRandomSecondaryClip();
        if (secondaryClip != null)
        {
            AudioSource secondarySource = GetAvailableAudioSource();
            ConfigureAndPlaySource(secondarySource, secondaryClip, sfxConfig, position);
        }
    }

    private void ConfigureAndPlaySource(AudioSource source, AudioClip clip, SoundEffectSO config, Vector3 position)
    {
        source.transform.position = position;
        source.clip = clip;

        source.volume = Random.Range(config.volumeMin, config.volumeMax);
        source.pitch = Random.Range(config.pitchMin, config.pitchMax);

        source.Play();
    }
}
