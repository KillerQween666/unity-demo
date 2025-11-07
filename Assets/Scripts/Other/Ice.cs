using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ice : MonoBehaviour {
    public float iceLiveTime = 10;
    private float iceLiveTimer = 0;

    private Collider2D collBox;

    private List<Cell> cellList = new List<Cell>();

    private void Start() {
        collBox = GetComponent<Collider2D>();

        Bounds bounds = collBox.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,       // 爆炸范围中心（碰撞体中心点）
            bounds.size,         // 爆炸范围大小（碰撞体尺寸）
            collBox.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
            LayerMask.GetMask("OnClick") // 目标检测层：僵尸层
        );

        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
               if (coll.TryGetComponent<Cell>(out var cell)) {
                    cell.StartIce();
                    cellList.Add(cell);
                }
            }
        }
    }

    private void Update() {
        iceLiveTimer += Time.deltaTime;
        if (iceLiveTimer > iceLiveTime) {
            Dead();
        }
    }

    public void FireDead() {
        foreach (var cell in cellList) {
            cell.EndIce();
        }
        Dead();
    }

    public void Dead() {
        Destroy(gameObject);
    }
}
