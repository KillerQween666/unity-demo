using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 结束界面UI（如最终波提示）
public class HugeWaveUI : MonoBehaviour {

    private Animator animator; // 控制界面动画

    private int waveIndex;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    // 显示界面（启用动画组件）
    public void Show(int index) {
        waveIndex = index;
        animator.SetTrigger("isTrigger");
    }

    // 播放"巨浪"音效（动画事件调用）
    public void PlayHugeWaveAudio() {
        AudioManager.Instance.PlayClip(Config.hugewave);
    }

    // 播放"一大波"音效（动画事件调用）
    public void PlayWaveSirenAudio() {
        AudioManager.Instance.PlayClip(Config.waveSiren);
        if (waveIndex == 1) {
            ZombieManager.Instance.HugeWaveSpawnLast1();
        } else if (waveIndex == 2) {
            ZombieManager.Instance.HugeWaveSpawnLast2();
        }
        
    }
}