using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 带路障僵尸类（继承自普通僵尸基类，扩展路障状态逻辑）
public class ZombieCone : ZombieCommon {
    public SpriteRenderer goodCone; // 完好的路障
    public SpriteRenderer badCone; // 轻微破损的路障
    public SpriteRenderer worstCone; // 严重破损的路障

    public Transform coneEmissionTransform; // 路障破碎时的特效生成位置

    // 路障状态标记（防止重复切换状态）
    bool isConeBad = false; // 路障是否轻微破损
    bool isConeWorst = false; // 路障是否严重破损
    bool isConeDead = false; // 路障是否完全损坏

    private new void Awake() {
        base.Awake(); // 调用父类的初始化方法
        // 初始仅显示完好路障，隐藏破损状态
        badCone.enabled = false;
        worstCone.enabled = false;
    }

    // 重写受伤逻辑（新增路障状态随生命值变化的处理）
    public override void TakeDamage(float damage) {
        base.TakeDamage(damage); // 先执行父类受伤逻辑（扣血、身体部位隐藏等）

        // 根据生命值区间切换路障状态
        if (HP < 200) {
            ConeBad(); // 切换至轻微破损状态
        }

        if (HP < 150) {
            ConeWorst(); // 切换至严重破损状态
        }

        if (HP < 100) {
            ConeDead(); // 切换至路障完全损坏状态
        }

    }

    // 路障轻微破损处理
    private void ConeBad() {
        if (isConeBad) return; // 已处于该状态则直接返回
        isConeBad = true;

        goodCone.enabled = false;
        badCone.enabled = true; // 显示轻微破损的路障
    }

    // 路障严重破损处理
    private void ConeWorst() {
        if (isConeWorst) return; // 已处于该状态则直接返回
        isConeWorst = true;

        badCone.enabled = false;
        worstCone.enabled = true; // 显示严重破损的路障
    }

    // 路障完全损坏处理
    private void ConeDead() {
        if (isConeDead) return; // 已处于该状态则直接返回
        isConeDead = true;

        worstCone.enabled = false; // 隐藏路障

        // 播放路障破碎特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayConeEmissionIEnumrator(coneEmissionTransform, spriteList[0].sortingOrder + 100, isCroze);
    }
}