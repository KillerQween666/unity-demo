using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����ĵĽ�ʬ���̳�����ͨ��ʬ��
public class ZombieFlag : Zombie {
    // ���������Ⱦ��
    public List<SpriteRenderer> hideSprite = new List<SpriteRenderer>();
    public List<SpriteRenderer> showSprite = new List<SpriteRenderer>();
    public SpriteRenderer goodFlag; // �������
    public SpriteRenderer badFlag; // ��������
    public GameObject flagParticalPrefab; // ������Ч

    // ����״̬���
    bool isFlagBad = false;
    bool isFlagDead = false;

    // ��ʼ������д���ࣩ
    private new void Awake() {
        base.Awake(); // ���û����ʼ��
        // ��ʼ��������Ⱦ��״̬(���ض�����ͬʱ�����Ľ�ʬӵ�еĲ���չ��)
        hideSprite.ForEach(r => r.enabled = false);
        showSprite.ForEach(r => r.enabled = true);
    }

    // ��д�����߼�����������״̬����
    public override void TakeDamage(float damage) {
        HP -= damage;
        // ������˸Ч��
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // ����״̬�仯
        if (HP <= 110) {
            HideHand();
            FlagBad(); // ��������
        }
        if (HP <= 20) {
            HideHead();
            FlagDead(); // ������ʧ
        }
        // �����ж�
        if (HP <= 0) ToDead();
    }

    // ��������
    private void FlagBad() {
        if (isFlagBad) return;
        isFlagBad = true;
        goodFlag.enabled = false;
        badFlag.enabled = true;
    }

    // ������ʧ
    private void FlagDead() {
        if (isFlagDead) return;
        isFlagDead = true;
        badFlag.enabled = false;
        // ����������Ч
        flagParticalPrefab.transform.parent = null;
        flagParticalPrefab.GetComponent<ParticleSystem>().Play();
        // �л���Ⱦ����ʾ״̬
        hideSprite.ForEach(r => r.enabled = true);
        showSprite.ForEach(r => r.enabled = false);
    }
}