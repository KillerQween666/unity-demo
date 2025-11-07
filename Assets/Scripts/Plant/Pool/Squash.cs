using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squash : Plant {
    // 发射豌豆的间隔时间（秒）
    public float findDuration = 0.5f;
    // 发射豌豆的计时器
    private float findTimer;

    // 检测僵尸的碰撞体（限定检测范围）
    public Collider2D leftColl2D;
    public Collider2D rightColl2d;
    public Collider2D attackBox;

    private Zombie pushZombie;

    private Vector3 attackPosition;

    private bool isAttack = false;

    // 启用状态逻辑：计时，到时间检测僵尸，有僵尸则触发攻击动画
    protected override void EnableUpdate() {
        findTimer += Time.deltaTime;

        if (findTimer > findDuration) {
            findTimer = 0;

            if (isAttack == true) return;

            // 检测碰撞体范围内的所有对象（在外面已经专门限制了僵尸层）
            Bounds bounds = leftColl2D.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,
                bounds.size,
                leftColl2D.transform.rotation.eulerAngles.z,
                LayerMask.GetMask("Zombie")
            );

            // 只要检测到任何碰撞体（僵尸），就触发攻击动画
            if (hitColliders.Length > 0) // 检查是否有检测到的对象
            {
                if (isAttack == true) return;
                isAttack = true;

                AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.squashHmm : Config.squashHmm2);
                animator.SetTrigger("leftTrigger");
                // 对爆炸范围内的僵尸执行伤害逻辑
                foreach (var coll in hitColliders) {
                    if (coll != null) { // 避免空引用异常
                                        // 尝试获取碰撞体上的Zombie组件
                        if (coll.TryGetComponent<Zombie>(out var zombie)) {
                            pushZombie = zombie;
                            break;
                        }
                    }
                }
                StartCoroutine(MoveZombieTop());
            }

            // 检测碰撞体范围内的所有对象（在外面已经专门限制了僵尸层）
            Bounds bounds2 = rightColl2d.bounds;
            Collider2D[] hitColliders2 = Physics2D.OverlapBoxAll(
                bounds2.center,
                bounds2.size,
                rightColl2d.transform.rotation.eulerAngles.z,
                LayerMask.GetMask("Zombie")
            );

            // 只要检测到任何碰撞体（僵尸），就触发攻击动画
            if (hitColliders2.Length > 0) // 检查是否有检测到的对象
            {
                if (isAttack == true) return;
                isAttack = true;

                AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.squashHmm : Config.squashHmm2);
                animator.SetTrigger("rightTrigger");
                foreach (var coll in hitColliders2) {
                    if (coll != null) { // 避免空引用异常
                                        // 尝试获取碰撞体上的Zombie组件
                        if (coll.TryGetComponent<Zombie>(out var zombie)) {
                            pushZombie = zombie;
                            break;
                        }
                    }
                }

                StartCoroutine(MoveZombieTop());
            }
        }
    }

    private IEnumerator MoveZombieTop() {
        attackPosition = pushZombie.bodyTransform.position;
        attackPosition.y += 2.3f;

        yield return new WaitForSeconds(0.4f);

        if (selfCell.topPlant != null) {
            selfCell.topPlant = null;
        } else {
            selfCell.currentPlant = null;
        }

        GetComponent<Collider2D>().enabled = false;

        transform.DOMove(attackPosition, 0.75f);

        shadow.transform.SetParent(null);
        shadow.transform.DOMoveX(attackPosition.x, 0.75f);
        yield return new WaitForSeconds(0.75f);
    }

    // 发射豌豆（由攻击动画的事件调用）
    public override void PlantFun() {
        StartCoroutine(MoveZombieBottom());
    }

    private IEnumerator MoveZombieBottom() {
        attackPosition.y -= 2.6f;

        transform.DOMove(attackPosition, 0.5f);
        yield return new WaitForSeconds(0.5f);

        // 检测碰撞体范围内的所有对象（在外面已经专门限制了僵尸层）
        Bounds bounds = attackBox.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,
            bounds.size,
            attackBox.transform.rotation.eulerAngles.z,
            LayerMask.GetMask("Zombie")
        );

        AudioManager.Instance.PlayClip(Config.squashThump);

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
                                // 尝试获取碰撞体上的Zombie组件
                if (coll.TryGetComponent<Enemy>(out var enemy)) {
                    if (coll.TryGetComponent<Zombie>(out var zombie) && zombie.isCanBoom && zombie.HP <= 600) {
                        zombie.ToDead();
                    } else {
                        enemy.TakeDamage(600, 2);
                    }
                        
                }
            }
        }

        Dead();
    }

    public override void Dead() {
        base.Dead();
        if (shadow != null) Destroy(shadow);
    }
}
