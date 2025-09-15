using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 游戏结束总界面（胜利/失败后显示）
public class GameEndUI : MonoBehaviour {
    public GameObject gameEndImage; // 结束界面图片
    public bool isGameEnd = false;  // 游戏是否结束的标记

    private void Start() {
        Hide(); // 初始隐藏
    }

    // 显示结束界面
    public void Show() {
        isGameEnd = true;
        AudioManager.Instance.StopBgm(); // 停止背景音乐
        gameEndImage.SetActive(true);
    }

    // 隐藏结束界面
    public void Hide() {
        gameEndImage.SetActive(false);
    }
}