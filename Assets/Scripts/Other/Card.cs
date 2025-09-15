using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 卡牌状态枚举：禁用、冷却中、等待阳光、准备就绪
public enum CardState {
    Disable,
    Cooling,
    WaitingSun,
    Ready
}

// 植物类型枚举：向日葵、豌豆射手
public enum PlantType {
    sunFlower,
    PeaShooter
}

// 植物卡牌脚本，控制卡牌状态（冷却、阳光判断）、UI显示及点击交互
public class Card : MonoBehaviour {

    // 当前卡牌状态（默认禁用）
    public CardState cardState = CardState.Disable;
    // 卡牌对应的植物类型
    public PlantType plantType = PlantType.sunFlower;

    // 卡牌UI元素：准备就绪时的高亮光效
    public GameObject cardLight;
    // 卡牌UI元素：未就绪/冷却时的灰色遮罩
    public GameObject cardGray;
    // 卡牌UI元素：冷却时的进度遮罩（显示冷却剩余时间）
    public Image cardMask;

    // 卡牌冷却时间（在Inspector赋值，单位：秒）
    [SerializeField]
    private float cdTime = 2;
    // 冷却计时器
    private float cdTimer = 0;

    // 种植对应植物所需的阳光数量（在Inspector赋值）
    [SerializeField]
    public int needSunPoint = 50;

    // 每帧更新：根据当前卡牌状态执行对应逻辑
    private void Update() {
        switch (cardState) {
            case CardState.Disable: // 禁用状态：无操作
                break;
            case CardState.Cooling: // 冷却状态：更新冷却进度
                CoolingUpdate();
                break;
            case CardState.WaitingSun: // 等待阳光状态：判断是否有足够阳光
                WaitingSunUpdate();
                break;
            case CardState.Ready: // 准备就绪状态：判断阳光是否足够（防止阳光不足时误操作）
                ReadyUpdate();
                break;
            default:
                break;
        }
    }

    // 冷却状态更新：计算冷却进度，冷却结束后切换到等待阳光状态
    void CoolingUpdate() {
        cdTimer += Time.deltaTime;
        // 更新冷却遮罩的填充比例（剩余冷却时间/总冷却时间）
        cardMask.fillAmount = (cdTime - cdTimer) / cdTime;

        // 冷却时间到，切换到等待阳光状态
        if (cdTimer > cdTime) {
            TransitionToWaitingSun();
        }
    }

    // 等待阳光状态更新：有足够阳光时，切换到准备就绪状态
    void WaitingSunUpdate() {
        if (needSunPoint <= SunManager.Instance.sunPoint) {
            TransitionToReady();
        }
    }

    // 准备就绪状态更新：阳光不足时，切回等待阳光状态
    void ReadyUpdate() {
        if (needSunPoint > SunManager.Instance.sunPoint) {
            TransitionToWaitingSun();
        }
    }

    // 切换到冷却状态（种植植物后调用）
    public void TransitionToCooling() {
        cardState = CardState.Cooling;

        cdTimer = 0; // 重置冷却计时器
        cardLight.SetActive(false); // 关闭高亮光效
        cardGray.SetActive(true); // 显示灰色遮罩
        cardMask.gameObject.SetActive(true); // 显示冷却进度遮罩
    }

    // 切换到等待阳光状态（冷却结束/阳光不足时调用）
    void TransitionToWaitingSun() {
        cardState = CardState.WaitingSun;

        cardLight.SetActive(false); // 关闭高亮光效
        cardGray.SetActive(true); // 显示灰色遮罩
        cardMask.gameObject.SetActive(false); // 隐藏冷却进度遮罩
    }

    // 切换到准备就绪状态（阳光足够时调用）
    public void TransitionToReady() {
        cardState = CardState.Ready;

        cardLight.SetActive(true); // 显示高亮光效
        cardGray.SetActive(false); // 隐藏灰色遮罩
        cardMask.gameObject.SetActive(false); // 隐藏冷却进度遮罩
    }

    // 卡牌点击事件（选中卡牌，让植物跟随鼠标）
    public void OnClick() {
        HandManager.Instance.AddPlant(plantType);
    }

    // 鼠标抬起事件（判断是否点击在单元格上，触发种植逻辑）
    public void OnPointerUp() { 
        // 将鼠标屏幕坐标转换为世界坐标（Z轴设为0，避免层级冲突）
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // 射线检测鼠标位置的碰撞体
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        // 点击到单元格时，调用单元格的点击逻辑（种植植物）
        if (hit) {
            if (hit.collider.CompareTag("Cell")) {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.OnClick();
            }
        }   
    }
}