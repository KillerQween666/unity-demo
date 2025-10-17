using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 僵尸生成状态枚举：未开始、生成中、生成结束
enum SpawnState {
    NotStart,
    Spawning,
    End
}

// 僵尸管理器，负责僵尸的生成（普通僵尸、旗帜僵尸）、数量统计及游戏胜利判断
public class ZombieManager : MonoBehaviour {

    // 单例实例，全局唯一访问点（外部控制僵尸生成逻辑）
    public static ZombieManager Instance { get; private set; }

    // 当前僵尸生成状态（默认未开始）
    private SpawnState spawnState = SpawnState.NotStart;

    // 僵尸生成点数组（在Inspector赋值，对应不同行的生成位置）
    public Transform[] spawnPointList;

    // 旗帜僵尸预制体（通常为最终波僵尸）
    public Zombie zombieFlagPrefab;
    public Zombie zombieFlagBucketPrefab;

    public List<Zombie> zombiePrefabs = new List<Zombie>();
    public List<float> spawnWeights = new List<float>();

    // 僵尸渲染层级排序值（控制僵尸显示先后，避免遮挡）
    public static int sortOrder = 100;

    // 当前场景中的僵尸总数（用于判断是否所有僵尸被消灭）
    private int zombieCount = 0;

    public float groanTime = 5f;
    private float groanTimer = 0;

    private float spawnZombieSpeed;
    private float spawnZombieTime;
    private float spawnZombieTimer;

    private int spawnlowerLevel = 0;
    private int spawnUpperLevel = 2;

    public string[] layerNames = new string[] { "Row1", "Row2", "Row3", "Row4", "Row5" };

