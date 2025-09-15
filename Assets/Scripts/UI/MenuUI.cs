using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �˵�����UI����ͣ�������ȹ��ܣ�
public class MenuUI : MonoBehaviour {
    public GameObject menuImage;  // �˵�����ͼƬ
    public GameObject menuButton; // �˵���ť
    public bool isHide = true;    // �˵��Ƿ����صı��

    // ���ز˵���ť
    public void ButtonHide() {
        menuButton.SetActive(false);
    }

    // ��ʾ�˵���ť
    public void ButtonShow() {
        menuButton.SetActive(true);
    }

    // ��ʾ�˵�����ͣ��Ϸ��
    public void Show() {
        if (menuImage.activeSelf) return; // ����ʾ���˳�

        AudioManager.Instance.PlayClip(Config.buttonClick);
        AudioManager.Instance.StopBgm(); // ֹͣ��������
        GameManager.Instance.PauseGame(); // ��ͣ��Ϸ
        menuImage.SetActive(true);
    }

    // ���ز˵����ָ���Ϸ��
    public void Hide() {
        if (!menuImage.activeSelf) return; // ���������˳�

        AudioManager.Instance.PlayClip(Config.buttonClick);
        AudioManager.Instance.PlayBgm(); // �ָ���������
        GameManager.Instance.ResumeGame(); // �ָ���Ϸ
        menuImage.SetActive(false);
    }
}