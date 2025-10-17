using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Enemy {
    public ZombieBucketDoor zombie;

    public Coroutine flashCoroutine; // 闪烁效果的协程
    private Collider2D coll;

    private void Awake() {
        coll = GetComponent<Collider2D>();
    }

    public override void TakeDamage(float damage, int hurtType = 0) {
        base.TakeDamage(damage, hurtType);

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(zombie.PlayDoorFlash());

        // 播放铁门被攻击的音效（随机选择）
        if (hurtType == 0 || hurtType == 3) zombie.PlayAttackSource();

        if (HP < 250) {
            zombie.DoorBad();
        }

        if (HP < 125) {
            zombie.DoorWorst();
        }

        if (HP <= 0) {
            // 切换僵尸的显示状态（隐藏部分渲染器，显示另一部分）
            zombie.hideSprite.ForEach(r => r.enabled = true);
            zombie.showSprite.ForEach(r => r.enabled = false);
            zombie.DoorDead();
            if (hurtType != 1 && hurtType != 2) zombie.PlayDoorEmission();
            Destroy(this.gameObject);
        }
    }

}
