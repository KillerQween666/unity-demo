using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 铲子UI（用于移除植物）
public class ShovelUI : MonoBehaviour {

    public Image shovelImage; // 铲子图标

    private void Start() {
        Show(); // 初始显示
    }

    // 显示铲子图标
    public void Show() {
        shovelImage.enabled = true;
    }

    // 隐藏铲子图标
    public void Hide() {
        shovelImage.enabled = false;
    }

    // 鼠标抬起时：移除点击位置的植物（UI事件调用）
    public void OnPointerUp() {
        // 转换鼠标位置到世界坐标
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // 射线检测点击的单元格
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        if (hit && hit.collider.CompareTag("Cell")) {
            // 调用单元格的移除植物方法
            Cell cell = hit.collider.GetComponent<Cell>();
            cell.SubPlant();
        }
    }
}