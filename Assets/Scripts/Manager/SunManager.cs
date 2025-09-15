using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// 阳光管理器，负责阳光的生成、数量统计、UI更新及阳光增减逻辑
public class SunManager : MonoBehaviour {

    // 单例实例，全局唯一访问点（外部通过此操作阳光）
    public static SunManager Instance { get; private set; }

    // 当前拥有的阳光数量
    public int sunPoint;

    // 显示阳光数量的UI文本（TextMeshPro）
    public TextMeshProUGUI sunPointText;

    // 自动生成阳光的间隔时间（秒）
    public float produceTime;
    // 阳光生成计时器
    private float produceTimer;
    // 阳光预制体（从对象池获取，在Inspector赋值）
    public Sun sunPrefab;

    // 是否开始自动生成阳光的标记（由GameManager控制启动）
    private bool isStartProduce = false;

    // 初始化单例
    private void Awake() {
        Instance = this;
    }

    // 游戏启动时更新阳光UI（显示初始阳光数量）
    private void Start() {
        UpdateSunPointText();
    }

    // 每帧更新：如果开启自动生成，执行阳光生成逻辑
    private void Update() {
        if (isStartProduce) {
            ProduceSun();
        }
    }

    public void StartProduce() {
        isStartProduce = true;
    }

    // 更新阳光数量的UI文本（确保显示与实际数量一致）
    public void UpdateSunPointText() {
        sunPointText.text = sunPoint.ToString();
    }

    // 减少阳光数量（种植植物时调用）
    public void SubSun(int point) {
        sunPoint -= point;
        UpdateSunPointText(); // 减完后立即更新UI
    }

    // 增加阳光数量（收集阳光时调用）
    public void AddSun(int point) {
        sunPoint += point;
        UpdateSunPointText(); // 加完后立即更新UI
    }

    // 自动生成阳光的核心逻辑（到时间生成阳光并让其移动到目标位置）
    void ProduceSun() {
        produceTimer += Time.deltaTime;
        // 计时器达到间隔时间，生成阳光
        if (produceTimer > produceTime) {
            produceTimer = 0; // 重置计时器

            // 随机生成阳光的初始位置（X：-5到7，Y：6，Z：-3，确保在屏幕上方）
            Vector3 position = new Vector3(Random.Range(-5, 7f), 6f, -3);
            // 从对象池获取阳光对象
            GameObject obj = ObjectPoolManager.Instance.GetSun();
            obj.transform.position = position;

            // 随机阳光的目标落点（Y：-4到3，让阳光落到地面附近）
            position.y = Random.Range(-4, 3f);
            // 让阳光线性移动到目标位置
            obj.GetComponent<Sun>().LinearTo(position);
        }
    }
}