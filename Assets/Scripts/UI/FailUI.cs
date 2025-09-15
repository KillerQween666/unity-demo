using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ʧ�ܽ���UI
public class FailUI : MonoBehaviour {

    private Animator animator; // ����ʧ�ܶ���

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // ��ʼ����
    }

    // ����ʧ�ܽ���
    public void Hide() {
        animator.enabled = false;
    }

    // ��ʾʧ�ܽ���
    public void Show() {
        animator.enabled = true;
    }

    // ��ʾ��Ϸ�����ܽ��棨�����¼����ã�
    public void ShowGameEndUI() {
        GameManager.Instance.PauseGame();
        UIManager.Instance.gameEndUI.Show();
    }
}