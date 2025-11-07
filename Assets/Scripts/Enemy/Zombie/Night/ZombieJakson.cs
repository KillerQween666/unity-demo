using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Tilemaps.TilemapRenderer;

// 撑杆僵尸类（继承自基础僵尸类，扩展撑杆跳和身体部位逻辑）
public class ZombieJakson : Zombie {

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public Light2DUse light2D;

    private Transform parent;

    public float spawnZombieTime = 10f;
    private float spawnZombieTimer;

    private bool isSpawned = false;
    public float startSpawnTime = 5f;
    private float startSpawnTimer;

    public Transform[] spawnPointList;
    public Zombie zombieDancer;

    private bool isReverse;

    private bool isPlayLight = false;

    private void Start() {
        parent = shadow.transform.parent;

        Vector3 position = transform.position;
        position.y -= 0.35f;
        transform.position = position;

        light2D.StopLight();
    }

    private void Update() {
        spawnZombieTimer += Time.deltaTime;
        if (spawnZombieTimer > spawnZombieTime) {
            animator.SetBool("isSpawnZombie", true);
            spawnZombieTimer = 0;
        }

        if (isSpawned == false) {
            startSpawnTimer += Time.deltaTime;
            if (startSpawnTimer > startSpawnTime) {
                isSpawned = true;

                animator.SetBool("isSpawnZombie", true);
            }
        }

    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 140) HideHand();
        if (HP <= 25) HideHead();
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayJaksonHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayJaksonHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    public void ExitParent() {
        light2D.transform.SetParent(null);
        shadow.transform.SetParent(null);
    }

    public void EnterParent() {
        shadow.transform.SetParent(parent);
        light2D.transform.SetParent(parent);
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Plant") && isSpawned == false && !collision.TryGetComponent<HypnoShroom>(out var hypnoShroom)) {
            animator.SetBool("isSpawnZombie", true);
            isSpawned = true;
        }
        base.OnTriggerEnter2D(collision); // 执行父类碰撞逻辑（如攻击植物/房子）

    }

    public void SpawnDancerZombie() {
        animator.SetBool("isSpawnZombie", true);
        isSpawned = true;
        StartCoroutine(SpawnZombie());
    }

    private IEnumerator SpawnZombie() {
        CheckReverse();
        AudioManager.Instance.PlayClip(Config.spawnGrave);

        for (int i = 0; i < 4; i++) {
            if ((row == 0 && i == 0) || (row == ZombieManager.Instance.spawnPointList.Length && i == 3)) {
                StartCoroutine(SpawnOnCellZombie(spawnPointList[4], i));
            } else {
                StartCoroutine(SpawnOnCellZombie(spawnPointList[i], i));
            }
        }

        yield return new WaitForSeconds(1);

        animator.SetBool("isSpawnZombie", false);
        spawnZombieTimer = 0;
    }

    IEnumerator SpawnOnCellZombie(Transform transform, int i) {

        Vector3 dirtPosition = transform.position;
        dirtPosition.y += 1f;

        ObjectPoolManager.Instance.PlayDirtSmallParticalIEnumrator(dirtPosition);
        ObjectPoolManager.Instance.PlayDirtBigParticalIEnumrator(dirtPosition);

        yield return new WaitForSeconds(1f);

        Zombie zombie;

        Vector3 position = transform.position;
        position.y -= 1f;

        zombie = Instantiate(zombieDancer, position, Quaternion.identity);
        zombie.transform.DOMoveY(transform.position.y, 0.2f);

        ZombieManager.Instance.AddZombie();

        if (isHypno) zombie.Hypnoed();

        // 调整渲染层级（按生成点行号区分，避免同行僵尸遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            int order = this.spriteList[0].sortingOrder;
            int row = this.row;
            if (i == 0 && row != 0) {
                sprite.sortingLayerName =  ZombieManager.Instance.layerNames[row - 1];
            }
            else if (i == 3 && row != ZombieManager.Instance.spawnPointList.Length) {
                sprite.sortingLayerName = ZombieManager.Instance.layerNames[row + 1];
            }
            else {
                sprite.sortingLayerName = ZombieManager.Instance.layerNames[row];

                if (i == 1) order += 100;
                else if (i == 2) order -= 200;
                else order -= 100;
            }
            sprite.sortingOrder += order;
        }
    }

    public void Reverse() {
        if (isReverse) {
            isReverse = false;
        } else {
            isReverse = true;
        }
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void CheckReverse() {
        if (isReverse == true) {
            Reverse();
        }
    }

    public override void Dead() {
        base.Dead();
        light2D.StopLight();

        Destroy(light2D);
        Destroy(shadow);
        AudioManager.Instance.danceZombie--;
        AudioManager.Instance.StopDanceBgm();
    }

    public void PlayLight() {
        if (isPlayLight) return;
        isPlayLight = true;

        AudioManager.Instance.danceZombie++;
        AudioManager.Instance.PlayDanceBgm();
        light2D.PlayLight();
    }

}