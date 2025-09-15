using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��ʬ����״̬ö�٣�δ��ʼ�������С����ɽ���
enum SpawnState {
    NotStart,
    Spawning,
    End
}

// ��ʬ������������ʬ�����ɣ���ͨ��ʬ�����Ľ�ʬ��������ͳ�Ƽ���Ϸʤ���ж�
public class ZombieManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ㣨�ⲿ���ƽ�ʬ�����߼���
    public static ZombieManager Instance { get; private set; }

    // ��ǰ��ʬ����״̬��Ĭ��δ��ʼ��
    private SpawnState spawnState = SpawnState.NotStart;

    // ��ʬ���ɵ����飨��Inspector��ֵ����Ӧ��ͬ�е�����λ�ã�
    public Transform[] spawnPointList;
    // ��ͨ��ʬԤ����
    public Zombie zombiePrefab;
    // ���Ľ�ʬԤ���壨ͨ��Ϊ���ղ���ʬ��
    public ZombieFlag zombieFlagPrefab;

    // ��ʬ��Ⱦ�㼶����ֵ�����ƽ�ʬ��ʾ�Ⱥ󣬱����ڵ���
    public static int sortOrder = 100;

    // ��ǰ�����еĽ�ʬ�����������ж��Ƿ����н�ʬ������
    private int zombieCount = 0;

    // ��ʼ������
    private void Awake() {
        Instance = this;
    }

    // ÿ֡���£����ɽ��������н�ʬ������ʱ��������Ϸʤ��
    private void Update() {
        if (spawnState == SpawnState.End && zombieCount == 0) {
            GameManager.Instance.GameEndSuccess();
        }
    }

    // ��ʼ���ɽ�ʬ����GameManager���ã������������̣�
    public void StartSpawn() {
        spawnState = SpawnState.Spawning; // �л���������״̬
        StartCoroutine(SpawnZombie()); // ��������Э��
    }

    // ��ʬ����Э�̣��ֲ����ɣ��������ɼ�������ͣ�
    IEnumerator SpawnZombie() {
        // ��ʼ�ȴ�10�루�����׼��ʱ�䣩
        yield return new WaitForSeconds(10);

        AudioManager.Instance.PlayClip(Config.zombieStartSpawn); // ���Ž�ʬ��ʼ���ɵ���Ч
        UIManager.Instance.flagMeterUI.GameStart(); // �������Ľ���������Ϸ��ʱ��

        // ��һ��������10ֻ��ͨ��ʬ��ÿ2.5��һֻ�������ࣩ
        for (int i = 0; i < 10; i++) {
            SpawnRandomZombie();
            yield return new WaitForSeconds(2.5f);
        }

        // �ڶ���������10ֻ��ͨ��ʬ��ÿ1.5��һֻ������ӿ죩
        for (int i = 0; i < 10; i++) {
            SpawnRandomZombie();
            yield return new WaitForSeconds(1.5f);
        }

        // �ȴ�5�루���ɵ����ղ���
        yield return new WaitForSeconds(5);

        // ���ղ���������1ֻ���Ľ�ʬ��������3��ȫ����ͨ��ʬ��ÿ��5ֻ��
        SpawnFlagZombie();
        for (int i = 0; i < 3; i++) {
            SpawnRowZombie();
            yield return new WaitForSeconds(1); // ÿ�м��1��
        }

        spawnState = SpawnState.End; // ���н�ʬ������ɣ��л������ɽ���״̬
    }

    // �������һֻ��ͨ��ʬ�����ѡ��һ�����ɵ㣩
    private void SpawnRandomZombie() {
        if (spawnState != SpawnState.Spawning) return; // ��������״̬����ִ��

        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // ���ѡһ�����ɵ�
        // ʵ������ͨ��ʬ��ѡ�е����ɵ�
        Zombie zombie = Instantiate(zombiePrefab, spawnPointList[index].position, Quaternion.identity);

        // ������ʬ�������������Ⱦ�㼶�������ɵ��кź�����ֵ������ͬ����ʬ�ڵ���
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingOrder += index * 1000 + sortOrder;
        }
        sortOrder += 100; // ÿ����һֻ��ʬ������ֵ�����������ɵ���ʾ�����棩

        zombieCount++; // ��ʬ����+1
    }

    // ����һֻ���Ľ�ʬ�����ѡ��һ�����ɵ㣩
    private void SpawnFlagZombie() {
        if (spawnState != SpawnState.Spawning) return; // ��������״̬����ִ��

        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // ���ѡһ�����ɵ�
        // ʵ�������Ľ�ʬ��ѡ�е����ɵ�
        Zombie zombie = Instantiate(zombieFlagPrefab, spawnPointList[index].position, Quaternion.identity);

        // �������Ľ�ʬ����Ⱦ�㼶��ͬ��ͨ��ʬ�߼��������ڵ���
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingOrder += index * 1000 + sortOrder;
        }
        sortOrder += 100;

        zombieCount++; // ��ʬ����+1
    }

    // ����һ������ͨ��ʬ�������������ɵ㣬ÿ��5ֻ��
    private void SpawnRowZombie() {
        // �����������ɵ㣬ÿ��������һֻ��ͨ��ʬ
        for (int i = 0; i < 5; i++) {
            Zombie zombie = Instantiate(zombiePrefab, spawnPointList[i].position, Quaternion.identity);

            // ������Ⱦ�㼶�������ɵ��к����֣�����ͬ�н�ʬ�ڵ���
            SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingOrder += i * 1000 + sortOrder;
            }
            sortOrder += 100;

            zombieCount++; // ��ʬ����+1
        }
    }

    // �Ƴ�һֻ��ʬ����ʬ����ʱ���ã�����������
    public void RemoveZombie() {
        zombieCount--;
    }

}