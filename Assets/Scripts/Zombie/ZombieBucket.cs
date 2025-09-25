using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 带桶僵尸类（继承自普通僵尸僵尸基类）
public class ZombieBucket : ZombieCommon {
    public SpriteRenderer goodBucket; // 完好的桶
    public SpriteRenderer badBucket; // 轻微破损的桶
    public SpriteRenderer worstBucket; // 严重破损的桶

    public Transform bucketEmissionTransform; // 桶破碎时的特效生成位置

    // 桶的状态标记（避免重复切换状态）
    bool isBucketBad = false; // 桶是否轻微破损
    bool isBucketWorst = false; // 桶是否严重破损
    bool isBucketDead = false; // 桶是否完全损坏

    private new void Awake() {
        base.Awake(); // 调用父类的初始化
        // 初始只只显示完好的桶
        badBucket.enabled = false;
        worstBucket.enabled = false;
    }

    // 重写受伤逻辑（增加桶的状态变化）
    public override void TakeDamage(float damage) {
        base.TakeDamage(damage); // 先执行父类的受伤处理

        // 根据生命值切换桶的状态
        if (HP < 375) {
            BucketBad(); // 切换到轻微破损
        }

        if (HP < 225) {
            BucketWorst(); // 切换到严重破损
        }

        if (HP < 100) {
            BucketDead(); // 桶完全损坏
        }

    }

    // 桶轻微破损处理
    private void BucketBad() {
        if (isBucketBad) return; // 已处于该状态则直接返回
        isBucketBad = true;

        goodBucket.enabled = false;
        badBucket.enabled = true; // 显示轻微破损的桶
    }

    // 桶严重破损处理
    private void BucketWorst() {
        if (isBucketWorst) return; // 已处于该状态则直接返回
        isBucketWorst = true;

        badBucket.enabled = false;
        worstBucket.enabled = true; // 显示严重破损的桶
    }

    // 桶完全损坏处理
    private void BucketDead() {
        if (isBucketDead) return; // 已处于该状态则直接返回
        isBucketDead = true;

        worstBucket.enabled = false; // 隐藏桶

        // 播放桶破碎的特效
        ObjectPoolManager.Instance.PlayBucketEmissionIEnumrator(bucketEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }

    // 重写受击音效（根据桶是否损坏播放不同音效）
    protected override void PlayAttackSource() {
        if (isBucketDead == true) {
            base.PlayAttackSource(); // 桶损坏后用父类的默认音效
        }
        else {
            // 桶完好时播放桶被攻击的音效（随机选一个）
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.bucket : Config.bucket2);
        }
    }
}