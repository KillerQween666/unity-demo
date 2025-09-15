using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 失败界面UI
public class FailUI : MonoBehaviour {

    private Animator animator; // 控制失败动画

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // 初始隐藏
    }

    // 隐藏失败界面
    public void Hide() {
        animator.enabled = false;
    }

    // 显示失败界面
    public void Show() {
        animator.enabled = true;
    }

    // 显示游戏结束总界面（动画事件调用）
    public void ShowGameEndUI() {
        GameManager.Instance.PauseGame();
        UIManager.Instance.gameEndUI.Show();
    }
}