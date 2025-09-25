using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 樱桃炸弹类（继承自植物基类，实现范围爆炸伤害功能）
public class CherryBomb : Plant {
    public Collider2D boomBox; // 爆炸范围碰撞体（用于检测爆炸范围内的僵尸）

    private bool isPlayClip = false; // 爆炸前音效播放标记（避免重复播放）

    // 植物专属功能（触发爆炸，由种植后的动画事件调用）
    public override void PlantFun() {
        StartCoroutine(Boom()); // 启动爆炸逻辑协程
    }

    // 启用状态更新逻辑（仅用于播放爆炸前的准备音效）
    protected override void EnableUpdate() {
        if (isPlayClip == true) return; // 音效已播放则直接返回
        isPlayClip = true; // 标记音效已播放

        AudioManager.Instance.PlayClip(Config.reverseBoom); // 播放爆炸前的准备音效
    }

    // 爆炸核心逻辑协程（音效、特效、范围伤害、自身销毁）
    IEnumerator Boom() {
        AudioManager.Instance.PlayClip(Config.cherryBombBoom); // 播放爆炸音效
        // 播放樱桃炸弹爆炸特效
        ObjectPoolManager.Instance.PlayCherryBombBoomParticalIEnumrator(transform);

        // 检测爆炸范围内的所有僵尸（仅检测"Zombie"层）
        Bounds bounds = boomBox.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,       // 爆炸范围中心（碰撞体中心点）
            bounds.size,         // 爆炸范围大小（碰撞体尺寸）
            boomBox.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
            LayerMask.GetMask("Zombie") // 目标检测层：僵尸层
        );

        // 调整自身渲染层级到"Front"（确保爆炸效果显示在最上层，不被遮挡）
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = "Front";
        }

        yield return new WaitForSeconds(0.05f); // 等待极短时间，确保特效与伤害判定同步

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
                // 尝试获取碰撞体上的Zombie组件
                if (coll.TryGetComponent<Zombie>(out var zombie)) {
                    // 播放僵尸被炸飞的特效（传入僵尸位置、是否有头、特效层级）
                    ObjectPoolManager.Instance.PlayZombieBoomSwfIEnumrator(zombie.transform, zombie.isHaveHead, zombie.spriteList[0].sortingOrder + 100);
                    zombie.Dead(); // 直接杀死僵尸
                }
            }
        }

        yield return new WaitForSeconds(0.5f); // 等待爆炸特效播放完成
        Dead(); // 销毁樱桃炸弹自身
    }
}