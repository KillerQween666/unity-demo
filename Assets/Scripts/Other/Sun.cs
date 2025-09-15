using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 阳光脚本，控制阳光的移动（线性/跳跃）、生命周期、点击收集及对象池回收
public class Sun : MonoBehaviour {

    // 阳光移动到目标位置的默认时长（点击收集时使用）
    public float moveDuration = 1.5f;
    // 收集阳光可获得的阳光点数
    public int point = 50;

    // 阳光最大存活时间（超时未收集则自动回收）
    private float liveTime = 5;
    // 阳光存活计时器（记录已存活时间）
    public float liveTimer;

    // 阳光是否已被点击收集的标记（防止重复收集）
    public bool isClick = false;

    // 阳光的碰撞体（用于检测点击，在Awake中自动获取）
    public Collider2D sunCollider2D;

    // 初始化：获取阳光的碰撞体组件
    private void Awake() {
        sunCollider2D = GetComponent<Collider2D>();
    }

    // 每帧更新：检查阳光存活时间，超时未收集则回收到对象池
    private void Update() {
        liveTimer += Time.deltaTime;
        // 存活时间超过上限且未被收集，终止当前移动动画并回收
        if (liveTimer > liveTime && isClick == false) {
            transform.DOKill(); // 终止DOTween移动动画，避免残留
            ObjectPoolManager.Instance.ReleaseSun(this.gameObject);
        }
    }

    // 阳光线性移动到目标位置（如自动生成的阳光落到地面）
    public void LinearTo(Vector3 targetPos) {
        // 计算移动距离和速度，确保不同距离下移动速度一致
        float distance = Vector3.Distance(transform.position, targetPos);
        float moveSpeed = 1.5f; // 固定移动速度
        float actualMoveDuration = distance / moveSpeed; // 实际移动时长（距离越远时间越长）

        // 执行线性移动动画
        transform.DOMove(targetPos, actualMoveDuration);
    }

    // 阳光跳跃式移动到目标位置（带弧形路径，如向日葵生成的阳光）
    public void JumpTo(Vector3 targetPos) {
        // 计算路径中间点（弧形顶点，Y轴随机抬高，模拟跳跃轨迹）
        Vector3 centerPos = (transform.position + targetPos) / 2; // 路径中点
        centerPos.y += Random.Range(0.3f, 0.7f); // 随机抬高Y轴，让路径更自然

        // 定义跳跃路径的三个点：起点 → 弧形顶点 → 终点
        Vector3[] pathPoints = new Vector3[] { transform.position, centerPos, targetPos };

        // 计算路径总长度，确保移动速度一致
        float pathLength = 0;
        float moveSpeed = 0.75f; // 跳跃移动速度（比线性慢，更符合视觉预期）
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            pathLength += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        float totalDuration = pathLength / moveSpeed; // 跳跃总时长

        // 执行弧形路径移动（CatmullRom类型让路径更平滑，OutQuad曲线让结尾减速）
        transform.DOPath(pathPoints, totalDuration, PathType.CatmullRom)
            .SetEase(Ease.OutQuad);
    }

    // 阳光点击收集事件（玩家点击阳光时触发）
    public void OnClick() {
        if (isClick == true) return; // 已收集过，避免重复执行
        isClick = true; // 标记为已收集

        AudioManager.Instance.PlayClip(Config.sunClick); // 播放收集阳光的音效
        sunCollider2D.enabled = false; // 禁用碰撞体，防止再次被点击

        transform.DOKill(); // 终止当前移动动画（无论之前是线性还是跳跃）

        // 阳光向阳光计数器位置移动（固定目标位置：(-5.62f, 4.31f, 0)）
        transform.DOMove(new Vector3(-5.62f, 4.31f, 0), moveDuration)
            .SetUpdate(true) // 即使游戏暂停，动画也继续（确保收集反馈正常）
            .SetEase(Ease.Linear) // 线性移动，快速到达目标
            .OnComplete( // 移动完成后，将阳光回收到对象池
            () => {
                ObjectPoolManager.Instance.ReleaseSun(this.gameObject);
            });

        SunManager.Instance.AddSun(point); // 增加玩家的阳光点数
    }
}