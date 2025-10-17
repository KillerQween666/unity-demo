using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 游戏管理器：统筹游戏全流程（开始、暂停、结束、重置）、输入响应和相机动画
public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; } // 单例实例，供全局调用

    bool isGameEnd = false; // 游戏是否结束的标记（防止重复触发结束逻辑）
    public bool isPause = false;   // 游戏是否暂停的标记
    public bool isGameStart = false;

    // 初始化单例
    private void Awake() {
        Instance = this;
    }

    // 处理玩家输入（空格暂停/恢复、ESC控制菜单）
    private void Update() {
        // 游戏已结束时，不响应任何输入
        if (UIManager.Instance.gameEndUI.isGameEnd == true) return;

        // 空格键：切换暂停/恢复（菜单显示时不响应）
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (UIManager.Instance.menuUI.menuImage.activeSelf) return;

            if (isPause) {
                ResumeGame(); // 已暂停则恢复
            }
            else {
                AudioManager.Instance.PlayClip(Config.pause); // 播放暂停音效
                PauseGame(); // 未暂停则暂停
            }
        }

        // ESC键：切换菜单显示/隐藏
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (UIManager.Instance.menuUI.menuImage.activeSelf) {
                UIManager.Instance.menuUI.Hide(); // 隐藏菜单
            }
            else {
                UIManager.Instance.menuUI.Show(); // 显示菜单
            }
        }

        if (Input.GetMouseButtonDown(1)) {
            UIManager.Instance.cardListUI.OnClick();
        }
    }

    // 游戏启动时执行
    private void Start() {
        AudioManager.Instance.PlayBgm(Config.prepareBgm); // 播放初始背景音乐
        GameStart(); // 触发游戏开始流程
    }

    // 游戏开始核心逻辑：启动相机右移动画
    void GameStart() {
        StartCoroutine(CameraMoveRight());
    }

    // 相机右移协程（游戏开局的相机动画）
    IEnumerator CameraMoveRight() {
        yield return new WaitForSeconds(0.5f); // 延迟0.5秒再移动
        // 相机线性移动到目标位置，完成后触发后续逻辑
        Camera.main.transform.DOMove(new Vector3(4.5f, 0, -10), 1.5f).SetEase(Ease.Linear).OnComplete(OnCameraMoveRightComplete);
    }

    // 相机左移（切换到准备界面时调用）
    public void CameraMoveLeft() {
        isGameStart = true;
        UIManager.Instance.CardChooserUI.Hide(); // 隐藏卡牌选择界面
        // 相机线性左移，完成后触发后续逻辑
        Camera.main.transform.DOMove(new Vector3(0, 0, -10), 1.5f).SetEase(Ease.Linear).OnComplete(OnCameraMoveLeftComplete);
    }

    // 相机右移完成后执行：显示卡牌列表和菜单按钮
    void OnCameraMoveRightComplete() {
        LevelManager.Instance.level.OnCameraMoveRightComplete();
    }

    // 相机左移完成后执行：显示准备界面
    void OnCameraMoveLeftComplete() {
        AudioManager.Instance.PlayClip(Config.prepare); // 播放准备音效
        UIManager.Instance.prepareUI.Show(); // 显示准备界面
    }

    // 准备界面动画完成后执行（动画事件调用）：正式启动游戏
    public void OnPrepareUIComplete() {
        LevelManager.Instance.level.OnGameStart();
    }

    // 游戏失败处理（如僵尸突破防线时调用）
    public void GameEndFail() {
        if (isGameEnd == true) return; // 已结束则不重复执行
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // 隐藏菜单按钮
        UIManager.Instance.failUI.Show(); // 显示失败界面
        AudioManager.Instance.PlayClip(Config.loseMusic); // 播放失败音效
    }

    // 游戏胜利处理（如击败所有僵尸时调用）
    public void GameEndSuccess() {
        if (isGameEnd == true) return; // 已结束则不重复执行
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // 隐藏菜单按钮
        UIManager.Instance.winUI.Show(); // 显示胜利界面
        AudioManager.Instance.PlayClip(Config.winMusic); // 播放胜利音效
    }

    // 暂停游戏：冻结时间缩放
    public void PauseGame() {
        isPause = true;
        Time.timeScale = 0; // 时间缩放为0，暂停所有基于时间的逻辑
    }

    // 恢复游戏：恢复时间缩放
    public void ResumeGame() {
        isPause = false;
        Time.timeScale = 1; // 时间缩放恢复为1，游戏正常运行
    }

    // 重置游戏：重新加载当前场景
    public void ResetGame() {
        ResumeGame(); // 先恢复游戏（避免场景加载时时间缩放异常）
        DOTween.KillAll(); // 终止所有DOTween动画（清除残留动画）

        AudioManager.Instance.PlayClip(Config.buttonClick); // 播放按钮点击音效
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重新加载当前场景
    }
}