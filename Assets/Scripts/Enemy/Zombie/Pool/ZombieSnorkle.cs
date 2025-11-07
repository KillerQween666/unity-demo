using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSnorkle : Zombie {
    private float waterXPosition = 7.2f;

    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    private bool isJumpToPool = false;

    public Transform fallWaterTransform;

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (isJumpToPool == false) {
            if (transform.position.x <= waterXPosition) {
                isJumpToPool = true;
                animator.SetTrigger("jumpToPoolTrigger");
            }
        }

    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 140) HideHand();
        if (HP <= 40) HideHead();
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlaySnorkleHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlaySnorkleHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    public void HideShadow() {
        if (shadow != null) shadow.SetActive(false);
    }

    public void PlayEnterWaterSource() {
        ObjectPoolManager.Instance.PlayFallWaterSwfIEnumrator(fallWaterTransform);
        AudioManager.Instance.PlayClip(Config.enterWater);
    }

    protected override void PlayBoomSwf(Transform transform, bool isHaveHead, int sort) {

    }
}
