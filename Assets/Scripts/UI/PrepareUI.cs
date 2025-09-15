using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ׼������UI����Ϸ��ʼǰ�ĵ���ʱ�ȣ�
public class PrepareUI : MonoBehaviour {

    private Animator animator; // ����׼������

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // ��ʼ����
    }

    // ����׼������
    public void Hide() {
        animator.enabled = false;
    }

    // ��ʾ׼������
    public void Show() {
        animator.enabled = true;
    }

    // ׼����ɣ�֪ͨ��Ϸ��������ʼ��Ϸ�������¼����ã�
    public void GameStart() {
        GameManager.Instance.OnPrepareUIComplete();
    }
}