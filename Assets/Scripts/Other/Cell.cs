using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 单元格脚本，负责植物种植、铲子移除植物，以及鼠标hover时的预览效果
public class Cell : MonoBehaviour {

    // 当前单元格上种植的植物（null表示无植物）
    public Plant currentPlant;
    // 鼠标hover时的植物预览实例（仅用于显示，不实际生效）
    private Plant plantPreview;

    // 单元格点击事件（种植植物/使用铲子，由HandManager处理）
    public void OnClick() {
        // 先销毁可能存在的预览植物，避免残留
        if (plantPreview != null) Destroy(plantPreview.gameObject);
        // 通知HandManager处理点击逻辑（种植或铲除）
        HandManager.Instance.OnCellClick(this);
    }

    // 鼠标进入单元格时触发（显示植物预览/高亮已有植物）
    public void OnPointerEnter(BaseEventData data) {
        // 若单元格有植物且选中了铲子，让植物高亮（提示可铲除）
        if (currentPlant != null && HandManager.Instance.shovel.activeSelf) {
            currentPlant.animator.SetBool("isBright", true);
        }

        // 若没有选中植物，或单元格已有植物，不显示预览
        if (HandManager.Instance.currentPlant == null || currentPlant != null) return;

        // 生成植物预览实例（复制当前选中的植物）
        plantPreview = Instantiate(HandManager.Instance.currentPlant);
        plantPreview.GetComponent<Collider2D>().enabled = false; // 禁用碰撞体，避免影响交互

        // 调整预览植物的显示效果：半透明（alpha 0.6）、层级降低（避免遮挡其他UI）
        SpriteRenderer[] sprites = plantPreview.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.color = new Color(1, 1, 1, 0.6f);
            sprite.sortingOrder -= 100;
        }

        // 让预览植物跟随单元格位置
        plantPreview.transform.position = transform.position;
    }

    // 鼠标离开单元格时触发（销毁预览/取消植物高亮）
    public void OnPointerExit(BaseEventData data) {
        // 若单元格有植物且选中了铲子，取消植物高亮
        if (currentPlant != null && HandManager.Instance.shovel.activeSelf) {
            currentPlant.animator.SetBool("isBright", false);
        }

        // 销毁预览植物，避免残留
        if (plantPreview != null) {
            Destroy(plantPreview.gameObject);
        }
    }

    // 移除单元格上的植物（铲子功能）
    public void SubPlant() {
        // 无植物或未选中铲子，不执行移除
        if (currentPlant == null || !HandManager.Instance.shovel.activeSelf) return;

        HandManager.Instance.ReturnShovel(); // 归还铲子（取消选中状态）
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.plant : Config.plant2); // 播放移除音效

        Destroy(currentPlant.gameObject); // 销毁植物实例
        currentPlant = null; // 清空当前植物引用
    }

    // 在单元格上种植植物（选中植物后点击单元格触发）
    public void AddPlant() {
        // 已有植物或未选中植物，不执行种植
        if (currentPlant != null || HandManager.Instance.currentPlant == null) return;

        // 调整植物种植位置（Z轴设为-2，确保显示在正确层级）
        Vector3 position = transform.position;
        position.z = -2;

        // 生成植物实例到单元格位置
        currentPlant = Instantiate(HandManager.Instance.currentPlant, position, Quaternion.identity);

        // 调整植物渲染层级为"Game"（与游戏场景其他物体一致）
        SpriteRenderer[] sprites = currentPlant.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = "Game";
        }

        // 激活植物（设置为启用状态，让植物开始工作）
        currentPlant.TransitionToEnable();
    }
}