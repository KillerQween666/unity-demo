using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ƶ�����������𱳾����ֺ���Ч�Ĳ��š���ͣ�ȿ���
public class AudioManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ�
    public static AudioManager Instance { get; private set; }

    // ���ڲ��ű������ֵ���ƵԴ
    private AudioSource bgmAudioSource;

    // ��Ч������
    private float clipVolume = 0.4f;

    // ��ʼ�������ͻ�ȡ��ƵԴ���
    private void Awake() {
        Instance = this;
        bgmAudioSource = GetComponent<AudioSource>();
    }

    // ѡ��ָ��·���ı������ֲ�����
    public void PlayBgm(string path) {
        AudioClip audioClip = Resources.Load<AudioClip>(path);
        bgmAudioSource.clip = audioClip;
        bgmAudioSource.Play();
    }

    // ���ŵ�ǰ���úõı�������
    public void PlayBgm() {
        bgmAudioSource.Play();
    }

    // ��ͣ��ǰ���ŵı�������
    public void StopBgm() {
        bgmAudioSource.Pause();
    }

    // ѡ��ָ��·������Ч�����ţ�ʹ�ö���ع�����ƵԴ����
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

    // Э�̣��ȴ���Ч������ɺ󣬽���ƵԴ����Żض����
    IEnumerator ReleaseSource(GameObject obj, float length) {
        yield return new WaitForSeconds(length);
        ObjectPoolManager.Instance.ReleaseSource(obj);
    }

    // ��Ӧ����������������ֵ�仯���޸ı�����������
    public void OnSliderValueChangeMusic(float value) {
        bgmAudioSource.volume = value;
    }

    // ��Ӧ��Ч��������ֵ�仯���޸���Ч����
    public void OnSliderValueChangeClip(float value) {
        clipVolume = value;
    }
}