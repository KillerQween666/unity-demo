using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 植物状态：禁用（未激活/死亡）、启用（正常工作）
enum PlantState {
    Disable,
    Enable
}

public class Plant : MonoBehaviour {
    // 当前植物状态，默认禁用
    PlantState plantState = PlantState.Disable;
    // 植物类型，默认向日葵
    public PlantType plantType = PlantType.sunFlower;

    // 植物自身动画组件（如闪烁、发光）
    public Animator animator;
    // 子物体动画组件（如豌豆射手攻击时的头部或眨眼动作）
    public Animator grandAnimator;

    // 植物生命值
    public int HP = 100;

    // 所有子物体的渲染器列表（用于受伤闪烁效果）
    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>();

    // 正常亮度和闪烁亮度（用于受伤效果）
    private float originalBright = 1f;
    private float flashBright = 2f;

    // 植物影子对象
    public GameObject shadow;

    // 初始化：收集所有渲染器（包含未激活的子物体）
    private void Start() {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            spriteList.Add(sprite);
        }
    }

    // 每帧更新：根据状态执行对应逻辑
    private void Update() {
        switch (plantState) {
            case PlantState.Disable:
                DisableUpdate();
                break;
            case PlantState.Enable:
                EnableUpdate();
                break;
        }
    }

    // 禁用状态的更新逻辑（默认空，子类可重写）
    void DisableUpdate() { }

    // 启用状态的更新逻辑（虚方法，子类需重写实现具体功能）
    protected virtual void EnableUpdate() { }

    // 切换到禁用状态：停止工作、关闭碰撞体、隐藏影子
    public void TransitionToDisable() {
        plantState = PlantState.Disable;
        GetComponent<Collider2D>().enabled = false;
        animator.enabled = false;
        grandAnimator.enabled = false;
        shadow.SetActive(false);
    }

    // 切换到启用状态：开始工作、开启碰撞体、显示影子
    public void TransitionToEnable() {
        plantState = PlantState.Enable;
        GetComponent<Collider2D>().enabled = true;
        animator.enabled = true;
        grandAnimator.enabled = true;
        shadow.SetActive(true);
    }

    // 受到伤害处理：扣血、触发闪烁动画，血量为0时执行死亡
    public void TakeDamage(int damage) {
        HP -= damage;
        animator.SetTrigger("flashTrigger");

        if (HP <= 0) {
            AudioManager.Instance.PlayClip(Config.eatFinish);
            Dead();
        }
    }

    // 播放受伤闪烁效果（亮度增强）
    public void PlayBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", flashBright);
        }
    }

    // 结束闪烁效果（恢复正常亮度）
    public void StopBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", originalBright);
        }
    }

    // 死亡逻辑：直接销毁植物对象
    private void Dead() {
        Destroy(gameObject);
    }

    public virtual void PlantFun() { }
}