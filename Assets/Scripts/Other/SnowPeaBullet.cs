using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 寒冰豌豆子弹类（继承自普通豌豆子弹基类，扩展减速效果）
public class SnowPeaBullet : PeaBullet {
    // 重写碰撞检测逻辑（实现寒冰子弹的专属效果）
    public override void OnTriggerEnter2D(Collider2D collision) {
        // 仅对标签为"Zombie"的对象执行逻辑
        if (collision.CompareTag("Zombie")) {
            if (isAttack == true) return; // 已命中过目标，避免重复造成伤害
            isAttack = true; // 标记子弹已命中，锁定状态

            // 从对象池获取并播放寒冰豌豆命中的粒子特效
            ObjectPoolManager.Instance.PlaySnowPeaBulletParticalIEnumrator(transform);

            // 获取命中的僵尸组件，执行伤害和减速
            if (collision != null) {
                Enemy enemy = collision.GetComponent<Enemy>();
                if (enemy != null) {
                    if (collision.TryGetComponent<Zombie>(out var zombie)) {
                        // 播放僵尸被炸飞的特效（传入僵尸位置、是否有头、特效层级）
                        if (zombie.IsCroze() == false) AudioManager.Instance.PlayClip(Config.Frozen);
                        zombie.TakeDamage(atkValue, 3);
                    } else {
                        enemy.TakeDamage(atkValue, 3);
                    }


                        
                }
            }

            // 子弹完成使命，回收到对象池（复用，减少性能消耗）
            ObjectPoolManager.Instance.ReleaseSnowPeaBullet(this.gameObject);
        }
    }
}