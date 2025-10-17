using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : Enemy {
    public ZombiePaper zombie;

    public Coroutine flashCoroutine; // 闪烁效果的协程
    private Collider2D coll;

    private void Awake() {
        coll = GetComponent<Collider2D>();
    }

    public override void TakeDamage(float damage, int hurtType = 0) {
        base.TakeDamage(damage, hurtType);

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(zombie.PlayPaperFlash());

        if (hurtType == 0 || hurtType == 3) zombie.PlayAttackSource();
        if (hurtType == 3)  zombie.PlaySlowSpeed();
        

        // 根据生命值切换报纸的状态
        if (HP < 75) {
            zombie.PaperBad(); // 切换到轻微破损
        }

        if (HP < 37.5) {
            zombie.PaperWorst(); // 切换到严重破损
        }

        if (HP <= 0) {
            zombie.PaperDead(); // 报纸完全损坏
            if (hurtType != 1 && hurtType != 2) {
                AudioManager.Instance.PlayClip(Config.paper);
                zombie.PlayPaperEmission();
            }
            Destroy(this.gameObject);
        }
    }

}
