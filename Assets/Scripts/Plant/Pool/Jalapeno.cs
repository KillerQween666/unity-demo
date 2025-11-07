using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jalapeno : Plant {
    public Collider2D boomBox; // 爆炸范围碰撞体（用于检测爆炸范围内的僵尸）

    private bool isPlayClip = false; // 爆炸前音效播放标记（避免重复播放）

    public float minX = -10f;
    public float maxX = 10f;
    public float interval = 2f;

    public GameObject jalapeno;

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
        AudioManager.Instance.PlayClip(Config.jalapeno); // 播放爆炸音效

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

        StartCoroutine(PlayBoomSwf());

        yield return new WaitForSeconds(0.05f); // 等待极短时间，确保特效与伤害判定同步

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
                // 尝试获取碰撞体上的Zombie组件
                if (coll.TryGetComponent<Enemy>(out var enemy)) {
                    enemy = coll.GetComponent<Enemy>();
                    if (enemy != null) {
                        if (coll.TryGetComponent<Zombie>(out var zombie)) {
                            if (coll.TryGetComponent<ZombieZamboni>(out var zombieZamboni)) {
                                zombieZamboni.IceDead();
                            }
                            // 播放僵尸被炸飞的特效（传入僵尸位置、是否有头、特效层级）
                            zombie.TakeDamage(600, 1);
                        }
                        else {
                            enemy.TakeDamage(600, 1);
                        }
                    }
                }

                if (coll.TryGetComponent<Ice>(out var ice)) {
                    ice.FireDead();
                }

            }
        }

        jalapeno.SetActive(false);
        shadow.SetActive(false);

        yield return new WaitForSeconds(1f);
        Dead(); // 销毁樱桃炸弹自身
    }

    public IEnumerator PlayBoomSwf() {

        // 计算X轴上的所有点
        List<Vector3> points = GeneratePoints();
        List<GameObject> objList = new List<GameObject>();

        // 在每个点实例化动画并播放
        foreach (Vector3 point in points) {
            GameObject obj = ObjectPoolManager.Instance.GetFirePeaBulletSwf(); // 从池获取动画对象
            obj.transform.position = point; // 设置动画播放位置

            objList.Add(obj);

            // 获取动画控制器和渲染组件
            SwfClipController swfClipController = obj.GetComponentInChildren<SwfClipController>();

            swfClipController.Play(true);
        }

        yield return new WaitForSeconds(0.8f);

        foreach (var obj in objList) {
            ObjectPoolManager.Instance.ReleaseFirePeaBulletSwf(obj);
        }
    }

    private List<Vector3> GeneratePoints() {
        List<Vector3> points = new List<Vector3>();
        float currentX = minX;

        // 从minX到maxX，每隔interval生成一个点
        while (currentX <= maxX + 0.01f) // 加0.01f避免浮点误差
        {
            // Y轴使用当前父物体的Y值，Z轴默认0（可根据需求修改）
            Vector3 point = new Vector3(currentX, transform.position.y + 0.5f, 0);
            points.Add(point);
            currentX += interval;
        }

        return points;
    }
}
