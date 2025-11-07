using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : Plant {
    // 发射豌豆的间隔时间（秒）
    public float shootDuration;
    // 发射豌豆的计时器
    private float shootTimer;

    // 豌豆发射点（决定豌豆从哪里射出）
    public Transform shootPointTransform;
    // 检测僵尸的碰撞体（限定检测范围）
    public Collider2D coll2D;

    // 启用状态逻辑：计时，到时间检测僵尸，有僵尸则触发攻击动画
    protected override void EnableUpdate() {
        shootTimer += Time.deltaTime;

        if (shootTimer > shootDuration) {
            shootTimer = 0;

            // 检测碰撞体范围内的所有对象（在外面已经专门限制了僵尸层）
            Bounds bounds = coll2D.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,
                bounds.size,
                coll2D.transform.rotation.eulerAngles.z,
                LayerMask.GetMask("Zombie")
            );

            // 只要检测到任何碰撞体（僵尸），就触发攻击动画
            if (hitColliders.Length > 0) // 检查是否有检测到的对象
            {
                grandAnimator.SetTrigger("attackTrigger");
            }
        }
    }

    // 发射豌豆（由攻击动画的事件调用）
    public override void PlantFun() {
        // 随机播放两种发射音效
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.shoot : Config.shoot2);

        // 从对象池获取豌豆并设置发射位置
        GameObject obj = ObjectPoolManager.Instance.GetPeaBullet();
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.transform.position = shootPointTransform.position;
    }

}