using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 食人花状态枚举：定义食人花的生命周期行为状态
public enum ChomperState {
    Idle,    // 闲置状态（可检测并准备攻击）
    Eat,     // 进食状态（咬住僵尸后持续吞噬）
    Attack,  // 攻击状态（向僵尸发起扑咬）
    Swallow  // 吞咽状态（进食结束后复位前的过渡）
}

// 食人花类（继承自植物基类，实现扑咬、进食、吞咽的状态机逻辑）
public class Chomper : Plant {

    private ChomperState chomperState = ChomperState.Idle; // 当前食人花状态（默认闲置）

    public float eatTime = 10f; // 进食持续时间（秒，咬住僵尸后保持该状态的时长）
    private float eatTimer = 0; // 进食计时器（记录已进食的时间）

    public ChomperAttackBox ChomperAttackBox; // 食人花的攻击检测盒（管理攻击范围和判定）

    // 启用状态更新逻辑：根据当前状态执行对应行为
    protected override void EnableUpdate() {
        switch (chomperState) {
            case ChomperState.Idle:
                break; // 闲置状态：无额外帧更新逻辑（仅等待攻击触发）
            case ChomperState.Attack:
                break; // 攻击状态：仅播放攻击动画，无帧更新逻辑
            case ChomperState.Eat:
                EatUpdate(); // 进食状态：计时，到点切换吞咽状态
                break;
            case ChomperState.Swallow:
                break; // 吞咽状态：仅播放吞咽动画，无帧更新逻辑
            default:
                break;
        }
    }

    // 进食状态更新：累计进食时间，达到阈值后切换吞咽状态
    void EatUpdate() {
        eatTimer += Time.deltaTime;
        if (eatTimer > eatTime) {
            chomperState = ChomperState.Swallow;
            animator.SetTrigger("swallowTrigger"); // 触发吞咽动画
        }
    }

    // 切换到攻击状态
    public void TranstionToAttack() {
        chomperState = ChomperState.Attack;
        ChomperAttackBox.GetComponent<Collider2D>().enabled = false; // 关闭攻击检测盒（避免重复攻击）
    }

    // 切换到进食状态
    public void TranstionToEat() {
        eatTimer = 0; // 重置进食计时器
        chomperState = ChomperState.Eat;
        animator.SetBool("isEat", true); // 开启进食动画状态
    }

    // 切换到闲置状态（进食/吞咽结束后复位）
    public void TranstionToIdle() {
        chomperState = ChomperState.Idle;
        ChomperAttackBox.isAttack = false; // 重置攻击检测盒的攻击标记
        ChomperAttackBox.GetComponent<Collider2D>().enabled = true; // 重新开启攻击检测盒（可再次攻击）
        animator.SetBool("isEat", false); // 关闭进食动画状态
    }

    // 植物专属功能（由吞咽动画事件调用，复位到闲置状态）
    public override void PlantFun() {
        TranstionToIdle();
    }

}