using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ILevel : MonoBehaviour {

    public virtual void GameController(float gameTime, float gameTimer) {
        // 时间到，显示结束界面
        if (gameTimer > gameTime) {
            UIManager.Instance.flagMeterUI.isGameing = false;

            CellManager.Instance.EndSpawn();
            UIManager.Instance.finalWaveUI.Show();
        }
    }

    public virtual void HugeWaveLast1() {

    }

    public virtual void HugeWaveLast2() {

    }

    public virtual void FinalWave() {

    }

    public virtual void OnCameraMoveRightComplete() {

    }

    public virtual void OnGameStart() {
        CardManager.Instance.EnableCards(); // 启用所有卡牌
        UIManager.Instance.cardListUI.ShovelUIShow(); // 显示铲子UI
        StartCoroutine(ZombieManager.Instance.StartSpawn()); // 开始生成僵尸
        CarManager.instance.ShowCarList(); // 显示小推车列表
    }


}
