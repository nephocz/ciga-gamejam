using System;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [Serializable]
    private class SFXEntry
    {
        public SFXType type;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public float pitch = 1f;
    }

    [SerializeField] private AudioSource oneShotSource;
    [SerializeField] private SFXEntry[] sfxEntries;
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;

    private static SFXManager instance;
    private readonly Dictionary<SFXType, SFXEntry> entryByType = new Dictionary<SFXType, SFXEntry>();
    private readonly Dictionary<SFXType, AudioSource> loopingSources = new Dictionary<SFXType, AudioSource>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (oneShotSource == null)
        {
            oneShotSource = GetComponent<AudioSource>();
        }

        if (oneShotSource == null)
        {
            oneShotSource = gameObject.AddComponent<AudioSource>();
        }

        oneShotSource.playOnAwake = false;
        oneShotSource.loop = false;

        BuildLookup();
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            BuildLookup();
        }
    }

    public static void Play(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.PlayOneShot(type);
    }

    public static void StartLoop(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.PlayLoop(type);
    }

    public static void StopLoop(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.StopLoopingSource(type);
    }

    private void PlayOneShot(SFXType type)
    {
        if (!TryGetEntry(type, out SFXEntry entry))
        {
            return;
        }

        oneShotSource.pitch = entry.pitch;
        oneShotSource.PlayOneShot(entry.clip, entry.volume * masterVolume);
    }

    private void PlayLoop(SFXType type)
    {
        if (!TryGetEntry(type, out SFXEntry entry))
        {
            return;
        }

        if (!loopingSources.TryGetValue(type, out AudioSource loopSource) || loopSource == null)
        {
            loopSource = gameObject.AddComponent<AudioSource>();
            loopingSources[type] = loopSource;
        }

        if (loopSource.isPlaying && loopSource.clip == entry.clip)
        {
            return;
        }

        loopSource.clip = entry.clip;
        loopSource.volume = entry.volume * masterVolume;
        loopSource.pitch = entry.pitch;
        loopSource.loop = true;
        loopSource.playOnAwake = false;
        loopSource.Play();
    }

    private void StopLoopingSource(SFXType type)
    {
        if (!loopingSources.TryGetValue(type, out AudioSource loopSource) || loopSource == null)
        {
            return;
        }

        loopSource.Stop();
    }

    private bool TryGetEntry(SFXType type, out SFXEntry entry)
    {
        if (!entryByType.TryGetValue(type, out entry) || entry.clip == null)
        {
            return false;
        }

        return true;
    }

    private void BuildLookup()
    {
        entryByType.Clear();

        foreach (SFXEntry entry in sfxEntries)
        {
            if (entry == null)
            {
                continue;
            }

            entryByType[entry.type] = entry;
        }
    }
}
