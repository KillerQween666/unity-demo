using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieCommonSwim : Zombie {
    private float waterXPosition = 7.2f;

    private bool isJumpToPool = false;

    public bool isSpawnOnWaterCell = false;

    public List<SpriteRenderer> hideRenderers = new List<SpriteRenderer>(); // 需隐藏的身体部位渲染器（如变种僵尸的附加部件）

    // 身体部位隐藏状态标记（避免重复执行隐藏逻辑）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public List<SpriteRenderer> hideSprite = new List<SpriteRenderer>(); // 需隐藏的渲染器

    public Transform fallWaterTransform;

    public Transform[] seaWeedTransform;
    public GameObject[] seaWeeds;
    public Vector2 spawnAreaSize = new Vector2(10f, 10f); // 2D生成区域大小（X=宽度，Y=高度）

    private static System.Random random = new System.Random();

    protected new void Awake() {
        base.Awake(); // 调用父类Zombie的初始化方法
        // 隐藏非普通僵尸的附加部件（铁桶、铁门、旗帜等）
        foreach (var render in hideRenderers) render.enabled = false;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (isJumpToPool == false) {
            if (transform.position.x <= waterXPosition) {
                isJumpToPool = true;
                animator.SetTrigger("jumpToPoolTrigger");
            }
        }
    }

    public void MoveToPoolY() {
        if (isSpawnOnWaterCell == true) return;

        Vector3 position = transform.position;
        position.y -= 0.8f;

        transform.DOMoveY(position.y, 0.16f);
    }

    public void HideShadow() {
        hideSprite.ForEach(r => r.enabled = false);
        if (shadow != null) shadow.SetActive(false);
    }

    // 重写受伤逻辑（增加身体部位隐藏判断）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应阈值时隐藏手部/头部
        if (HP <= 70) HideHand();
        if (HP <= 20) HideHead();
    }

    // 头部隐藏处理（含特效播放）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器
        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 手部隐藏处理（含特效播放）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器
        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    public void PlayEnterWaterSource() {
        HideShadow();
        ObjectPoolManager.Instance.PlayFallWaterSwfIEnumrator(fallWaterTransform);
        AudioManager.Instance.PlayClip(Config.enterWater);
    }

    public void SpawnSeaWeed() {
        GameObject[] gameObjects = RandomPickCells(3, seaWeeds);

        // 循环生成物体
        for (int i = 0; i < seaWeedTransform.Length; i++) {
            // 1. 随机选择一个预制体（4选1）
            Random2DPosition(i, gameObjects[i]);
           
        }
    }

    private GameObject[] RandomPickCells(int count, GameObject[] gameObjects) {
        return gameObjects.OrderBy(c => random.Next()).Take(count).ToArray();
    }

    private void Random2DPosition(int i, GameObject gameObject) {
        // 生成器的位置（区域中心）
        Vector3 spawnerPos = seaWeedTransform[i].position;

        // 在X轴范围：[中心X - 区域宽度/2, 中心X + 区域宽度/2]
        float randomX = Random.Range(spawnerPos.x - spawnAreaSize.x / 2f, spawnerPos.x + spawnAreaSize.x / 2f);

        // 在Y轴范围：[中心Y - 区域高度/2, 中心Y + 区域高度/2]
        float randomY = Random.Range(spawnerPos.y - spawnAreaSize.y / 2f, spawnerPos.y + spawnAreaSize.y / 2f);

        // Z轴固定为0（2D场景无需调整）
        GameObject selectedPrefab = seaWeeds[Random.Range(0, seaWeeds.Length)];
        GameObject obj = Instantiate(gameObject, spawnerPos, Quaternion.Euler(0, 0, 0));

        // 3. 实例化物体（Z轴旋转为0，适配2D）
        obj.transform.SetParent(seaWeedTransform[i]);
    }

    protected override void PlayBoomSwf(Transform transform, bool isHaveHead, int sort) {
        
    }
}
