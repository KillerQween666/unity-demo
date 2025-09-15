using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFlower : Plant {
    // 产生阳光的间隔时间（秒）
    public float produceDuration = 5;
    // 产生阳光的计时器
    private float produceTimer;

    // 阳光预制体（从对象池获取，无需手动实例化）
    public Sun sunPrefab;

    // 阳光跳跃的最小/最大距离（左右随机）
    public float jumpMinDistance = 0.6f;
    public float jumpMaxDistance = 2f;

    // 启用状态逻辑：计时，到时间触发发光动画（动画事件调用ProduceSun）
    protected override void EnableUpdate() {
        produceTimer += Time.deltaTime;

        if (produceTimer > produceDuration) {
            produceTimer = 0;
            animator.SetTrigger("glowTrigger");
        }
    }

    // 产生阳光（由发光动画的事件调用）
    public override void PlantFun() {
        // 阳光初始位置（向日葵上方，调整Z轴避免遮挡）
        Vector3 position = transform.position;
        position.z = -4;

        // 从对象池获取阳光并设置初始位置
        GameObject obj = ObjectPoolManager.Instance.GetSun();
        obj.transform.position = position;

        // 随机阳光跳跃的目标位置（50%向左，50%向右）
        float distance = Random.Range(jumpMinDistance, jumpMaxDistance);
        distance = Random.Range(0, 2) < 1 ? -distance : distance;
        position.x += distance;

        // 让阳光跳向目标位置
        obj.GetComponent<Sun>().JumpTo(position);
    }

}