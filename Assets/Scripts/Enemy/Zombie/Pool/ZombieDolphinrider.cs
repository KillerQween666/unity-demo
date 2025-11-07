using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDolphinrider : Zombie {
    private float waterXPosition = 7.2f;

    private bool isJumpToPool = false;

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏
    private bool isUsePole = false; // 是否已使用撑杆

    private bool isCanOverPlant = true;

    private bool isRide = false;
    public float rideMoveSpeed = 3;

    public Transform fallWaterTransform;

    protected override void FixedUpdate() {
        base.FixedUpdate();

        if (isJumpToPool == false) {
            if (transform.position.x <= waterXPosition) {
                isJumpToPool = true;
                animator.SetTrigger("jumpToPoolTrigger");
            }
        }

        if (isRide) {
            transform.Translate(Vector3.left * Time.deltaTime * rideMoveSpeed);
        }

    }

    public void HideShadow() {
        if (shadow != null) shadow.SetActive(false);
    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 125) HideHand();
        if (HP <= 25) HideHead();
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayDolphinriderHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayDolphinriderHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 重写碰撞进入逻辑（增加撑杆跳触发）
    protected override void OnTriggerEnter2D(Collider2D collision) {
        // 未使用过撑杆且碰撞到植物时，触发撑杆跳动画
        if (isUsePole == false) {
            if (isHypno) {
                if (collision.CompareTag("Zombie")) {
                    isUsePole = true; // 标记撑杆已使用
                    animator.SetTrigger("jumpOverPlantTrigger"); // 触发跳转动
                }
            }
            else {
                if (collision.CompareTag("House")) {
                    GameManager.Instance.GameEndFail(); // 攻击房子，游戏失败

                }

                if (collision.CompareTag("Plant") && collision.GetComponent<Plant>().isCanEat == true) {
                    isUsePole = true; // 标记撑杆已使用
                    animator.SetTrigger("jumpOverPlantTrigger"); // 触发跳转动画 

                    if (collision.TryGetComponent<Tallnut>(out var tallnut)) {
                        isCanOverPlant = false;
                        base.OnTriggerEnter2D(collision);
                    }
                }
            }
        }
        else {
            base.OnTriggerEnter2D(collision); // 其他情况执行父类碰撞逻辑（如攻击植物/房子）
        }

    }

    // 播放跳跃音效（动画事件调用）
    public void PlayJumpSource() {
        if (isCanOverPlant == false) {
            AudioManager.Instance.PlayClip(Config.bonk);
            animator.SetTrigger("walkTrigger");
            Vector3 position = transform.position;
            position.x += 2f;
            transform.DOMoveX(position.x, 0);
        }
        else {
            AudioManager.Instance.PlayClip(Config.dolphinJumping);
        }
    }

    public void PlayAppearSource() {
        AudioManager.Instance.PlayClip(Config.dolphinAppear);
    }

    public void PlayEnterWaterSource() {
        ObjectPoolManager.Instance.PlayFallWaterSwfIEnumrator(fallWaterTransform);
        AudioManager.Instance.PlayClip(Config.enterWater);
    }

    protected override void PlayBoomSwf(Transform transform, bool isHaveHead, int sort) {

    }

    public void StartRide() {
        isRide = true;
    }

    public void EndRide() {
        isRide = false;
    }
}
