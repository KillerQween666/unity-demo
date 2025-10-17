using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieFootball : Zombie {

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public SpriteRenderer goodHelmet; // 完好的头盔
    public SpriteRenderer badHelmet; // 轻微破损的头盔
    public SpriteRenderer worstHelmet; // 严重破损的头盔

    public Transform helmetEmissionTransform; // 头盔掉落时的特效生成位置

    // 桶的状态标记（避免重复切换状态）
    bool isHelmetBad = false; // 头盔是否轻微破损
    bool isHelmetWorst = false; // 头盔是否严重破损
    bool isHelmetDead = false; // 头盔是否完全损坏

    private new void Awake() {
        base.Awake(); // 调用父类的初始化
        // 初始只只显示完好的桶
        badHelmet.enabled = false;
        worstHelmet.enabled = false;
    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 根据生命值切换桶的状态
        if (HP < 650) {
            HelmetBad(); // 切换到轻微破损
        }

        if (HP < 375) {
            HelmetWorst(); // 切换到严重破损
        }

        if (HP < 100) {
            HelmetDead(); // 桶完全损坏
        }

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 70) HideHand();

        if (HP <= 25) HideHead();
    }

    // 桶轻微破损处理
    private void HelmetBad() {
        if (isHelmetBad) return; // 已处于该状态则直接返回
        isHelmetBad = true;

        goodHelmet.enabled = false;
        badHelmet.enabled = true; // 显示轻微破损的桶
    }

    // 桶严重破损处理
    private void HelmetWorst() {
        if (isHelmetWorst) return; // 已处于该状态则直接返回
        isHelmetWorst = true;

        badHelmet.enabled = false;
        worstHelmet.enabled = true; // 显示严重破损的桶
    }

    // 桶完全损坏处理
    private void HelmetDead() {
        if (isHelmetDead) return; // 已处于该状态则直接返回
        isHelmetDead = true;

        worstHelmet.enabled = false; // 隐藏桶

        // 播放桶破碎的特效
        ObjectPoolManager.Instance.PlayFootballHelmetEmissionIEnumrator(helmetEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayFootballHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayFootballHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

}