    // 初始化单例
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        setSpawnZombieSpeed(2.5f);
    }

    // 每帧更新：生成结束且所有僵尸被消灭时，触发游戏胜利
    private void Update() {
        switch (spawnState) {
            case SpawnState.NotStart:
                break;
            case SpawnState.Spawning:
                SpawningUpdate();
                break;
            case SpawnState.End:
                EndUpdate();
                break;
            default:
                break;
        }
    }

    void EndUpdate() {
        if (spawnState == SpawnState.End && zombieCount == 0) {
            GameManager.Instance.GameEndSuccess();
        }
    }

    void SpawningUpdate() {
        groanTimer += Time.deltaTime;
        if (groanTimer > groanTime) {
            groanTimer = 0;

            var clips = new[] { Config.groan, Config.groan2, Config.groan3, Config.groan4, Config.groan5, Config.groan6 };
            AudioManager.Instance.PlayClip(clips[Random.Range(0, 6)]);
        }

        spawnZombieTimer += Time.deltaTime;
        if (spawnZombieTimer > spawnZombieTime) {
            spawnZombieTimer = 0;

            SpawnRandomZombie();
        }
    }

    public IEnumerator StartSpawn() {
        yield return new WaitForSeconds(10);

        AudioManager.Instance.PlayClip(Config.zombieStartSpawn); // 播放僵尸开始生成的音效
        UIManager.Instance.flagMeterUI.GameStart(); // 启动旗帜进度条（游戏计时）
        spawnState = SpawnState.Spawning; // 切换到生成中状态
    }

    public void HugeWaveSpawnLast1() {
        LevelManager.Instance.level.HugeWaveLast1();
    }

    public void HugeWaveSpawnLast2() {
        LevelManager.Instance.level.HugeWaveLast2();
    }

    public void FinalWaveSpawn() {
        LevelManager.Instance.level.FinalWave();
        spawnState = SpawnState.End;
    }

    // 随机生成一只僵尸（随机选择一个生成点）
    public void SpawnRandomZombie(int ran = -1) {

        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // 随机选一个生成点
        // 实例化普通僵尸到选中的生成点

        if (ran == -1)   ran = Random.Range(spawnlowerLevel, spawnUpperLevel);

        Zombie zombie = Instantiate(zombiePrefabs[ran], spawnPointList[index].position, Quaternion.identity);
        spawnZombieTime = spawnZombieSpeed * spawnWeights[ran];

        // 调整僵尸所有子物体的渲染层级（按生成点行号和排序值，避免同屏僵尸遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = layerNames[index];
            sprite.sortingOrder += sortOrder;
        }
        zombie.row = index;
        AddZombie();
    }

    // 生成一只旗帜僵尸（随机选择一个生成点）
    public void SpawnFlagBucketZombie() {
        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // 随机选一个生成点
        // 实例化旗帜僵尸到选中的生成点
        Zombie zombie = Instantiate(zombieFlagPrefab, spawnPointList[index].position, Quaternion.identity);

        // 调整旗帜僵尸的渲染层级（同普通僵尸逻辑，避免遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = layerNames[index];
            sprite.sortingOrder += sortOrder;
        }
        zombie.row = index;
        AddZombie();
    }

    public void SpawnFlagZombie() {
        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // 随机选一个生成点
        // 实例化旗帜僵尸到选中的生成点
        Zombie zombie = Instantiate(zombieFlagPrefab, spawnPointList[index].position, Quaternion.identity);

        // 调整旗帜僵尸的渲染层级（同普通僵尸逻辑，避免遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingLayerName = layerNames[index];
            sprite.sortingOrder += sortOrder;
        }
        zombie.row = index;
        AddZombie();
    }

    // 生成一整行选定的僵尸（覆盖所有生成点，每行5只）
    public void SpawnRowZombie(int ran) {
        // 遍历所有生成点，每个点生成一只铁桶僵尸
        for (int i = 0; i < 5; i++) {
            Zombie zombie = Instantiate(zombiePrefabs[ran], spawnPointList[i].position, Quaternion.identity);

            // 调整渲染层级（按生成点行号区分，避免同行僵尸遮挡）
            SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = layerNames[i];
                sprite.sortingOrder += sortOrder;
            }

            zombie.row = i;
            AddZombie();
        }
    }

    public void setSpawnZombieSpeed(float speed) {
        spawnZombieSpeed = speed;
    }

    public void setSpawnLevel(int lowerLevel, int upperLevel) {
        spawnlowerLevel = lowerLevel;
        spawnUpperLevel = upperLevel;
    }

    public void AddZombie() {
        sortOrder += 100;
        zombieCount++;
    }

    // 移除一只僵尸（僵尸死亡时调用，减少总数）
    public void RemoveZombie() {
        zombieCount--;
    }

    public void SpawnAllTombstoneZombiesIenumerator(int ran, int ran2 = -1) {
        StartCoroutine(SpawnAllTombstoneZombies(ran, ran2));
    }

    public IEnumerator SpawnAllTombstoneZombies(int ran, int ran2 = -1) {
        List<Cell> cellList = CellManager.Instance.GetAllHaveBraveCell();
        AudioManager.Instance.PlayClip(Config.spawnGrave);

        foreach (var cell in cellList) {

            Vector3 dirtPosition = cell.transform.position;
            dirtPosition.y -= 0.5f;

            ObjectPoolManager.Instance.PlayDirtSmallParticalIEnumrator(dirtPosition);
            ObjectPoolManager.Instance.PlayDirtBigParticalIEnumrator(cell.transform.position);
        }

        yield return new WaitForSeconds(1);

        foreach (var cell in cellList) {
            Zombie zombie;

            Vector3 position = cell.transform.position, position2 = cell.transform.position;
            position.y -= 2f;
            position2.y -= 1f;

            if (ran2 != -1) {
                zombie = Instantiate(zombiePrefabs[Random.Range(ran, ran2)], position, Quaternion.identity);
            } else {
               zombie = Instantiate(zombiePrefabs[ran], position, Quaternion.identity);
            }

            zombie.transform.DOMoveY(position2.y, 0.2f);  

            SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingLayerName = layerNames[cell.row];
                sprite.sortingOrder += sortOrder;
            }

            AddZombie();
        }
    }

}