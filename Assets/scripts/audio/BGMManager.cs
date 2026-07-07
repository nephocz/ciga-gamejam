// ========================================================================
// 文件功能：背景音乐管理器
// 作为持久化单例，根据当前活动场景自动切换背景音乐，并支持音乐之间的淡入淡出。
// 通过 SceneManager.sceneLoaded 事件监听场景加载，使用 AudioSource 播放音乐，
// 在协程中利用 Mathf.Lerp 控制音量渐变，根据面板配置的 sceneMusics 映射场景与音轨。
// 在组件面板中可配置音频源、场景音乐列表、音量、淡入淡出时长及是否使用未缩放时间。
// ========================================================================

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

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 实现单例模式：若已存在实例则销毁自身；否则将自身设为持久对象（DontDestroyOnLoad）。
    /// 若未在面板指定 AudioSource，则先尝试从当前物体获取，再动态添加组件；
    /// 并设置音频源为循环播放、禁止唤醒时播放、初始音量为面板配置值。
    /// 在组件面板中可显示的字段：audioSource、volume。
    /// </summary>
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

    /// <summary>
    /// Unity 生命周期函数 OnEnable，在对象启用时调用。
    /// 通过 SceneManager.sceneLoaded += OnSceneLoaded 注册场景加载事件回调，
    /// 使得每次新场景加载后能自动切换背景音乐。
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unity 生命周期函数 OnDisable，在对象禁用时调用。
    /// 取消订阅场景加载事件，防止内存泄漏。
    /// </summary>
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// Unity 生命周期函数 Start，在首次 Update 之前调用。
    /// 通过 SceneManager.GetActiveScene().name 获取当前活动场景名称，
    /// 调用 PlayMusicForScene 播放对应背景音乐。
    /// </summary>
    private void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// 场景加载事件回调，当场景加载完成时触发。
    /// 若加载模式为叠加（Additive）则忽略，避免副场景误触音乐切换；
    /// 否则使用加载的场景名称调用 PlayMusicForScene。
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            return;
        }

        PlayMusicForScene(scene.name);
    }

    /// <summary>
    /// 根据场景名称播放对应的背景音乐。
    /// 通过 GetMusicForScene 查找面板配置的 sceneMusics 中匹配的场景音乐；
    /// 若目标音乐与当前正在播放的音乐相同且未播放，则直接恢复音量并播放；
    /// 否则调用 StartFadeToClip 启动淡入淡出协程切换到新音乐。
    /// 在组件面板中可显示的字段：sceneMusics、volume。
    /// </summary>
    private void PlayMusicForScene(string sceneName)
    {
        AudioClip targetClip = GetMusicForScene(sceneName);

        if (targetClip == null)
        {
            return;
        }

        if (audioSource.clip == targetClip)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.volume = volume;
                audioSource.Play();
            }

            return;
        }

        StartFadeToClip(targetClip);
    }

    /// <summary>
    /// 启动切换到新音乐的淡入淡出流程。
    /// 停止当前正在运行的淡入淡出协程，然后启动 FadeToClip 协程。
    /// </summary>
    private void StartFadeToClip(AudioClip targetClip)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeToClip(targetClip));
    }

    /// <summary>
    /// 协程：执行音乐切换的淡入淡出。
    /// 若当前正在播放音乐，先通过 FadeVolume(0f) 将音量渐降至 0；
    /// 然后更换 AudioSource.clip 为目标音乐，以 0 音量启动播放，
    /// 再通过 FadeVolume(volume) 将音量渐升至面板配置的目标音量。
    /// 淡入淡出完成后将 fadeRoutine 置空。
    /// 在组件面板中可显示的字段：fadeDuration、volume、useUnscaledTime（影响 FadeVolume）。
    /// </summary>
    private IEnumerator FadeToClip(AudioClip targetClip)
    {
        if (audioSource.isPlaying)
        {
            yield return FadeVolume(0f);
        }

        audioSource.clip = targetClip;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeVolume(volume);
        fadeRoutine = null;
    }

    /// <summary>
    /// 协程：在 fadeDuration 时长内将 AudioSource 音量渐变到目标值。
    /// 使用 Mathf.Lerp 在起始音量和目标音量之间线性插值，时间增量依据 useUnscaledTime
    /// 决定使用 Time.unscaledDeltaTime 还是 Time.deltaTime；
    /// 若 fadeDuration <= 0 则直接设置目标音量。
    /// 在组件面板中可显示的字段：fadeDuration、useUnscaledTime。
    /// </summary>
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

    /// <summary>
    /// 根据场景名称在面板配置的 sceneMusics 数组中查找对应的 AudioClip。
    /// 遍历数组，若场景名匹配则返回音乐资源，否则返回 null。
    /// 在组件面板中可显示的字段：sceneMusics。
    /// </summary>
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