using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePeaBullet : PeaBullet {

    public Collider2D attackBox;

    protected override void ReleaseBullet() {
        ObjectPoolManager.Instance.ReleaseFirePeaBullet(this.gameObject);
    }

    // 重写碰撞检测逻辑（实现寒冰子弹的专属效果）
    public override void OnTriggerEnter2D(Collider2D collision) {
        // 仅对标签为"Zombie"的对象执行逻辑
        if (collision.CompareTag("Zombie")) {
            // 检测爆炸范围内的所有僵尸（仅检测"Zombie"层）
            Bounds bounds = attackBox.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,       // 爆炸范围中心（碰撞体中心点）
                bounds.size,         // 爆炸范围大小（碰撞体尺寸）
                attackBox.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
                LayerMask.GetMask("Zombie") // 目标检测层：僵尸层
            );

            ObjectPoolManager.Instance.PlayFirePeaBulletSwfIEnumrator(transform);
            AudioManager.Instance.PlayClip(Config.firepea);

            // 对爆炸范围内的僵尸执行伤害逻辑
            foreach (var coll in hitColliders) {
                if (coll != null) { // 避免空引用异常
                    if (coll.TryGetComponent<Enemy>(out var enemy)) {
                        enemy.TakeDamage(atkValue, 4);
                    }
                }
            }

            if (isRelease == false) ObjectPoolManager.Instance.ReleaseFirePeaBullet(this.gameObject);
        }
    }

}
