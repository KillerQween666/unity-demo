using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 双发射手类（继承自植物基类，实现双发豌豆功能）
public class TwoPeaShooter : Plant {
    public float shootDuration; // 发射豌豆的间隔时间（秒）
    private float shootTimer;   // 发射计时器（累计时间，到点触发发射）

    public Transform shootPointTransform; // 豌豆发射点（决定豌豆生成位置）
    public Collider2D coll2D; // 检测僵尸的碰撞体（限定定攻击僵尸的范围）

    // 启用状态更新逻辑：计时并检测僵尸，满足条件时触发攻击
    protected override void EnableUpdate() {
        shootTimer += Time.deltaTime; // 累加计时器

        // 达到发射间隔时，检测是否有僵尸
        if (shootTimer > shootDuration) {
            shootTimer = 0; // 重置计时器

            // 获取碰撞体范围，检测该范围内的僵尸（仅检测"Zombie"层）
            Bounds bounds = coll2D.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,       // 检测范围中心
                bounds.size,         // 检测范围大小
                coll2D.transform.rotation.eulerAngles.z, // 旋转角度
                LayerMask.GetMask("Zombie") // 只检测僵尸层
            );

            // 检测到僵尸时，触发攻击动画
            if (hitColliders.Length > 0) {
                grandAnimator.SetTrigger("attackTrigger");
            }
        }
    }

    // 发射豌豆（由攻击动画事件调用）
    public override void PlantFun() {
        // 随机播放一种种发射音效
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.shoot : Config.shoot2);

        // 从对象池获取豌豆子弹，设置发射位置
        GameObject obj = ObjectPoolManager.Instance.GetPeaBullet();
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.transform.position = shootPointTransform.position;
    }

}