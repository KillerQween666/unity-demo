using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 撑杆僵尸类（继承自基础僵尸类，扩展撑杆跳和身体部位逻辑）
public class ZombiePole : Zombie {

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏
    private bool isUsePole = false; // 是否已使用撑杆

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeDamage(float damage) {
        base.TakeDamage(damage); // 执行父类受伤处理（扣血、闪烁等）
        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 125) HideHand();
        if (HP <= 25) HideHead();
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        animator.SetBool("isHurt", true); // 切换到受伤动画状态
        isUsePole = true; // 标记撑杆已使用

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayPoleHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayPoleHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 重写碰撞进入逻辑（增加撑杆跳触发）
    protected override void OnTriggerEnter2D(Collider2D collision) {
        // 未使用过撑杆且碰撞到植物时，触发撑杆跳动画
        if (collision.CompareTag("Plant") && isUsePole == false) {
            isUsePole = true; // 标记撑杆已使用
            animator.SetTrigger("jumpTrigger"); // 触发跳转动画
            return;
        }
        else {
            base.OnTriggerEnter2D(collision); // 其他情况执行父类碰撞逻辑（如攻击植物/房子）
        }

    }

    // 播放跳跃音效（动画事件调用）
    public void PlayJumpSource() {
        AudioManager.Instance.PlayClip(Config.jump);
    }

}