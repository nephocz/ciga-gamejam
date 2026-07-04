using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    [Serializable]
    private class SceneMusic
    {
        public string sceneName;
        public AudioClip music;
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private SceneMusic[] sceneMusics;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [SerializeField, Min(0f)] private float fadeDuration = 1f;
    [SerializeField] private bool useUnscaledTime = true;

    private static BGMManager instance;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            return;
        }

        PlayMusicForScene(scene.name);
    }

    private void PlayMusicForScene(string sceneName)
    {
        AudioClip targetClip = GetMusicForScene(sceneName);

        if (audioSource.clip == targetClip && audioSource.isPlaying)
        {
            StartFadeToVolume(volume);
            return;
        }

        StartFadeToClip(targetClip);
    }

    private void StartFadeToClip(AudioClip targetClip)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeToClip(targetClip));
    }

    private void StartFadeToVolume(float targetVolume)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeVolume(targetVolume));
    }

    private IEnumerator FadeToClip(AudioClip targetClip)
    {
        if (audioSource.isPlaying)
        {
            yield return FadeVolume(0f);
        }

        if (targetClip == null)
        {
            audioSource.Stop();
            audioSource.clip = null;
            fadeRoutine = null;
            yield break;
        }

        audioSource.clip = targetClip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeVolume(volume);
        fadeRoutine = null;
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        if (fadeDuration <= 0f)
        {
            audioSource.volume = targetVolume;
            yield break;
        }

        while (elapsed < fadeDuration)
        {
            elapsed += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    private AudioClip GetMusicForScene(string sceneName)
    {
        foreach (SceneMusic sceneMusic in sceneMusics)
        {
            if (sceneMusic.sceneName == sceneName)
            {
                return sceneMusic.music;
            }
        }

        return null;
    }
}
