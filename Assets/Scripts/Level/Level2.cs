using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2 : ILevel{

    private int setSpeedCount = 0;
    private bool isHugeWaveLast1 = false;
    private bool isHugeWaveLast2 = false;

    public override void OnCameraMoveRightComplete() {
        UIManager.Instance.cardListUI.Show(); // 显示卡牌列表界面
        UIManager.Instance.menuUI.ButtonShow(); // 显示菜单按钮
        UIManager.Instance.CardChooserUI.Show(); // 显示卡牌选择界面
    }

    public override void OnGameStart() {
        AudioManager.Instance.PlayBgm(Config.bgm2); // 切换到游戏主背景音乐
        CellManager.Instance.StartSpawn(6);
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

        ZombieManager.Instance.SpawnAllTombstoneZombiesIenumerator(0);
        ZombieManager.Instance.SpawnFlagZombie();
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(2);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnRowZombie(3);
        }
    }

    IEnumerator SpawnHugeWaveLast1() {
        CellManager.Instance.StartSpawnTombstones(2);
        yield return new WaitForSeconds(1.3f);

        ZombieManager.Instance.SpawnAllTombstoneZombiesIenumerator(1, 3);

        ZombieManager.Instance.SpawnFlagZombie();
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(4);  
        }

        yield return new WaitForSeconds(2f);
        ZombieManager.Instance.SpawnRowZombie(7);
    }

    IEnumerator SpawnFinalWave() {
        CellManager.Instance.StartSpawnTombstones(4);
        yield return new WaitForSeconds(1.3f);

        ZombieManager.Instance.SpawnAllTombstoneZombiesIenumerator(4);

        ZombieManager.Instance.SpawnFlagBucketZombie();
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(5);
        }
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(6);
        }
        yield return new WaitForSeconds(2f)
            ;
        yield return new WaitForSeconds(0.5f);
        ZombieManager.Instance.SpawnRowZombie(7);
    }

    public override void GameController(float gameTime, float gameTimer) {
        base.GameController(gameTime, gameTimer);

        if (gameTimer >= gameTime * 0 && setSpeedCount == 0) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(2.5f);
            ZombieManager.Instance.setSpawnLevel(0, 2);
            CellManager.Instance.SetSpawnTime(20);
        }

        if (gameTimer >= gameTime * 0.25 && setSpeedCount == 1) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(1.75f);
            ZombieManager.Instance.setSpawnLevel(0, 6);
            CellManager.Instance.SetSpawnTime(17);
        }

        if (gameTimer >= gameTime * 0.5 && setSpeedCount == 2) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(1f);
            ZombieManager.Instance.setSpawnLevel(2, 8);
            CellManager.Instance.SetSpawnTime(14);
        }

        if (gameTimer >= gameTime * 0.8 && setSpeedCount == 3) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(0.5f);
            ZombieManager.Instance.setSpawnLevel(4, 8);
            CellManager.Instance.SetSpawnTime(10);
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
