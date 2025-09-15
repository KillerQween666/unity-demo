using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 结束界面UI（如最终波提示）
public class EndUI : MonoBehaviour {

    private Animator animator; // 控制界面动画

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        Hide(); // 初始隐藏
    }

    // 隐藏界面（禁用动画组件）
    public void Hide() {
        animator.enabled = false;
    }

    // 显示界面（启用动画组件）
    public void Show() {
        animator.enabled = true;
    }

    // 播放"巨浪"音效（动画事件调用）
    public void PlayHugeWaveAudio() {
        AudioManager.Instance.PlayClip(Config.hugewave);
    }

    // 播放"最终波"音效（动画事件调用）
    public void PlayFianlWaveAudio() {
        AudioManager.Instance.PlayClip(Config.finalwave);
    }
}