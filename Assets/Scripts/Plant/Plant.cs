using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 植物状态枚举：定义植物的工作状态
enum PlantState {
    Disable,  // 禁用（未激活、死亡或停止工作）
    Enable    // 启用（正常生长、工作）
}

// 植物基类：封装所有植物的通用逻辑（状态管理、受击、渲染控制等）
public class Plant : MonoBehaviour {
    PlantState plantState = PlantState.Disable; // 当前植物状态（默认禁用）
    public PlantType plantType = PlantType.sunFlower; // 该植物对应的类型

    public Animator animator; // 植物自身动画组件（如整体动作、发光）
    public Animator grandAnimator; // 子物体动画组件（如局部细节动作）
    public Collider2D grandCollider2D; // 子物体碰撞体（如攻击判定区域）

    public int HP = 100; // 植物生命值

    protected List<SpriteRenderer> spriteList = new List<SpriteRenderer>(); // 所有渲染器列表（用于受击闪烁）

    // 亮度参数（控制受击/高亮效果）
    protected float originalBright = 1f; // 正常显示亮度
    protected float flashBright = 2f; // 闪烁/高亮时的亮度

    [HideInInspector]
    public bool isBrighten = false; // 是否处于高亮状态（隐藏在Inspector，内部控制）

    public GameObject shadow; // 植物的影子对象

    // 初始化：收集所有子物体的渲染器（含未激活的）
    private void Start() {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            spriteList.Add(sprite);
        }
    }

    // 每帧更新：根据当前状态执行对应逻辑
    private void Update() {
        switch (plantState) {
            case PlantState.Disable:
                DisableUpdate(); // 禁用状态的更新
                break;
            case PlantState.Enable:
                EnableUpdate(); // 启用状态的更新
                break;
        }
    }

    // 禁用状态更新逻辑（默认空实现，子类可扩展）
    void DisableUpdate() { }

    // 启用状态更新逻辑（虚方法，子类需重写实现具体功能）
    protected virtual void EnableUpdate() { }

    // 切换到禁用状态（停止工作、关闭交互）
    public void TransitionToDisable() {
        plantState = PlantState.Disable;
        GetComponent<Collider2D>().enabled = false; // 关闭自身碰撞
        animator.enabled = false; // 停止自身动画
        if (grandAnimator != null) grandAnimator.enabled = false; // 停止子物体动画
        if (grandCollider2D != null) grandCollider2D.enabled = false; // 关闭子物体碰撞
        shadow.SetActive(false); // 隐藏影子
    }

    // 切换到启用状态（开始工作、开启交互）
    public void TransitionToEnable() {
        plantState = PlantState.Enable;
        GetComponent<Collider2D>().enabled = true; // 开启自身碰撞
        animator.enabled = true; // 启动自身动画
        if (grandAnimator != null) grandAnimator.enabled = true; // 启动子物体动画
        if (grandCollider2D != null) grandCollider2D.enabled = true; // 开启子物体碰撞
        shadow.SetActive(true); // 显示影子
    }

    // 受击处理（扣血、闪烁，血量为0时死亡）
    public virtual void TakeDamage(int damage) {
        HP -= damage;
        if (!isBrighten) StartCoroutine(PlayFlash()); // 未高亮时播放受击闪烁

        if (HP <= 0) {
            AudioManager.Instance.PlayClip(Config.eatFinish); // 播放被吃掉的音效
            Dead(); // 执行死亡逻辑
        }
    }

    // 播放高亮效果（增强亮度）
    public void PlayBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", flashBright);
        }
    }

    // 结束高亮效果（恢复正常亮度）
    public void StopBright() {
        foreach (var sprite in spriteList) {
            sprite.material.SetFloat("_Brightness", originalBright);
        }
    }

    // 受击闪烁效果协程
    public IEnumerator PlayFlash() {
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", flashBright)); // 提亮
        yield return new WaitForSeconds(0.15f); // 持续0.15秒
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", originalBright)); // 恢复
    }

    // 死亡逻辑（默认销毁对象，子类可重写）
    public virtual void Dead() {
        Destroy(gameObject);
    }

    // 植物专属功能方法（虚方法，子类实现具体能力）
    public virtual void PlantFun() { }
}