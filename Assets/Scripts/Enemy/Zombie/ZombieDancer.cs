using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static UnityEngine.RuleTile.TilingRuleOutput;

// 撑杆僵尸类（继承自基础僵尸类，扩展撑杆跳和身体部位逻辑）
public class ZombieDancer : Zombie {

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public UnityEngine.Transform parent;

    private bool isReverse;

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 70) HideHand();
        if (HP <= 25) HideHead();
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayDancerHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayDancerHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    public void ExitParent() {
        shadow.transform.SetParent(null);
    }

    public void EnterParent() {
        shadow.transform.SetParent(parent);
    }

    public void Reverse() {
        if (isReverse) {
            isReverse = false;
        }
        else {
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

        Destroy(shadow.gameObject);
    }
}