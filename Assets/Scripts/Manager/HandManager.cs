using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 手部管理器，负责植物跟随鼠标、种植植物、铲子操作等核心交互逻辑
public class HandManager : MonoBehaviour {

    // 单例实例，全局唯一访问点
    public static HandManager Instance { get; private set; }

    // 存储植物类型与对应预制体的映射（从Resources加载，快速获取预制体）
    private Dictionary<PlantType, Plant> plantPrefabs = new Dictionary<PlantType, Plant>();

    // 当前选中的植物（跟随鼠标，未种植）
    public Plant currentPlant;

    // 当前选中植物对应的卡牌（用于后续冷却等操作）
    private Card card;

    // 当前选中的铲子（跟随鼠标，未使用）
    public GameObject shovel;

    // 初始化单例，收集所有植物预制体
    private void Awake() {
        Instance = this;
        CollectAllPlantPrefabs();
    }

    // 每帧更新：让选中的植物/铲子跟随鼠标
    private void Update() {
        FollowCursor();
    }

    // 从Resources/Plants目录收集所有植物预制体，存入字典
    private void CollectAllPlantPrefabs() {
        Plant[] plants = Resources.LoadAll<Plant>("Plants"); // 加载指定路径下所有Plant类型预制体
        foreach (var plant in plants) {
            // 避免重复添加同一类型的植物预制体
            if (!plantPrefabs.ContainsKey(plant.plantType)) {
                plantPrefabs.Add(plant.plantType, plant);
            }
        }
    }

    // 让当前选中的植物/铲子跟随鼠标位置
    void FollowCursor() {
        // 没有选中植物也没有选中铲子，直接返回
        if (currentPlant == null && !shovel.activeSelf) return;

        // 将鼠标屏幕坐标转换为世界坐标（Z轴设为0，避免与其他物体层级冲突）
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // 选中植物则让植物跟随鼠标
        if (currentPlant != null) {
            currentPlant.transform.position = mouseWorldPosition;
            //currentPlant.transform.position = mouseWorldPosition;
        }
        // 选中铲子则让铲子跟随鼠标
        else if (shovel.activeSelf) {
            shovel.transform.position = mouseWorldPosition;
        }
    }

    // 添加选中的植物（从卡牌点击触发，生成植物预制体并跟随鼠标）
    public void AddPlant(PlantType plantType) {
        // 已有选中的植物或铲子，不重复添加
        if (currentPlant != null || shovel.activeSelf) return;

        AudioManager.Instance.PlayClip(Config.selectPlant); // 播放选择植物的音效
        card = CardManager.Instance.GetCardByPlantType(plantType); // 获取对应植物的卡牌

        // 从字典中获取植物预制体，生成实例
        if (plantPrefabs.TryGetValue(plantType, out Plant prefab)) {
            currentPlant = Instantiate(prefab);

            // 调整植物所有渲染器的层级为"UI"，确保跟随鼠标时显示在最上层
            SpriteRenderer[] sprites = currentPlant.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = "UI";
            }

            // 初始设为禁用状态（未种植时不工作）
            currentPlant.TransitionToDisable();
        }
    }

    // 点击单元格时执行（种植植物或使用铲子移除植物）
    public void OnCellClick(Cell cell) {
        // 单元格没有植物时，种植当前选中的植物
        if (cell.currentPlant == null) {
            cell.AddPlant(); // 单元格添加植物

            // 种植成功后，处理卡牌冷却、扣阳光、销毁跟随鼠标的植物
            if (cell.currentPlant != null) {
                card.TransitionToCooling(); // 卡牌进入冷却状态
                SunManager.Instance.SubSun(card.needSunPoint); // 扣除种植所需阳光
                AudioManager.Instance.PlayClip(Config.plant); // 播放种植音效

                Destroy(currentPlant.gameObject); // 销毁跟随鼠标的植物实例
                currentPlant = null; // 清空当前选中植物
            }
        }

        // 有选中的铲子时，移除单元格中的植物
        if (shovel.activeSelf) {
            cell.SubPlant();
        }
    }

    // 点击铲子UI时执行（选中/取消选中铲子）
    public void OnShovelUIClick() {
        // 已有选中的植物，无法选中铲子
        if (currentPlant != null) return;

        // 没有选中铲子：显示铲子实例并跟随鼠标，隐藏铲子UI
        if (!shovel.activeSelf) {

            shovel.SetActive(true);
            AudioManager.Instance.PlayClip(Config.shovelClick); // 播放铲子选中音效
            UIManager.Instance.shovelUI.Hide();
        }
        // 已有选中的铲子：取消选中，隐藏铲子实例，显示铲子UI
        else {
            ReturnShovel();
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2); // 播放取消音效
        }
    }

    // 归还铲子（取消选中，恢复初始状态）
    public void ReturnShovel() {
        shovel.SetActive(false); // 隐藏铲子实例
        UIManager.Instance.shovelUI.Show(); // 重新显示铲子UI
    }
}