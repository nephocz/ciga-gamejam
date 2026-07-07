// ========================================================================
// 文件功能：音效管理器（SFX Manager）
// 作为全局单例管理游戏中的所有音效（一次性音效与循环音效）。
// 通过 AudioSource 组件播放音效，支持一次性播放（PlayOneShot）和循环播放/停止。
// 通过静态方法 Play、StartLoop、StopLoop 供其他脚本调用，并在 Inspector 中配置音效库与主音量。
// ========================================================================

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

    /// <summary>
    /// Unity 生命周期函数 Awake，在脚本实例加载时调用。
    /// 实现单例模式：若已存在实例则销毁自身；否则将自身设为跨场景持久对象（DontDestroyOnLoad）。
    /// 若未在面板指定 oneShotSource，则先通过 GetComponent 获取，若仍无则动态添加 AudioSource 组件并配置为一次性播放源。
    /// 最后调用 BuildLookup 构建音效类型到条目的字典查找表。
    /// 可在组件面板中显示的字段：oneShotSource（一次性播放音源）、sfxEntries（音效条目数组）、masterVolume（主音量）。
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

    /// <summary>
    /// Unity 编辑器验证回调 OnValidate，在 Inspector 值变更或脚本加载时调用。
    /// 若应用正在运行，则调用 BuildLookup 重新构建查找表，确保编辑器中的修改即时生效。
    /// </summary>
    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            BuildLookup();
        }
    }

    /// <summary>
    /// 公开静态方法：播放指定类型的一次性音效。
    /// 通过单例实例调用 PlayOneShot 方法，使用 AudioSource.PlayOneShot 播放。
    /// 无在组件面板中显示的字段（静态方法，操作实例的 oneShotSource、sfxEntries 和 masterVolume）。
    /// </summary>
    public static void Play(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.PlayOneShot(type);
    }

    /// <summary>
    /// 公开静态方法：开始循环播放指定类型的音效。
    /// 通过单例实例调用 PlayLoop 方法，创建或复用专用的循环 AudioSource 并播放。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public static void StartLoop(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.PlayLoop(type);
    }

    /// <summary>
    /// 公开静态方法：停止指定类型的循环音效。
    /// 通过单例实例调用 StopLoopingSource 方法，停止并移除对应的循环 AudioSource。
    /// 无在组件面板中显示的字段。
    /// </summary>
    public static void StopLoop(SFXType type)
    {
        if (instance == null)
        {
            return;
        }

        instance.StopLoopingSource(type);
    }

    /// <summary>
    /// 内部方法：播放一次性音效。
    /// 通过 TryGetEntry 获取对应类型的 SFXEntry，设置 oneShotSource 的音高，
    /// 然后调用 AudioSource.PlayOneShot(AudioClip, float) 播放该音效片段，音量 = 条目音量 * 主音量。
    /// 使用面板字段：oneShotSource、sfxEntries（经由 entryByType 查询）、masterVolume。
    /// </summary>
    private void PlayOneShot(SFXType type)
    {
        if (!TryGetEntry(type, out SFXEntry entry))
        {
            return;
        }

        oneShotSource.pitch = entry.pitch;
        oneShotSource.PlayOneShot(entry.clip, entry.volume * masterVolume);
    }

    /// <summary>
    /// 内部方法：开始循环播放音效。
    /// 如果该类型尚未有循环 AudioSource 则动态添加一个，配置 clip、音量（条目音量 * 主音量）、音高、循环开启并播放。
    /// 若相同音效已在播放则忽略重复请求。
    /// 使用面板字段：sfxEntries、masterVolume。
    /// </summary>
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

    /// <summary>
    /// 内部方法：停止指定类型的循环音效。
    /// 从字典中获取对应的 AudioSource 并调用 Stop() 停止播放。
    /// 使用字段：loopingSources。
    /// </summary>
    private void StopLoopingSource(SFXType type)
    {
        if (!loopingSources.TryGetValue(type, out AudioSource loopSource) || loopSource == null)
        {
            return;
        }

        loopSource.Stop();
    }

    /// <summary>
    /// 内部辅助方法：尝试从查找字典中获取指定类型的 SFXEntry。
    /// 若存在且 clip 不为空则返回 true，否则 false。
    /// 使用字段：entryByType（通过 sfxEntries 构建）。
    /// </summary>
    private bool TryGetEntry(SFXType type, out SFXEntry entry)
    {
        if (!entryByType.TryGetValue(type, out entry) || entry.clip == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// 构建/重建音效类型到条目的字典查找表。
    /// 遍历 sfxEntries 数组，以 SFXType 为键、SFXEntry 为值存入 entryByType 字典。
    /// 在 Awake 和 OnValidate（运行时）中调用，确保查找表与面板数据同步。
    /// </summary>
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