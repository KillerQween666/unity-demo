using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 寒冰射手类（继承自植物基类，实现发射减速豌豆功能）
public class SnowPeaShooter : Plant {
    public float shootDuration; // 发射豌豆的间隔时间（单位：秒）
    private float shootTimer;   // 发射计时器（累计时间，到点触发攻击）

    public Transform shootPointTransform; // 豌豆发射点（决定寒冰豌豆生成的位置）
    public Collider2D coll2D;             // 检测僵尸的碰撞体（限定攻击检测范围）

    public SnowPeaBullet snowPeaBullet;   // 寒冰豌豆子弹预制体（备用引用）

    // 启用状态更新逻辑：计时检测僵尸，满足条件触发攻击
    protected override void EnableUpdate() {
        shootTimer += Time.deltaTime; // 累加发射计时器

        // 达到发射间隔时，执行检测逻辑
        if (shootTimer > shootDuration) {
            shootTimer = 0; // 重置计时器

            // 检测碰撞体范围内的僵尸（仅检测"Zombie"层，提升性能）
            Bounds bounds = coll2D.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,       // 检测范围中心（碰撞体中心点）
                bounds.size,         // 检测范围大小（碰撞体尺寸）
                coll2D.transform.rotation.eulerAngles.z, // 检测范围旋转角度
                LayerMask.GetMask("Zombie") // 目标检测层：僵尸层
            );

            // 检测到僵尸时，触发攻击动画
            if (hitColliders.Length > 0) {
                grandAnimator.SetTrigger("attackTrigger");
            }
        }
    }

    // 发射寒冰豌豆（由攻击动画的事件调用）
    public override void PlantFun() {
        // 随机播放一种发射音效（增加音效多样性）
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.shoot : Config.shoot2);

        // 从对象池获取寒冰豌豆子弹，设置发射位置
        GameObject obj = ObjectPoolManager.Instance.GetSnowPeaBullet();
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.transform.position = shootPointTransform.position;
    }
}