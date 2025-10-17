using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 樱桃炸弹类（继承自植物基类，实现范围爆炸伤害功能）
public class IceShroom : Plant {

    private bool isPlayCrozeSource = false;

    // 植物专属功能（触发爆炸，由种植后的动画事件调用）
    public override void PlantFun() {
        StartCoroutine(Boom()); // 启动爆炸逻辑协程
    }

    // 爆炸核心逻辑协程（音效、特效、范围伤害、自身销毁）
    IEnumerator Boom() {

        UIManager.Instance.screenFlashUI.PlayFlash(0);
        ObjectPoolManager.Instance.PlayIceShroomBoomParticalIEnumrator(transform);

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("Zombie");
 
        // 调整自身渲染层级到"Front"（确保爆炸效果显示在最上层，不被遮挡）
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = "Front";
        }

        yield return new WaitForSeconds(0.05f); // 等待极短时间，确保特效与伤害判定同步

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var obj in gameObjects) {
            if (obj != null) { // 避免空引用异常
                // 尝试获取碰撞体上的Zombie组件
                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null) {
                    if (obj.TryGetComponent<Zombie>(out var zombie)) {
                        if (zombie.IsCroze() == false) isPlayCrozeSource = true;

                        zombie.PlayCrozeSpeed();
                        zombie.TakeDamage(25f);
                    }
                    else {
                        enemy.TakeDamage(25f);
                    }
                }
            }
        }

        if (isPlayCrozeSource) AudioManager.Instance.PlayClip(Config.Frozen);

        Dead(); // 销毁樱桃炸弹自身
    }
}