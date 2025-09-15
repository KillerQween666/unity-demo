using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小车管理器，负责小车列表的显示控制
public class CarManager : MonoBehaviour {

    // 单例实例，全局唯一访问点
    public static CarManager instance { get; private set; }

    // 存储小车的数组（在Inspector中赋值）
    public Car[] carList;

    // 初始化单例
    private void Awake() {
        instance = this;
    }

    // 显示小车列表：通过DoTween动画将所有小车移动到指定X位置
    public void ShowCarList() {
        foreach (Car car in carList) {
            // 小车局部X轴移动到-0.2f，动画时长0.5秒
            car.transform.DOLocalMoveX(-0.2f, 0.5f);
        }
    }
}