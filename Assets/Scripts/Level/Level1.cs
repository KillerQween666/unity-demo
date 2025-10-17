using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1 : ILevel {

    private int setSpeedCount = 0;
    private bool isHugeWaveLast1 = false;

    public override void OnCameraMoveRightComplete() {
        StartCoroutine(MoveLeft());
    }

    public override void OnGameStart() {
        SunManager.Instance.StartProduce();
        UIManager.Instance.cardListUI.Show();
        AudioManager.Instance.PlayBgm(Config.bgm1); // «–ªªµΩ”Œœ∑÷˜±≥æ∞“Ù¿÷
        UIManager.Instance.menuUI.ButtonShow(); // œ‘ æ≤Àµ•∞¥≈•
        base.OnGameStart();
    }

    public override void HugeWaveLast1() {
        StartCoroutine(SpawnHugeWaveLast1());
    }

    public override void FinalWave() {
        StartCoroutine(SpawnFinalWave());
    }

    IEnumerator MoveLeft() {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.CameraMoveLeft();
    }

    IEnumerator SpawnHugeWaveLast1() {

        ZombieManager.Instance.SpawnFlagZombie();
        for (int i = 0; i < 2; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(0);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnRowZombie(1);
        }
    }

    IEnumerator SpawnFinalWave() {

        ZombieManager.Instance.SpawnFlagBucketZombie();
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(0.5f);
            ZombieManager.Instance.SpawnRowZombie(2);
            yield return new WaitForSeconds(0.25f);
            ZombieManager.Instance.SpawnRowZombie(3);
        }
    }

    public override void GameController(float gameTime, float gameTimer) {
        base.GameController(gameTime, gameTimer);

        if (gameTimer >= gameTime * 0 && setSpeedCount == 0) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(2.5f);
            ZombieManager.Instance.setSpawnLevel(0, 2);
        }

        if (gameTimer >= gameTime * 0.25 && setSpeedCount == 1) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(1.5f);
            ZombieManager.Instance.setSpawnLevel(0, 4);
        }

        if (gameTimer >= gameTime * 0.45 && isHugeWaveLast1 == false) {
            isHugeWaveLast1 = true;

            UIManager.Instance.hugeWaveUI.Show(1);
        }

        if (gameTimer >= gameTime * 0.75 && setSpeedCount == 2) {
            setSpeedCount++;
            ZombieManager.Instance.setSpawnZombieSpeed(0.5f);
            ZombieManager.Instance.setSpawnLevel(0, 4);
        }
    }
}
