using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;

public class Zombie : MonoBehaviour {

    public Animator animator; // �������

    // ���岿λ��Ⱦ���б�
    public List<SpriteRenderer> headRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> handRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> hideRenderers = new List<SpriteRenderer>();

    public float HP = 100; // ����ֵ
    public float minSpeed = 0.8f; // ��С�ƶ��ٶ�
    public float maxSpeed = 1.2f; // ����ƶ��ٶ�
    public int atkValue = 10; // ������

    private Plant currentEatPlant; // ��ǰ������ֲ��

    // ���岿λ���ر��
    private bool isHideHead = false;
    private bool isHideHand = false;

    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>(); // ������Ⱦ��

    // ��˸Ч������
    private float originalBright = 1f;
    private float flashBright = 2f;
    public Coroutine flashCoroutine;

    // ��Чλ��
    public Transform headEmissionTransform;
    public Transform handEmissionTransform;
    public GameObject shadow; // Ӱ��

    // ��ʼ�������״̬
    public void Awake() {
        animator = GetComponent<Animator>();

        // �����������ͨ��ʬ����Ĳ������أ���Ͱ�����ţ����ĵȣ�
        foreach (var render in hideRenderers) render.enabled = false;

        // �ռ�������Ⱦ��
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) spriteList.Add(sprite);

        RandomWalk(); // �����ʼ�����߶���
    }

    // ���ö����ٶ�
    public void SetAnimatorSpeed(float speed) {
        animator.speed = speed;
    }

    // ��ʳ״̬���£������߼���
    public void EatPlant() {
        currentEatPlant?.TakeDamage(atkValue); // ����ֲ��
        // ������Ž�ʳ��Ч
        var clips = new[] { Config.eatPlant, Config.eatPlant2, Config.eatPlant3 };
        AudioManager.Instance.PlayClip(clips[Random.Range(0, 3)]);
    }

    // �������루��ײ��⣩
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            currentEatPlant = collision.GetComponent<Plant>();
            animator.SetBool("isEat", true);
        }
        else if (collision.CompareTag("House")) {
            animator.SetBool("isEat", true);
            GameManager.Instance.GameEndFail(); // ������Ϸʧ��
        }
    }

    // �����˳�����ײ��⣩
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            animator.SetBool("isEat", false);
            currentEatPlant = null;
        }
    }

    // ������߶������ٶ�
    void RandomWalk() {
        int randomIndex = Random.Range(0, 2);
        animator.SetInteger("walkIndex", randomIndex);
        SetAnimatorSpeed(Random.Range(minSpeed, maxSpeed));
    }

    // �����������
    void RandomDead() {
        int randomIndex = Random.Range(0, 7);
        animator.SetInteger("deadIndex", randomIndex < 3 ? 0 : (randomIndex < 6 ? 1 : 2));
    }

    // ����ͷ��
    public void HideHead() {
        if (isHideHead) return;
        isHideHead = true;

        headRenderers.ForEach(r => r.enabled = false);
        // ����ͷ��������Ч
        ObjectPoolManager.Instance.PlayHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100);
    }

    // �����ֲ�
    public void HideHand() {
        if (isHideHand) return;
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false);
        // �����ֲ�������Ч
        ObjectPoolManager.Instance.PlayHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100);
    }

    // �ܵ��˺����鷽��������������д��
    public virtual void TakeDamage(float damage) {
        HP -= damage;
        // ������˸Ч��
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // ������ֵ�������岿λ
        if (HP <= 70) HideHand();
        if (HP <= 20) HideHead();
        // �����ж�
        if (HP <= 0) ToDead();
    }

    // ��������
    public void ToDead() {
        SetAnimatorSpeed(1);
        RandomDead();
        shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetBool("isDead", true);
    }

    // ��ѹ�������类С�Ƴ�ѹ��
    public void Push() {
        SetAnimatorSpeed(1);
        shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetBool("isPush", true);
    }

    // ��ȫ�������Ƴ�����
    public void Dead() {
        Destroy(gameObject);
        ZombieManager.Instance.RemoveZombie();
    }

    // ������˸Ч��
    public virtual IEnumerator PlayFlash() {
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", flashBright));
        yield return new WaitForSeconds(0.2f);
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", originalBright));
    }
}