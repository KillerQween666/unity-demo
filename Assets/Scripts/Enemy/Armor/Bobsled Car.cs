using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BobsledCar : Enemy {
    public ZombieBobsled zombie;

    public Coroutine flashCoroutine; // 闪烁效果的协程
    private Collider2D coll;

    private void Awake() {
        coll = GetComponent<Collider2D>();
    }

    public override void TakeDamage(float damage, int hurtType = 0) {
        if (zombie.isCarDead) return;

        base.TakeDamage(damage, hurtType);

        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(zombie.PlayCarFlash());

        if (hurtType == 0 || hurtType == 3) zombie.PlayAttackSource();

        // 根据生命值切换报纸的状态
        if (HP < 150) {
            zombie.ShowCar2(); // 切换到轻微破损
        }

        if (HP < 100) {
            zombie.ShowCar3(); // 切换到严重破损
        }

        if (HP < 50) {
            zombie.ShowCar4();
        }

        if (HP <= 0) {
            zombie.SitStop();
            zombie.CarDead(); // 报纸完全损坏 
        }
    }

}

