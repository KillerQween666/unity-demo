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
    public List<SpriteRenderer> hideHeadSprite = new List<SpriteRenderer>(); // 需隐藏的渲染器
    public List<SpriteRenderer> hideHandSprite = new List<SpriteRenderer>();
    public List<SpriteRenderer> showSprite = new List<SpriteRenderer>(); // 需显示的渲染器

    public Transform paperEmissionTransform; // 报纸掉落时的特效生成位置

    public List<SpriteRenderer> paperRenderers = new List<SpriteRenderer>();

    // 报纸的状态标记（避免重复切换状态）
    bool isPaperBad = false; // 报纸是否轻微破损
    bool isPaperWorst = false; // 报纸是否严重破损
    bool isPaperDead = false; // 保值是否完全损坏

    bool isCraze = false;

    private new void Awake() {
        base.Awake(); // 调用父类的初始化
        // 初始只只显示完好的桶
        badPaper.enabled = false;
        worstPaper.enabled = false;

        hideHeadSprite.ForEach(r => r.enabled = false);
        hideHandSprite.ForEach(r => r.enabled = false);
    }

    // 重写受伤逻辑（按生命值阈值隐藏身体部位）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应值时隐藏手部/头部
        if (HP <= 400) HideHand();

        if (HP <= 50) HideHead();
    }

    // 桶轻微破损处理
    public void PaperBad() {
        if (isPaperBad) return; // 已处于该状态则直接返回
        isPaperBad = true;

        goodPaper.enabled = false;
        badPaper.enabled = true; // 显示轻微破损的桶
    }

    // 桶严重破损处理
    public void PaperWorst() {
        if (isPaperWorst) return; // 已处于该状态则直接返回
        isPaperWorst = true;

        badPaper.enabled = false;
        worstPaper.enabled = true; // 显示严重破损的桶
    }

    // 桶完全损坏处理
    public void PaperDead() {
        if (isPaperDead) return; // 已处于该状态则直接返回
        isPaperDead = true;

        worstPaper.enabled = false; // 隐藏桶
        if (isHideHand == false) hideHandSprite.ForEach(r => r.enabled = true);

        // 播放桶破碎的特效
        animator.SetTrigger("losePaperTrigger");  
    }

    public void PlayPaperEmission() {
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

        animator.SetBool("isCraze", true);
        EnterCrazy();

        hideHandSprite.ForEach(r => r.enabled = true);
        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器

        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayPaperHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    public void EnterCrazy() {
        if (isCraze) return;
        isCraze = true;

        showSprite.ForEach(r => r.enabled = false);
        if (isHideHead == false) hideHeadSprite.ForEach(r => r.enabled = true);
        if (isHideHand == false) hideHandSprite.ForEach(r => r.enabled = true);

        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.paperCry : Config.paperCry2);

        originSpeed *= 3f;
        atkValue = 50;
        SetAnimatorSpeed(originSpeed);
    }

    public virtual IEnumerator PlayPaperFlash() {
        paperRenderers.ForEach(s => s.material.SetFloat("_Brightness", flashBright));
        yield return new WaitForSeconds(0.2f);
        paperRenderers.ForEach(s => s.material.SetFloat("_Brightness", originalBright));
    }
}
