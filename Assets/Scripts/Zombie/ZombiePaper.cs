using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePaper : Zombie {

    // 身体部位及状态标记（控制隐藏逻辑和撑杆使用状态）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    public SpriteRenderer goodPaper; // 完好的报纸
    public SpriteRenderer badPaper; // 轻微破损的报纸
    public SpriteRenderer worstPaper; // 严重破损的报纸

    // 变种僵尸的显示切换渲染器列表
    public List<SpriteRenderer> hideSprite = new List<SpriteRenderer>(); // 需隐藏的渲染器
    public List<SpriteRenderer> showSprite = new List<SpriteRenderer>(); // 需显示的渲染器

    public Transform paperEmissionTransform; // 报纸掉落时的特效生成位置

    // 报纸的状态标记（避免重复切换状态）
    bool isPaperBad = false; // 报纸是否轻微破损
    bool isPaperWorst = false; // 报纸是否严重破损
    bool isPaperDead = false; // 保值是否完全损坏

    private new void Awake() {
        base.Awake(); // 调用父类的初始化
        // 初始只只显示完好的桶
        badPaper.enabled = false;
        worstPaper.enabled = false;
    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 根据生命值切换报纸的状态
        if (HP < 725) {
            PaperBad(); // 切换到轻微破损
        }

        if (HP < 650) {
            PaperWorst(); // 切换到严重破损
        }

        if (HP < 475) {
            PaperDead(); // 报纸完全损坏
        }

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 250) HideHand();

        if (HP <= 50) HideHead();
    }

    // 桶轻微破损处理
    private void PaperBad() {
        if (isPaperBad) return; // 已处于该状态则直接返回
        isPaperBad = true;

        goodPaper.enabled = false;
        badPaper.enabled = true; // 显示轻微破损的桶
    }

    // 桶严重破损处理
    private void PaperWorst() {
        if (isPaperWorst) return; // 已处于该状态则直接返回
        isPaperWorst = true;

        badPaper.enabled = false;
        worstPaper.enabled = true; // 显示严重破损的桶
    }

    // 桶完全损坏处理
    private void PaperDead() {
        if (isPaperDead) return; // 已处于该状态则直接返回
        isPaperDead = true;

        worstPaper.enabled = false; // 隐藏桶

        // 播放桶破碎的特效
        animator.SetTrigger("losePaperTrigger");
        ObjectPoolManager.Instance.PlayPaperEmissionIEnumrator(paperEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 隐藏头部处理（含状态更新和特效）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器

        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayPaperHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 隐藏手部处理（含特效）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayPaperHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    public void EnterCrazy() {
        showSprite.ForEach(r => r.enabled = false);
        hideSprite.ForEach(r => r.enabled = true);

        originSpeed *= 2.5f;
        atkValue = 50;
        SetAnimatorSpeed(originSpeed);
    }

}
