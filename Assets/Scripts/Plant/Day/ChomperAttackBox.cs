using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 食人花攻击检测盒：管理食人花的攻击范围判定与攻击逻辑触发
public class ChomperAttackBox : MonoBehaviour {

    public Chomper chomper; // 关联的食人花主体（用于状态切换和动画控制）
    public bool isAttack = false; // 攻击状态标记（避免重复触发攻击）

    // 碰撞检测：检测到僵尸时触发攻击
    private void OnTriggerEnter2D(Collider2D collision) {
        // 仅对标签为"Zombie"的对象执行攻击逻辑
        if (collision.CompareTag("Zombie") && collision.TryGetComponent<Zombie>(out var zombie)) {
            chomper.animator.SetTrigger("attackTrigger"); // 触发食人花的攻击动画
            chomper.TranstionToAttack(); // 将食人花切换到攻击状态

            StartCoroutine(AttackZombie(zombie)); // 启动攻击僵尸的协程（同步动画与逻辑）
        }
    }

    // 攻击僵尸协程（配合动画节奏执行伤害与状态切换）
    IEnumerator AttackZombie(Zombie zombie) {
        // 等待0.8秒（匹配攻击动画的扑咬动作时长，确保视觉与逻辑同步）
        yield return new WaitForSeconds(0.8f);

        AudioManager.Instance.PlayClip(Config.eatZombie); // 播放吞噬僵尸的音效

        if (zombie != null) { // 若僵尸仍存在（未被其他攻击消灭）
            if (isAttack == false) { // 未触发过攻击，执行吞噬逻辑
                zombie.TakeDamage(10000, 2);
                chomper.TranstionToEat(); // 将食人花切换到进食状态
            }
            isAttack = true; // 标记为已攻击，防止重复触发  
        }
        else { // 若僵尸已消失（被其他攻击消灭）
            yield return new WaitForSeconds(0.1f); // 短暂延迟后
            if (isAttack == false) chomper.TranstionToIdle(); // 未攻击则复位到闲置状态
        }
    }
}