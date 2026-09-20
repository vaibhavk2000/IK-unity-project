using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundEffect", menuName = "Combat Audio/Sound Effect Configuration")]
public class SoundEffectSO : ScriptableObject
{
    [Header("Primary Audio Clips")]
    public AudioClip[] primaryClips;

    [Header("Secondary Layer Clips")]
    public AudioClip[] secondaryLayerClips;

    [Header("Randomization Ranges")]
    [Range(0.1f, 1.0f)] public float volumeMin = 0.85f;
    [Range(0.1f, 1.0f)] public float volumeMax = 1.0f;
    
    [Range(0.5f, 1.5f)] public float pitchMin = 0.92f;
    [Range(0.5f, 1.5f)] public float pitchMax = 1.08f;

    public AudioClip GetRandomPrimaryClip()
    {
        if (primaryClips == null || primaryClips.Length == 0) return null;
        return primaryClips[Random.Range(0, primaryClips.Length)];
    }

    public AudioClip GetRandomSecondaryClip()
    {
        if (secondaryLayerClips == null || secondaryLayerClips.Length == 0) return null;
        return secondaryLayerClips[Random.Range(0, secondaryLayerClips.Length)];
    }
}
