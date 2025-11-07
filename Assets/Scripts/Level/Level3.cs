using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level3 : ILevel {

    private int setSpeedCount = 0;
    private bool isHugeWaveLast1 = false;
    private bool isHugeWaveLast2 = false;

    public override void OnCameraMoveRightComplete() {
        UIManager.Instance.cardListUI.Show(); // 显示卡牌列表界面
        UIManager.Instance.menuUI.ButtonShow(); // 显示菜单按钮
        UIManager.Instance.CardChooserUI.Show(); // 显示卡牌选择界面
    }

    public override void OnGameStart() {
        SunManager.Instance.StartProduce();
        AudioManager.Instance.PlayBgm(Config.bgm3); // 切换到游戏主背景音乐
        base.OnGameStart();
    }

    public override void HugeWaveLast1() {
        StartCoroutine(SpawnHugeWaveLast1());
    }

    public override void HugeWaveLast2() {
        StartCoroutine(SpawnHugeWaveLast2());
    }

    public override void FinalWave() {
        StartCoroutine(SpawnFinalWave());
    }

    IEnumerator SpawnHugeWaveLast2() {

        ZombieManager.Instance.SpawnAllWaterCellZombiesIenumerator(3, 0);

        ZombieManager.Instance.SpawnFlagZombie();
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(0);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnRowZombie(1);
        }
    }

    IEnumerator SpawnHugeWaveLast1() {

        ZombieManager.Instance.SpawnAllWaterCellZombiesIenumerator(6, 1, 3);

        ZombieManager.Instance.SpawnFlagZombie();
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(2);
        }
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnWaterRowZombie(3);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnLandRowZombie(3);
        }

    }

    IEnumerator SpawnFinalWave() {

        ZombieManager.Instance.SpawnAllWaterCellZombiesIenumerator(10, 2);
        ZombieManager.Instance.SpawnFlagBucketZombie();

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 4; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnLandRowZombie(6);
            ZombieManager.Instance.SpawnWaterRowZombie(3);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnWaterRowZombie(4);
        }
    }

    

    public override void GameController(float gameTime, float gameTimer) {
        base.GameController(gameTime, gameTimer);

        if (gameTimer >= gameTime * 0 && setSpeedCount == 0) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(2.4f);
            ZombieManager.Instance.setSpawnLevel(0, 2);
            ZombieManager.Instance.setWaterSpawnLevel(0, 2);
        }

        if (gameTimer >= gameTime * 0.25 && setSpeedCount == 1) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(1.6f);
            ZombieManager.Instance.setSpawnLevel(0, 5);
            ZombieManager.Instance.setWaterSpawnLevel(1, 3);
        }

        if (gameTimer >= gameTime * 0.5 && setSpeedCount == 2) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(1f);
            ZombieManager.Instance.setSpawnLevel(2, 7);
            ZombieManager.Instance.setWaterSpawnLevel(1, 5);
        }

        if (gameTimer >= gameTime * 0.75 && setSpeedCount == 3) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(0.4f);
            ZombieManager.Instance.setSpawnLevel(4, 7);
            ZombieManager.Instance.setWaterSpawnLevel(2, 5);
        }


        if (gameTimer >= gameTime * 0.25 && isHugeWaveLast2 == false) {
            isHugeWaveLast2 = true;

            UIManager.Instance.hugeWaveUI.Show(2);
        }

        if (gameTimer >= gameTime * 0.55 && isHugeWaveLast1 == false) {
            isHugeWaveLast1 = true;

            UIManager.Instance.hugeWaveUI.Show(1);
        }

    }
}
