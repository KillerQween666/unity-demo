using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffPeaBullet : PeaBullet {
    public override void OnTriggerEnter2D(Collider2D collision) {
        // 仅对标签为"Zombie"的对象执行逻辑
        if (collision.CompareTag("Zombie")) {
            if (isAttack == true) return; // 已命中过目标，避免重复造成伤害
            isAttack = true; // 标记子弹已命中，锁定状态

            // 从对象池获取并播放寒冰豌豆命中的粒子特效
            ObjectPoolManager.Instance.PlayPuffPeaBulletParticalIEnumrator(transform);

            // 获取命中的僵尸组件，执行伤害和减速
            if (collision != null) {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null) {
                    if (collision.TryGetComponent<Zombie>(out var zombie)) {
                        zombie.TakeDamage(atkValue);
                    }
                    else {
                        enemy.TakeDamage(atkValue);
                    }

                }
            }

            // 子弹完成使命，回收到对象池（复用，减少性能消耗）
            ObjectPoolManager.Instance.ReleasePuffPeaBullet(this.gameObject);
        }
    }
}
