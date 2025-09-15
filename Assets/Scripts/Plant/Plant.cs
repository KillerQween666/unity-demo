using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ֲ��״̬�����ã�δ����/�����������ã�����������
enum PlantState {
    Disable,
    Enable
}

public class Plant : MonoBehaviour {
    // ��ǰֲ��״̬��Ĭ�Ͻ���
    PlantState plantState = PlantState.Disable;
    // ֲ�����ͣ�Ĭ�����տ�
    public PlantType plantType = PlantType.sunFlower;

    // ֲ�����������������˸�����⣩
    public Animator animator;
    // �����嶯����������㶹���ֹ���ʱ��ͷ����գ�۶�����
    public Animator grandAnimator;

    // ֲ������ֵ
    public int HP = 100;

    // �������������Ⱦ���б�����������˸Ч����
    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>();

    // �������Ⱥ���˸���ȣ���������Ч����
    private float originalBright = 1f;
    private float flashBright = 2f;

    // ֲ��Ӱ�Ӷ���
    public GameObject shadow;

    // ��ʼ�����ռ�������Ⱦ��������δ����������壩
    private void Start() {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            spriteList.Add(sprite);
        }
    }

    // ÿ֡���£�����״ִ̬�ж�Ӧ�߼�
    private void Update() {
        switch (plantState) {
            case PlantState.Disable:
                DisableUpdate();
                break;
            case PlantState.Enable:
                EnableUpdate();
                break;
        }
    }

    // ����״̬�ĸ����߼���Ĭ�Ͽգ��������д��
    void DisableUpdate() { }

    // ����״̬�ĸ����߼����鷽������������дʵ�־��幦�ܣ�
    protected virtual void EnableUpdate() { }

    // �л�������״̬��ֹͣ�������ر���ײ�塢����Ӱ��
    public void TransitionToDisable() {
        plantState = PlantState.Disable;
        GetComponent<Collider2D>().enabled = false;
        animator.enabled = false;
        grandAnimator.enabled = false;
        shadow.SetActive(false);
    }

    // �л�������״̬����ʼ������������ײ�塢��ʾӰ��
    public void TransitionToEnable() {
        plantState = PlantState.Enable;
        GetComponent<Collider2D>().enabled = true;
        animator.enabled = true;
        grandAnimator.enabled = true;
        shadow.SetActive(true);
    }

    // �ܵ��˺�������Ѫ��������˸������Ѫ��Ϊ0ʱִ������
    public void TakeDamage(int damage) {
        HP -= damage;
        animator.SetTrigger("flashTrigger");

        if (HP <= 0) {
            AudioManager.Instance.PlayClip(Config.eatFinish);
            Dead();
        }
    }

    // ����������˸Ч����������ǿ��
    public void PlayBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", flashBright);
        }
    }

    // ������˸Ч�����ָ��������ȣ�
    public void StopBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", originalBright);
        }
    }

    // �����߼���ֱ������ֲ�����
    private void Dead() {
        Destroy(gameObject);
    }

    public virtual void PlantFun() { }
}