using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ʤ������UI
public class WinUI : MonoBehaviour {

    private Animator animator; // ����ʤ������

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // ��ʼ����
    }

    // ����ʤ������
    public void Hide() {
        animator.enabled = false;
    }

    // ��ʾʤ������
    public void Show() {
        animator.enabled = true;
    }

    // ��ʾ��Ϸ�����ܽ��棨�����¼����ã�
    public void ShowGameEndUI() {
        GameManager.Instance.PauseGame();
        UIManager.Instance.gameEndUI.Show();
    }
}