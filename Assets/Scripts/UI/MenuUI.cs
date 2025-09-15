using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 菜单界面UI（暂停、继续等功能）
public class MenuUI : MonoBehaviour {
    public GameObject menuImage;  // 菜单界面图片
    public GameObject menuButton; // 菜单按钮
    public bool isHide = true;    // 菜单是否隐藏的标记

    // 隐藏菜单按钮
    public void ButtonHide() {
        menuButton.SetActive(false);
    }

    // 显示菜单按钮
    public void ButtonShow() {
        menuButton.SetActive(true);
    }

    // 显示菜单（暂停游戏）
    public void Show() {
        if (menuImage.activeSelf) return; // 已显示则退出

        AudioManager.Instance.PlayClip(Config.buttonClick);
        AudioManager.Instance.StopBgm(); // 停止背景音乐
        GameManager.Instance.PauseGame(); // 暂停游戏
        menuImage.SetActive(true);
    }

    // 隐藏菜单（恢复游戏）
    public void Hide() {
        if (!menuImage.activeSelf) return; // 已隐藏则退出

        AudioManager.Instance.PlayClip(Config.buttonClick);
        AudioManager.Instance.PlayBgm(); // 恢复背景音乐
        GameManager.Instance.ResumeGame(); // 恢复游戏
        menuImage.SetActive(false);
    }
}