using System;
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
    // 普通僵尸预制体
    public Zombie zombiePrefab;
    // 旗帜僵尸预制体（通常为最终波僵尸）
    public ZombieFlag zombieFlagPrefab;

    // 僵尸渲染层级排序值（控制僵尸显示先后，避免遮挡）
    public static int sortOrder = 100;

    // 当前场景中的僵尸总数（用于判断是否所有僵尸被消灭）
    private int zombieCount = 0;

    // 初始化单例
    private void Awake() {
        Instance = this;
    }

    // 每帧更新：生成结束且所有僵尸被消灭时，触发游戏胜利
    private void Update() {
        if (spawnState == SpawnState.End && zombieCount == 0) {
            GameManager.Instance.GameEndSuccess();
        }
    }

    // 开始生成僵尸（由GameManager调用，启动生成流程）
    public void StartSpawn() {
        spawnState = SpawnState.Spawning; // 切换到生成中状态
        StartCoroutine(SpawnZombie()); // 启动生成协程
    }

    // 僵尸生成协程（分波生成，控制生成间隔和类型）
    IEnumerator SpawnZombie() {
        // 初始等待10秒（给玩家准备时间）
        yield return new WaitForSeconds(10);

        AudioManager.Instance.PlayClip(Config.zombieStartSpawn); // 播放僵尸开始生成的音效
        UIManager.Instance.flagMeterUI.GameStart(); // 启动旗帜进度条（游戏计时）

        // 第一波：生成10只普通僵尸，每2.5秒一只（慢节奏）
        for (int i = 0; i < 10; i++) {
            SpawnRandomZombie();
            yield return new WaitForSeconds(2.5f);
        }

        // 第二波：生成10只普通僵尸，每1.5秒一只（节奏加快）
        for (int i = 0; i < 10; i++) {
            SpawnRandomZombie();
            yield return new WaitForSeconds(1.5f);
        }

        // 等待5秒（过渡到最终波）
        yield return new WaitForSeconds(5);

        // 最终波：先生成1只旗帜僵尸，再生成3行全屏普通僵尸（每行5只）
        SpawnFlagZombie();
        for (int i = 0; i < 3; i++) {
            SpawnRowZombie();
            yield return new WaitForSeconds(1); // 每行间隔1秒
        }

        spawnState = SpawnState.End; // 所有僵尸生成完成，切换到生成结束状态
    }

    // 随机生成一只普通僵尸（随机选择一个生成点）
    private void SpawnRandomZombie() {
        if (spawnState != SpawnState.Spawning) return; // 非生成中状态，不执行

        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // 随机选一个生成点
        // 实例化普通僵尸到选中的生成点
        Zombie zombie = Instantiate(zombiePrefab, spawnPointList[index].position, Quaternion.identity);

        // 调整僵尸所有子物体的渲染层级（按生成点行号和排序值，避免同屏僵尸遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingOrder += index * 1000 + sortOrder;
        }
        sortOrder += 100; // 每生成一只僵尸，排序值递增（后生成的显示在上面）

        zombieCount++; // 僵尸总数+1
    }

    // 生成一只旗帜僵尸（随机选择一个生成点）
    private void SpawnFlagZombie() {
        if (spawnState != SpawnState.Spawning) return; // 非生成中状态，不执行

        int index = UnityEngine.Random.Range(0, spawnPointList.Length); // 随机选一个生成点
        // 实例化旗帜僵尸到选中的生成点
        Zombie zombie = Instantiate(zombieFlagPrefab, spawnPointList[index].position, Quaternion.identity);

        // 调整旗帜僵尸的渲染层级（同普通僵尸逻辑，避免遮挡）
        SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            sprite.sortingOrder += index * 1000 + sortOrder;
        }
        sortOrder += 100;

        zombieCount++; // 僵尸总数+1
    }

    // 生成一整行普通僵尸（覆盖所有生成点，每行5只）
    private void SpawnRowZombie() {
        // 遍历所有生成点，每个点生成一只普通僵尸
        for (int i = 0; i < 5; i++) {
            Zombie zombie = Instantiate(zombiePrefab, spawnPointList[i].position, Quaternion.identity);

            // 调整渲染层级（按生成点行号区分，避免同行僵尸遮挡）
            SpriteRenderer[] sprites = zombie.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var sprite in sprites) {
                sprite.sortingOrder += i * 1000 + sortOrder;
            }
            sortOrder += 100;

            zombieCount++; // 僵尸总数+1
        }
    }

    // 移除一只僵尸（僵尸死亡时调用，减少总数）
    public void RemoveZombie() {
        zombieCount--;
    }

}