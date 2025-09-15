using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 胜利界面UI
public class WinUI : MonoBehaviour {

    private Animator animator; // 控制胜利动画

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // 初始隐藏
    }

    // 隐藏胜利界面
    public void Hide() {
        animator.enabled = false;
    }

    // 显示胜利界面
    public void Show() {
        animator.enabled = true;
    }

    // 显示游戏结束总界面（动画事件调用）
    public void ShowGameEndUI() {
        GameManager.Instance.PauseGame();
        UIManager.Instance.gameEndUI.Show();
    }
}