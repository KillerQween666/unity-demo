using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 准备界面UI（游戏开始前的倒计时等）
public class PrepareUI : MonoBehaviour {

    private Animator animator; // 控制准备动画

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // 初始隐藏
    }

    // 隐藏准备界面
    public void Hide() {
        animator.enabled = false;
    }

    // 显示准备界面
    public void Show() {
        animator.enabled = true;
    }

    // 准备完成，通知游戏管理器开始游戏（动画事件调用）
    public void GameStart() {
        GameManager.Instance.OnPrepareUIComplete();
    }
}