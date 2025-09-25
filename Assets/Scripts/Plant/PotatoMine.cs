using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 土豆地雷状态枚举：定义土豆地雷的生命周期状态
public enum PotateState {
    Wait,   // 等待状态（刚种下，未激活）
    Grow,   // 生长状态（激活中，准备就绪）
    Ready   // 就绪状态（可触发爆炸）
}

// 土豆地雷类（继承自植物基类，实现延时激活、接触爆炸功能）
public class PotatoMine : Plant {
    private PotateState potateState = PotateState.Wait; // 当前土豆地雷状态（默认等待）

    public float waitTime = 10; // 等待激活的时间（秒）
    private float waitTimer = 0; // 等待激活的计时器

    public Collider2D boomBox; // 爆炸范围碰撞体（用于检测爆炸范围内的僵尸）

    // 碰撞检测：就绪状态下接触僵尸触发爆炸
    private void OnTriggerEnter2D(Collider2D collision) {
        if (potateState != PotateState.Ready) return; // 非就绪状态不响应碰撞

        if (collision.CompareTag("Zombie")) { // 检测到僵尸
            ObjectPoolManager.Instance.PlayPotatoBoomParticalIEnumrator(transform); // 播放爆炸前特效
            StartCoroutine(Boom()); // 启动爆炸协程
        }
    }

    // 启用状态更新逻辑：根据当前状态执行对应行为
    protected override void EnableUpdate() {
        switch (potateState) {
            case PotateState.Wait:
                WaitUpdate(); // 等待状态：计时等待激活
                break;
            case PotateState.Grow:
                break; // 生长状态：仅播放动画，无额外更新逻辑
            case PotateState.Ready:
                break; // 就绪状态：仅通过碰撞触发，无帧更新逻辑
            default:
                break;
        }
    }

    // 等待状态更新：累计时间，达到阈值后切换到生长状态
    void WaitUpdate() {
        waitTimer += Time.deltaTime;
        if (waitTimer > waitTime) {
            TransitionToGrow(); // 切换到生长状态
        }
    }

    // 切换到生长状态（激活过程）
    public void TransitionToGrow() {
        potateState = PotateState.Grow;
        AudioManager.Instance.PlayClip(Config.potatoRise); // 播放生长音效
        animator.SetTrigger("growTrigger"); // 触发生长动画
    }

    // 切换到就绪状态（可爆炸）
    public void TransitionToReady() {
        potateState = PotateState.Ready;
        boomBox.enabled = true; // 开启爆炸范围检测

        if (haveZombie()) { // 若就绪时范围内已有僵尸，直接触发爆炸
            StartCoroutine(Boom());
        }
    }

    // 检测当前位置是否有僵尸（用于就绪时的即时判定）
    bool haveZombie() {
        Collider2D collider = GetComponent<Collider2D>();
        Bounds bounds = collider.bounds;
        // 检测自身范围内的僵尸（仅"Zombie"层）
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,
            bounds.size,
            collider.transform.rotation.eulerAngles.z,
            LayerMask.GetMask("Zombie")
        );

        return hitColliders.Length > 0; // 有僵尸则返回true
    }

    // 爆炸逻辑协程（播放动画、音效，造成范围伤害）
    IEnumerator Boom() {
        // 检测爆炸范围内的所有僵尸
        Bounds bounds = boomBox.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,
            bounds.size,
            boomBox.transform.rotation.eulerAngles.z,
            LayerMask.GetMask("Zombie")
        );

        AudioManager.Instance.PlayClip(Config.potatoBoom); // 播放爆炸音效
        animator.SetTrigger("boomTrigger"); // 触发爆炸动画
        yield return new WaitForSeconds(0.05f); // 等待动画帧，确保视觉效果同步

        // 对爆炸范围内的所有僵尸执行死亡逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用
                if (coll.TryGetComponent<Zombie>(out var zombie)) { // 获取僵尸组件
                    zombie.Dead(); // 直接杀死僵尸
                }
            }
        }

        yield return new WaitForSeconds(0.6f); // 等待爆炸动画结束
        Dead(); // 销毁土豆地雷自身
    }

    // 植物专属功能（由生长动画事件调用，切换到就绪状态）
    public override void PlantFun() {
        TransitionToReady();
    }
}