using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 游戏管理器，负责统筹游戏流程（开始、暂停、结束、重置）、输入控制和相机动画
public class GameManager : MonoBehaviour {

    // 单例实例，全局唯一访问点（外部通过此调用游戏核心逻辑）
    public static GameManager Instance { get; private set; }

    // 游戏是否结束的标记（防止重复触发结束逻辑）
    bool isGameEnd = false;

    // 游戏是否暂停的标记
    bool isPause = false;

    // 初始化单例（确保场景中只有一个GameManager）
    private void Awake() {
        Instance = this;
    }

    // 处理玩家输入（空格暂停/恢复、ESC打开/关闭菜单）
    private void Update() {
        // 游戏结束后，不响应任何输入
        if (UIManager.Instance.gameEndUI.isGameEnd == true) return;

        // 空格键：暂停/恢复游戏（菜单显示时不响应）
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

        // ESC键：打开/关闭菜单
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (UIManager.Instance.menuUI.menuImage.activeSelf) {
                UIManager.Instance.menuUI.Hide(); // 菜单已显示则隐藏
            }
            else {
                UIManager.Instance.menuUI.Show(); // 菜单未显示则打开
            }
        }
    }

    // 游戏启动时执行（自动触发）
    private void Start() {
        GameStart();
    }

    // 游戏开始核心逻辑：启动相机移动动画
    void GameStart() {
        StartCoroutine(CameraMove());
    }

    // 相机移动协程（游戏开始时的相机动画）
    IEnumerator CameraMove() {
        // 记录相机初始位置（用于后续还原）
        Vector3 position = Camera.main.transform.position;

        // 等待0.5秒（延迟启动，避免开局太突兀）
        yield return new WaitForSeconds(0.5f);

        // 相机线性移动到(4,0,-10)，动画时长1.5秒
        Camera.main.transform.DOMove(new Vector3(4, 0, -10), 1.5f).SetEase(Ease.Linear);

        // 等待2秒（停留时间，让玩家看清场景）
        yield return new WaitForSeconds(2f);

        // 相机移回初始位置，动画结束后触发准备界面
        Camera.main.transform.DOMove(position, 1.5f).SetEase(Ease.Linear).OnComplete(OnCameraMoveComplete);
    }

    // 相机移动动画完成后执行：显示准备界面
    void OnCameraMoveComplete() {
        AudioManager.Instance.PlayClip(Config.prepare); // 播放准备音效
        UIManager.Instance.prepareUI.Show(); // 显示准备界面
    }

    // 准备界面完成后执行（由PrepareUI的动画事件调用）：正式启动游戏
    public void OnPrepareUIComplete() {
        SunManager.Instance.StartProduce(); // 开启阳光生成
        ZombieManager.Instance.StartSpawn(); // 开始生成僵尸
        UIManager.Instance.cardListUI.Show(); // 显示卡牌列表
        CarManager.instance.ShowCarList(); // 显示小车列表
        AudioManager.Instance.PlayBgm(Config.bgm1); // 播放背景音乐
        UIManager.Instance.menuUI.ButtonShow(); // 显示菜单按钮
    }

    // 游戏失败结束（外部调用，如僵尸突破防线时）
    public void GameEndFail() {
        if (isGameEnd == true) return; // 已结束则不重复执行
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // 隐藏菜单按钮
        UIManager.Instance.failUI.Show(); // 显示失败界面
        AudioManager.Instance.PlayClip(Config.loseMusic); // 播放失败音效
    }

    // 游戏胜利结束（外部调用，如击败所有僵尸时）
    public void GameEndSuccess() {
        if (isGameEnd == true) return; // 已结束则不重复执行
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // 隐藏菜单按钮
        UIManager.Instance.winUI.Show(); // 显示胜利界面
        AudioManager.Instance.PlayClip(Config.winMusic); // 播放胜利音效
    }

    // 暂停游戏（设置时间缩放为0，冻结游戏流程）
    public void PauseGame() {
        isPause = true;
        Time.timeScale = 0; // 时间缩放为0，所有基于Time的逻辑（如计时器、动画）暂停
    }

    // 恢复游戏（设置时间缩放为1，恢复游戏流程）
    public void ResumeGame() {
        isPause = false;
        Time.timeScale = 1; // 时间缩放恢复为1，游戏正常运行
    }

    // 重置游戏（重新加载当前场景，回到初始状态）
    public void ResetGame() {
        ResumeGame(); // 先恢复游戏（避免场景加载时时间缩放异常）
        DOTween.KillAll(); // 终止所有DOTween动画（防止残留动画影响新场景）

        AudioManager.Instance.PlayClip(Config.buttonClick); // 播放按钮点击音效
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 重新加载当前场景
    }
}