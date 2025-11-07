using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class ZombieBobsled : Zombie {

    private bool isSit = false;
    public float carTarget = 0;

    public SpriteRenderer carSortSprite;

    public List<SpriteRenderer> carRenderers = new List<SpriteRenderer>();

    public SpriteRenderer carInside;
    public SpriteRenderer car;
    public SpriteRenderer car2;
    public SpriteRenderer car3;
    public SpriteRenderer car4;

    private bool isCar2;
    private bool isCar3;
    private bool isCar4;
    public bool isCarDead;

    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public bool isHaveCar = false;

    public Transform[] spawnPointList;

    public ZombieBobsled zombiePrefab;

    private List<ZombieBobsled> zombieList = new List<ZombieBobsled>();

    private new void Awake() {
        base.Awake();

        if (shadow != null) shadow.SetActive(false);

        if (isHaveCar == true) {
            car2.enabled = false;
            car3.enabled = false;
            car4.enabled = false;
            SpawnZombie();
        } 
            
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (isSit) {
            if (transform.position.x <= carTarget) {
                SitStop();
            }
        }
    }

    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 70) HideHand();
        if (HP <= 20) HideHead();
    }

    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayBobsledHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayBobsledHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    public void Sit() {
        isSit = true;
    }

    public void SitStop() {
        isSit = false;
        CarDead();

        animator.SetTrigger("walkTrigger");
    }

    public void PushCarStop() {
        animator.SetTrigger("jumpTrigger");
    }

    public void SitCarSort() {
        if (isHaveCar && enemy != null) {
            car.sortingOrder = carSortSprite.sortingOrder;
            car2.sortingOrder = carSortSprite.sortingOrder;
            car3.sortingOrder = carSortSprite.sortingOrder;
            car4.sortingOrder = carSortSprite.sortingOrder;
        }
    }

    public void SpawnZombie() {
        if (isHaveCar) {
            for (int i = 0; i < spawnPointList.Length; i++) {
                ZombieBobsled zombie = Instantiate(zombiePrefab, spawnPointList[i].position, Quaternion.identity);
                zombieList.Add(zombie);
            }
        }
    }

    public virtual IEnumerator PlayCarFlash() {

        carRenderers.ForEach(s => s.material.SetFloat("_Brightness", flashBright));
        yield return new WaitForSeconds(0.2f);
        carRenderers.ForEach(s => s.material.SetFloat("_Brightness", originalBright));
    }

    public void ShowCar2() {
        if (isCar2) return; // 已处于该状态则直接返回
        isCar2 = true;

        car.enabled = false;
        car2.enabled = true; // 显示轻微破损的桶
    }

    public void ShowCar3() {
        if (isCar3) return; // 已处于该状态则直接返回
        isCar3 = true;

        car2.enabled = false;
        car3.enabled = true; // 显示轻微破损的桶
    }

    public void ShowCar4() {
        if (isCar4) return; // 已处于该状态则直接返回
        isCar4 = true;

        car3.enabled = false;
        car4.enabled = true; // 显示轻微破损的桶
    }

    public void CarDead() {
        if (isCarDead) return; // 已处于该状态则直接返回
        isCarDead = true;

        if (enemy != null) {
            Destroy(enemy.gameObject);
        }

        if (isHaveCar == false) return;

        Walk();
        foreach (var zombie in zombieList) {
            zombie.Walk();
        }

       
    }
    
    private void Walk() {
         animator.SetTrigger("walkTrigger");
        if (shadow != null) shadow.SetActive(true);
    }

    public override void Dead() {
        if (isHaveCar) {
            base.Dead();
        } else {
            Destroy(this.gameObject);
        }
    }
}
