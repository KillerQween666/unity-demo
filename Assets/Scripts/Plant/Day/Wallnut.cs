using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 坚果墙类（继承自植物基类，实现防御功能及损伤状态变化）
public class Wallnut : Plant {
    public SpriteRenderer face;    // 坚果墙完好状态的脸
    public SpriteRenderer face2;   // 坚果墙中度损伤的脸
    public SpriteRenderer face3;   // 坚果墙重度损伤的脸

    bool isShowFace2 = false;  // 标记是否已显示中度损伤的脸
    bool isShowFace3 = false;  // 标记是否已显示重度损伤的脸
    bool isHurtLarge = false;  // 标记是否触发了重度损伤特效（避免重复播放）

    private void Awake() {
        // 初始状态：只显示完好的脸，隐藏损伤状态的脸
        face2.enabled = false;
        face3.enabled = false;
    }

    // 重写受击处理（增加损伤状态切换和特效播放）
    public override void TakeDamage(int damage) {
        base.TakeDamage(damage); // 执行父类受击逻辑（扣血、闪烁）
        animator.SetTrigger("protectedTrigger"); // 触发防御受击动画

        isHurtLarge = false; // 重置重度损伤标记（每次受击默认轻伤）

        // 根据生命值切换损伤状态
        if (HP <= 600) {
            ShowFace2(); // 切换到中度损伤
        }
        if (HP <= 300) {
            ShowFace3(); // 切换到重度损伤
        }

        // 未触发重度损伤且未死亡时，播放轻度损伤特效
        if (isHurtLarge == false && HP > 0) {
            ObjectPoolManager.Instance.PlayWallnutHurtSmallParticalIEnumrator(transform);
        }
    }

    // 切换到中度损伤状态
    void ShowFace2() {
        if (isShowFace2) return; // 已处于该状态则直接返回
        isShowFace2 = true;

        isHurtLarge = true; // 标记为重度损伤（播放对应特效）
        ObjectPoolManager.Instance.PlayWallnutHurtLargeParticalIEnumrator(transform); // 播放重度损伤特效
        face.enabled = false;
        face2.enabled = true; // 显示中度损伤的脸
    }

    // 切换到重度损伤状态
    void ShowFace3() {
        if (isShowFace3) return; // 已处于该状态则直接返回
        isShowFace3 = true;

        isHurtLarge = true; // 标记为重度损伤（播放对应特效）
        ObjectPoolManager.Instance.PlayWallnutHurtLargeParticalIEnumrator(transform); // 播放重度损伤特效
        face2.enabled = false;
        face3.enabled = true; // 显示重度损伤的脸
    }

    // 重写死亡逻辑（增加死亡特效）
    public override void Dead() {
        isHurtLarge = true; // 标记为重度损伤（播放死亡时的重度特效）
        ObjectPoolManager.Instance.PlayWallnutHurtLargeParticalIEnumrator(transform); // 播放死亡损伤特效
        base.Dead(); // 执行父类死亡逻辑（销毁对象）
    }
}