using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ϸ�����ܽ��棨ʤ��/ʧ�ܺ���ʾ��
public class GameEndUI : MonoBehaviour {
    public GameObject gameEndImage; // ��������ͼƬ
    public bool isGameEnd = false;  // ��Ϸ�Ƿ�����ı��

    private void Start() {
        Hide(); // ��ʼ����
    }

    // ��ʾ��������
    public void Show() {
        isGameEnd = true;
        AudioManager.Instance.StopBgm(); // ֹͣ��������
        gameEndImage.SetActive(true);
    }

    // ���ؽ�������
    public void Hide() {
        gameEndImage.SetActive(false);
    }
}