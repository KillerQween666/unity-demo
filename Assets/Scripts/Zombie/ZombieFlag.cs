using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 带旗帜的僵尸（继承自普通僵尸）
public class ZombieFlag : Zombie {
    // 旗帜相关渲染器
    public List<SpriteRenderer> hideSprite = new List<SpriteRenderer>();
    public List<SpriteRenderer> showSprite = new List<SpriteRenderer>();
    public SpriteRenderer goodFlag; // 完好旗帜
    public SpriteRenderer badFlag; // 破损旗帜
    public GameObject flagParticalPrefab; // 旗帜特效

    // 旗帜状态标记
    bool isFlagBad = false;
    bool isFlagDead = false;

    // 初始化（重写基类）
    private new void Awake() {
        base.Awake(); // 调用基类初始化
        // 初始化旗帜渲染器状态(隐藏动画的同时将旗帜僵尸拥有的部分展现)
        hideSprite.ForEach(r => r.enabled = false);
        showSprite.ForEach(r => r.enabled = true);
    }

    // 重写受伤逻辑（增加旗帜状态处理）
    public override void TakeDamage(float damage) {
        HP -= damage;
        // 播放闪烁效果
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // 旗帜状态变化
        if (HP <= 110) {
            HideHand();
            FlagBad(); // 旗帜破损
        }
        if (HP <= 20) {
            HideHead();
            FlagDead(); // 旗帜消失
        }
        // 死亡判断
        if (HP <= 0) ToDead();
    }

    // 旗帜破损
    private void FlagBad() {
        if (isFlagBad) return;
        isFlagBad = true;
        goodFlag.enabled = false;
        badFlag.enabled = true;
    }

    // 旗帜消失
    private void FlagDead() {
        if (isFlagDead) return;
        isFlagDead = true;
        badFlag.enabled = false;
        // 播放旗帜特效
        flagParticalPrefab.transform.parent = null;
        flagParticalPrefab.GetComponent<ParticleSystem>().Play();
        // 切换渲染器显示状态
        hideSprite.ForEach(r => r.enabled = true);
        showSprite.ForEach(r => r.enabled = false);
    }
}