using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {
    public void LoadTargetLevel(string targetSceneName) {
        Time.timeScale = 1; // 时间缩放恢复为1，游戏正常运行
        DOTween.KillAll(); // 终止所有DOTween动画（清除残留动画）

        SceneManager.LoadScene(targetSceneName);
    }
}
