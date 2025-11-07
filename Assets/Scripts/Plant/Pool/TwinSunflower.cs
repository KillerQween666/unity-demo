using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwinSunflower : Plant {
    // 产生阳光的间隔时间（秒）
    public float produceDuration = 5;
    // 产生阳光的计时器
    private float produceTimer;

    // 阳光跳跃的最小/最大距离（左右随机）
    public float jumpMinDistance = 0.6f;
    public float jumpMaxDistance = 2f;

    // 启用状态逻辑：计时，到时间触发发光动画（动画事件调用ProduceSun）
    protected override void EnableUpdate() {
        produceTimer += Time.deltaTime;

        if (produceTimer > produceDuration) {
            produceTimer = 0;
            StartCoroutine(ProduceSun());
        }
    }

    public IEnumerator ProduceSun() {
        // 渐变总时长（亮占1秒暗占0.5秒）
        float duration = 1f;
        // 当前计时器
        float timer = 0f;
        // 当前亮度（初始值为原始亮度）
        float currentBrighten = originalBright;

        // 第一阶段：渐渐变亮
        // 当计时器小于目标时长时，持续更新
        while (timer < duration) {
            // 累加每帧流逝的时间
            timer += Time.deltaTime;
            //处于高亮状态就不变化
            if (isBrighten == true) continue;
            // 计算进度（0到1之间）
            float progress = timer / duration;
            // 从原始亮度平滑过渡到闪烁亮度
            currentBrighten = Mathf.Lerp(originalBright, flashBright, progress);
            // 应用亮度到所有精灵
            spriteList.ForEach(s => s.material.SetFloat("_Brightness", currentBrighten));
            // 等待下一帧（关键：没有这句会瞬间完成）
            yield return null;
        }

        // 确保亮度准确达到目标值
        currentBrighten = flashBright;
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", currentBrighten));

        // 执行植物功能（假设这是一个中间要执行的方法）
        PlantFun();
        PlantFun();
        // 亮着停留0.15秒
        yield return new WaitForSeconds(0.5f);

        // 第二阶段：渐渐变暗
        // 重置计时器，重新开始计算
        timer = 0f;
        duration = 0.5f;
        while (timer < duration) {
            timer += Time.deltaTime;

            //处于高亮状态就不变化
            if (isBrighten == true) continue;

            float progress = timer / duration;

            // 从闪烁亮度平滑过渡回原始亮度
            currentBrighten = Mathf.Lerp(flashBright, originalBright, progress);
            spriteList.ForEach(s => s.material.SetFloat("_Brightness", currentBrighten));
            // 等待下一帧
            yield return null;
        }

        // 不是处于高亮状态的话确保最终亮度准确回到原始值
        if (isBrighten == false) {
            currentBrighten = originalBright;
            spriteList.ForEach(s => s.material.SetFloat("_Brightness", currentBrighten));
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
