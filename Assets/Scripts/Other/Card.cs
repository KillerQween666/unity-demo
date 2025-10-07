using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 卡牌状态枚举：定义卡牌的不同功能状态
public enum CardState {
    Disable,       // 禁用（无法交互）
    select,        // 选中（用于卡槽切换等场景）
    Cooling,       // 冷却中（种植后等待冷却）
    WaitingSun,    // 等待阳光（阳光不足，无法种植）
    Ready          // 准备就绪（阳光充足且冷却完成，可种植）
}

// 植物类型枚举：对应卡牌可种植的植物种类
public enum PlantType {
    sunFlower,        // 向日葵
    PeaShooter,       // 豌豆射手
    TwoPeaShooter,    // 双发射手
    SnowPeaShooter,   // 寒冰射手
    Wallnut,          // 坚果墙
    PotatoMine,       // 土豆地雷
    CherryBomb,       // 樱桃炸弹
    Chomper,           // 食人花
    SunShroom,
    PuffShroom,
    FumeShroom,
    ScaredyShroom,
    IceShroom,
    DoomShroom,
    HypnoShroom,
    Gravebuster,
}

// 植物卡牌核心脚本：控制卡牌状态、UI显示、点击种植及冷却逻辑
public class Card : MonoBehaviour {

    private CardState cardState = CardState.Disable; // 当前卡牌状态（默认禁用）
    public CardState defaultState = CardState.Disable; // 卡牌初始默认状态
    public PlantType plantType = PlantType.sunFlower; // 该卡牌对应的植物类型

    // 卡牌UI元素
    public GameObject cardLight; // 准备就绪时的高亮提示光效
    public GameObject cardGray;  // 未就绪/冷却时的灰色遮罩（表示不可用）
    public Image cardMask;       // 冷却时的进度遮罩（直观显示剩余冷却时间）

    [SerializeField]
    private float cdTime = 2;    // 卡牌冷却时间（在Inspector手动设置，单位：秒）
    private float cdTimer = 0;   // 冷却计时器（记录已冷却时长）

    [SerializeField]
    public int needSunPoint = 50; // 种植对应植物所需的阳光数量（Inspector设置）

    private bool isInSlot = false; // 标记卡牌是否在卡槽中
    public Transform originParent; // 卡牌的原始父物体（用于复位）
    public Vector3 originPosition; // 卡牌的原始位置（用于复位）

    private void Awake() {
        // 初始化：记录卡牌的原始父物体和位置（后续复位用）
        originParent = transform.parent;
        originPosition = GetComponent<RectTransform>().anchoredPosition;
    }

    // 每帧更新：根据当前状态执行对应逻辑（状态驱动行为）
    private void Update() {
        switch (cardState) {
            case CardState.Disable: // 禁用状态：不执行任何逻辑
                break;
            case CardState.Cooling: // 冷却状态：实时更新冷却进度
                CoolingUpdate();
                break;
            case CardState.WaitingSun: // 等待阳光：检测阳光是否充足
                WaitingSunUpdate();
                break;
            case CardState.Ready: // 准备就绪：防止阳光突然不足导致的状态异常
                ReadyUpdate();
                break;
            default:
                break;
        }
    }

    // 冷却状态更新逻辑
    void CoolingUpdate() {
        cdTimer += Time.deltaTime; // 累加冷却时间
        // 更新冷却遮罩填充比例（反向显示：剩余冷却时间/总冷却时间）
        cardMask.fillAmount = (cdTime - cdTimer) / cdTime;

        // 冷却结束：切换到等待阳光状态（需再次判断阳光是否足够）
        if (cdTimer > cdTime) {
            TransitionToWaitingSun();
        }
    }

    // 等待阳光状态更新逻辑
    void WaitingSunUpdate() {
        // 阳光充足时，切换到准备就绪状态
        if (needSunPoint <= SunManager.Instance.sunPoint) {
            TransitionToReady();
        }
    }

    // 准备就绪状态更新逻辑
    void ReadyUpdate() {
        // 阳光不足时，切回等待阳光状态
        if (needSunPoint > SunManager.Instance.sunPoint) {
            TransitionToWaitingSun();
        }
    }

    // 切换到冷却状态（种植植物后调用）
    public void TransitionToCooling() {
        cardState = CardState.Cooling;

        cdTimer = 0; // 重置冷却计时器
        cardLight.SetActive(false); // 关闭高亮
        cardGray.SetActive(true);   // 显示灰色遮罩
        cardMask.gameObject.SetActive(true); // 显示冷却进度
    }

    // 切换到等待阳光状态（冷却结束或阳光不足时触发）
    void TransitionToWaitingSun() {
        cardState = CardState.WaitingSun;

        cardLight.SetActive(false); // 关闭高亮
        cardGray.SetActive(true);   // 显示灰色遮罩
        cardMask.gameObject.SetActive(false); // 隐藏冷却进度
    }

    // 切换到准备就绪状态（阳光充足时触发）
    public void TransitionToReady() {
        cardState = CardState.Ready;

        cardLight.SetActive(true);  // 显示高亮（提示可点击）
        cardGray.SetActive(false);  // 隐藏灰色遮罩
        cardMask.gameObject.SetActive(false); // 隐藏冷却进度
    }

    public void TransitionToPlanting() {
        cardMask.gameObject.SetActive(true);
        cardGray.SetActive(true);
    }

    public void CancelPlant() {
        cardMask.gameObject.SetActive(false);
        cardGray.SetActive(false);
    }

    // 卡牌点击事件（UI按钮绑定）
    public void OnClick() {
        if (cardState == CardState.Ready) {
            // 准备就绪时：选中卡牌，让植物跟随鼠标
            TransitionToPlanting();
            HandManager.Instance.AddPlant(plantType);
        } else if (cardState == CardState.select) {
            // 选中状态时：切换卡牌在卡槽/卡池的位置
            if (isInSlot) {
                isInSlot = false;
                CardSlotManager.Instance.MoveToPool(this); // 移回卡池
            }
            else {
                isInSlot = true;
                CardSlotManager.Instance.MoveToSlot(this); // 移到卡槽
            }
        }
    }

    // 鼠标抬起事件（检测是否种植植物）
    public void OnPointerUp() {
        if (cardState != CardState.Ready) return;

        // 将鼠标屏幕坐标转换为2D世界坐标（Z轴设0，避免层级偏差）
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // 射线检测：判断鼠标是否点击在可种植的单元格上
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("OnClick"));

        if (hit) {
            if (hit.collider.CompareTag("Cell")) {
                // 点击单元格：触发单元格的种植逻辑
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.OnClick();
            }
            else if (hit.collider.CompareTag("CardList")) {
                // 点击卡池：取消卡牌选中状态，恢复UI显示
                CardListUI cardListUI = hit.collider.GetComponent<CardListUI>();
                cardListUI.OnClick();
            }
        }
    }

    // 切换到禁用状态
    public void TransitionToDisable() {
        cardState = CardState.Disable;
    }

    // 切换到默认状态（初始化或重置时调用）
    public void TransitionToDefaultState() {
        if (defaultState == CardState.Cooling) {
            TransitionToCooling();
        }
        else if (defaultState == CardState.Ready) {
            TransitionToReady();
        }
    }

    // 切换到选中状态（用于卡槽管理）
    public void TransitionToSelectState() {
        cardState = CardState.select;
    }
}