using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��������UI�������ղ���ʾ��
public class EndUI : MonoBehaviour {

    private Animator animator; // ���ƽ��涯��

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // ��ʼ����
    }

    // ���ؽ��棨���ö��������
    public void Hide() {
        animator.enabled = false;
    }

    // ��ʾ���棨���ö��������
    public void Show() {
        animator.enabled = true;
    }

    // ����"����"��Ч�������¼����ã�
    public void PlayHugeWaveAudio() {
        AudioManager.Instance.PlayClip(Config.hugewave);
    }

    // ����"���ղ�"��Ч�������¼����ã�
    public void PlayFianlWaveAudio() {
        AudioManager.Instance.PlayClip(Config.finalwave);
    }
}