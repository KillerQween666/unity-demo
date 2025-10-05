using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 带桶和旗帜的僵尸类（继承自普通僵尸基类）
public class ZombieBucketFlag : ZombieCommon {

    public SpriteRenderer goodBucket; // 完好的桶
    public SpriteRenderer badBucket; // 轻微破损的桶
    public SpriteRenderer worstBucket; // 严重破损的桶

    public Transform bucketEmissionTransfrom; // 桶破碎特效的生成位置

    public SpriteRenderer goodFlag; // 完好的旗帜
    public SpriteRenderer badFlag; // 破损的旗帜

    public Transform flagEmissionTransfrom; // 旗帜消失特效的生成位置

    // 桶的状态标记（避免重复切换）
    bool isBucketBad = false; // 桶是否轻微破损
    bool isBucketWorst = false; // 桶是否严重破损
    bool isBucketDead = false; // 桶是否完全损坏

    // 旗帜的状态标记（避免重复切换）
    bool isFlagBad = false; // 旗帜是否破损
    bool isFlagDead = false; // 旗帜是否是否消失

    private new void Awake() {
        base.Awake(); // 调用父类的初始化方法
        // 初始状态：只显示完好的桶和旗帜，隐藏破损状态
        badBucket.enabled = false;
        worstBucket.enabled = false;
        badFlag.enabled = false;
    }

    // 重写受伤逻辑（增加桶和旗帜的状态变化）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 根据生命值区间切换桶的状态
        if (HP < 450) {
            BucketBad(); // 切换到轻微破损
        }

        if (HP < 325) {
            BucketWorst(); // 切换到严重破损
        }

        if (HP < 200) {
            BucketDead(); // 桶完全损坏
        }

        // 根据生命值区间切换旗帜的状态
        if (HP < 100) {
            FlagBad(); // 旗帜破损
        }

        if (HP < 20) {
            FlagDead(); // 旗帜消失
        }

    }

    // 桶轻微破损处理
    private void BucketBad() {
        if (isBucketBad) return; // 已处于该状态则不重复执行
        isBucketBad = true;

        goodBucket.enabled = false;
        badBucket.enabled = true; // 显示轻微破损的桶
    }

    // 桶严重破损处理
    private void BucketWorst() {
        if (isBucketWorst) return; // 已处于该状态则不重复执行
        isBucketWorst = true;

        badBucket.enabled = false;
        worstBucket.enabled = true; // 显示严重破损的桶
    }

    // 桶完全损坏处理
    private void BucketDead() {
        if (isBucketDead) return; // 已处于该状态则不重复执行
        isBucketDead = true;

        worstBucket.enabled = false; // 隐藏桶

        // 播放桶破碎的特效
        ObjectPoolManager.Instance.PlayBucketEmissionIEnumrator(bucketEmissionTransfrom, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 重写受击音效（根据桶的状态播放不同音效）
    protected override void PlayAttackSource() {
        if (isBucketDead == true) {
            base.PlayAttackSource(); // 桶损坏后使用父类的默认音效
        }
        else {
            // 桶完好时播放桶被攻击的音效（随机选择）
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.bucket : Config.bucket2);
        }
    }

    // 旗帜破损处理
    private void FlagBad() {
        if (isFlagBad) return; // 已处于该状态则不重复执行
        isFlagBad = true;
        goodFlag.enabled = false;
        badFlag.enabled = true; // 显示破损的旗帜
    }

    // 旗帜消失处理
    private void FlagDead() {
        if (isFlagDead) return; // 已处于该状态则不重复执行
        isFlagDead = true;
        badFlag.enabled = false; // 隐藏旗帜
        // 播放旗帜消失的特效
        ObjectPoolManager.Instance.PlayFlagEmissionIEnumrator(flagEmissionTransfrom, spriteList[0].sortingOrder + 100, isCroze);

        // 切换僵尸的显示状态（隐藏部分渲染器，显示另一部分）
        hideSprite.ForEach(r => r.enabled = true);
        showSprite.ForEach(r => r.enabled = false);
    }
}