using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 豌豆子弹脚本，控制子弹移动、生命周期、碰撞伤害及对象池回收
public class PeaBullet : MonoBehaviour {

    // 子弹移动速度（单位：单位/秒，向右飞行）
    public float speed = 3;
    // 子弹攻击力（命中僵尸时造成的伤害值）
    public float atkValue = 30;

    // 子弹最大存活时间（超时后自动回收，避免内存泄漏）
    public float liveTime = 5;
    // 子弹存活计时器（记录已存活时间）
    public float liveTimer;

    // 子弹是否已命中目标的标记（防止重复触发伤害）
    public bool isAttack = false;
    // 子弹是否已回收的标记（配合对象池，避免重复回收）
    public bool isRelease = false;

    // 固定时间步更新：控制子弹移动（FixedUpdate适合物理相关逻辑，移动更稳定）
    private void FixedUpdate() {
        // 子弹持续向右移动（基于世界坐标，不受旋转影响）
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // 每帧更新：检查子弹存活时间，超时未命中则回收
    private void Update() {
        liveTimer += Time.deltaTime;
        // 存活时间超过上限，且未命中、未回收时，回收到对象池
        if (liveTimer > liveTime && isAttack == false && isRelease == false) {
            ObjectPoolManager.Instance.ReleasePeaBullet(this.gameObject);
        }
    }

    // 碰撞触发：子弹命中僵尸时执行伤害逻辑
    private void OnTriggerEnter2D(Collider2D collision) {
        // 只对"Zombie"标签的对象生效
        if (collision.CompareTag("Zombie")) {
            if (isAttack == true) return; // 已命中过，避免重复伤害
            isAttack = true; // 标记为已命中

            // 播放子弹命中粒子效果（从对象池获取粒子）
            ObjectPoolManager.Instance.PlayPeaBulletParticalIEnumrator(transform);

            // 随机播放一种命中音效（增加音效多样性）
            var clips = new[] { Config.splat, Config.splat2, Config.splat3 };
            AudioManager.Instance.PlayClip(clips[Random.Range(0, 3)]);

            // 给命中的僵尸造成伤害
            collision.GetComponent<Zombie>().TakeDamage(atkValue);

            // 伤害处理完成后，将子弹回收到对象池
            ObjectPoolManager.Instance.ReleasePeaBullet(this.gameObject);
        }
    }

}