using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 音频管理器，负责背景音乐和音效的播放、暂停等控制
public class AudioManager : MonoBehaviour {

    // 单例实例，全局唯一访问点
    public static AudioManager Instance { get; private set; }

    // 用于播放背景音乐的音频源
    private AudioSource bgmAudioSource;

    // 音效的音量
    private float clipVolume = 0.4f;

    // 初始化单例和获取音频源组件
    private void Awake() {
        Instance = this;
        bgmAudioSource = GetComponent<AudioSource>();
    }

    // 选择指定路径的背景音乐并播放
    public void PlayBgm(string path) {
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.Play();
    }

    // 播放当前设置好的背景音乐
    public void PlayBgm() {
        bgmAudioSource.Play();
    }

    // 暂停当前播放的背景音乐
    public void StopBgm() {
        bgmAudioSource.Pause();
    }

    // 选择指定路径的音效并播放，使用对象池管理音频源对象
    public void PlayClip(string path) {
        GameObject obj = ObjectPoolManager.Instance.GetSource();
        AudioSource source = obj.GetComponent<AudioSource>();

        source.volume = clipVolume;
        source.ignoreListenerPause = true;

        AudioClip audioClip = Resources.Load<AudioClip>(path);
        source.clip = audioClip;
        source.Play();

        StartCoroutine(ReleaseSource(obj, audioClip.length));
    }

    // 协程，等待音效播放完成后，将音频源对象放回对象池
    IEnumerator ReleaseSource(GameObject obj, float length) {
        yield return new WaitForSeconds(length);
        ObjectPoolManager.Instance.ReleaseSource(obj);
    }

    // 响应背景音乐音量滑块值变化，修改背景音乐音量
    public void OnSliderValueChangeMusic(float value) {
        bgmAudioSource.volume = value;
    }

    // 响应音效音量滑块值变化，修改音效音量
    public void OnSliderValueChangeClip(float value) {
        clipVolume = value;
    }
}