using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRock : Plant {
    // 发射豌豆的间隔时间（秒）
    public float attackDuration;
    // 发射豌豆的计时器
    private float attackTimer;

    // 检测僵尸的碰撞体（限定检测范围）
    public Collider2D attackBox;

    public float atkValue = 12.5f;

    public SpriteRenderer spike1;
    public SpriteRenderer spike2;

    private int pierceCount = 0;

    // 启用状态逻辑：计时，到时间检测僵尸，有僵尸则触发攻击动画
    protected override void EnableUpdate() {
        attackTimer += Time.deltaTime;

        if (attackTimer > attackDuration) {
            attackTimer = 0;

            // 检测碰撞体范围内的所有对象（在外面已经专门限制了僵尸层）
            Bounds bounds = attackBox.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,
                bounds.size,
                attackBox.transform.rotation.eulerAngles.z,
                LayerMask.GetMask("Zombie")
            );

            // 只要检测到任何碰撞体（僵尸），就触发攻击动画
            if (hitColliders.Length > 0) // 检查是否有检测到的对象
            {
                animator.SetTrigger("attackTrigger");
            }
        }
    }

    // 发射豌豆（由攻击动画的事件调用）
    public override void PlantFun() {
        // 检测爆炸范围内的所有僵尸（仅检测"Zombie"层）
        Bounds bounds = attackBox.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,       // 爆炸范围中心（碰撞体中心点）
            bounds.size,         // 爆炸范围大小（碰撞体尺寸）
            attackBox.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
            LayerMask.GetMask("Zombie") // 目标检测层：僵尸层
        );

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
                                // 尝试获取碰撞体上的Zombie组件
                if (coll.TryGetComponent<Enemy>(out var enemy)) {
                    enemy.TakeDamage(atkValue);
                }
            }
        }
    }

    public void PierceZombieZamboni() {
        pierceCount++;
        if (pierceCount == 1) {
            spike1.enabled = false;
        } 
        if (pierceCount == 2) {
            spike2.enabled = false;
        }
        if (pierceCount >= 3) {
            Dead();
        }
    }
}
