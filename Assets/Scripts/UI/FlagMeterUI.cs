using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 旗帜进度条UI（控制游戏时间和进度显示）
public class FlagMeterUI : MonoBehaviour {

    public GameObject flagMeterImage;
    public Image filgMeterMask;     // 进度遮罩（控制显示比例）
    public Image filgMeterHead;     // 进度条头部图标

    public Transform endPosition;   // 头部图标的终点位置
    public float gameTime = 10;     // 游戏总时长（秒）
    private float gameTimer;        // 计时器

    private bool isGameStart = false; // 游戏是否开始
    private bool isGameEnd = false;   // 游戏是否结束
    private bool isHugeWave = false;

    private int setSpeed;

    private void Start() {
        Hide(); // 初始隐藏
    }

    private void Update() {
        if (isGameStart && !isGameEnd) {
            gameTimer += Time.deltaTime;
            // 更新进度遮罩比例（随时间减少）
            filgMeterMask.fillAmount = (gameTime - gameTimer) / gameTime;

            if (gameTimer > gameTime * 0.25 && setSpeed == 0) {
                setSpeed++;
                ZombieManager.Instance.setSpawnZombieSpeed(1.5f);
                ZombieManager.Instance.setSpawnLevel(2, 7);
            }

            if (gameTimer > gameTime * 0.45 && isHugeWave == false) {
                isHugeWave = true;

                UIManager.Instance.hugeWaveUI.Show();
            }

            if (gameTimer > gameTime * 0.75 && setSpeed == 1) {
                setSpeed++;
                ZombieManager.Instance.setSpawnZombieSpeed(0.5f);
                ZombieManager.Instance.setSpawnLevel(4, 7);
            }

            // 时间到，显示结束界面
            if (gameTimer > gameTime && isGameEnd == false) {
                isGameEnd = true;
   
                UIManager.Instance.finalWaveUI.Show();
                
            }
        }
    }

    // 游戏开始：显示进度条并启动头部图标动画
    public void GameStart() {
        isGameStart = true;
        Show();
        // 头部图标从起点移动到终点（时长=游戏总时间）
        filgMeterHead.transform.DOMove(endPosition.position, gameTime).SetEase(Ease.Linear);
    }

    // 显示进度条（激活背景图片）
    void Show() {
        flagMeterImage.SetActive(true);
    }

    // 隐藏进度条（禁用背景图片）
    void Hide() {
        flagMeterImage.SetActive(false);
    }
}