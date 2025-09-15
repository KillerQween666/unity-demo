using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// 对象池管理器，通过对象池复用高频创建/销毁的对象（子弹、阳光、粒子、音频源），减少性能消耗
public class ObjectPoolManager : MonoBehaviour {

    // 单例实例，全局唯一访问点（外部通过此获取/回收对象）
    public static ObjectPoolManager Instance { get; private set; }

    // 各类对象的预制体（在Inspector赋值，用于创建对象池初始实例）
    public GameObject peaBulletPartical;  // 豌豆子弹粒子效果预制体
    public GameObject peaBullet;          // 豌豆子弹预制体
    public GameObject sun;                // 阳光预制体
    public GameObject headEmissionPartical;// 头部发射粒子效果预制体
    public GameObject handEmissionPartical;// 手部发射粒子效果预制体
    private GameObject audioSource;       // 音频源对象（动态创建，无需预制体）

    // 各类对象对应的对象池（管理对象的创建、复用、销毁）
    private ObjectPool<GameObject> peaBulletParticalPool;
    private ObjectPool<GameObject> peaBulletPool;
    private ObjectPool<GameObject> sunPool;
    private ObjectPool<GameObject> headEmissionParticalPool;
    private ObjectPool<GameObject> handEmissionParticalPool;
    private ObjectPool<GameObject> sourcePool; // 音频源对象池

    // 初始化单例
    private void Awake() {
        Instance = this;
    }

    // 游戏启动时初始化所有对象池（设置创建、获取、回收、销毁的回调逻辑）
    private void Start() {
        // 豌豆子弹粒子池：初始10个，最大300个，超出最大数量则销毁对象
        peaBulletParticalPool = new ObjectPool<GameObject>(
            CreatePeaBulletPartical,  // 创建对象的方法
            ActionOnGet,               // 从池获取对象时的回调
            ActionOnRelease,           // 回收对象到池时的回调
            ActionOnDestroy,           // 对象超出池最大数量时的销毁回调
            true, 10, 300              // 允许集合收缩、初始容量、最大容量
        );

        // 豌豆子弹池：同上，获取时额外重置子弹状态
        peaBulletPool = new ObjectPool<GameObject>(
            CreatePeaBullet,
            ActionOnGetPeaBullet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 阳光池：同上，获取时额外重置阳光状态
        sunPool = new ObjectPool<GameObject>(
            CreateSun,
            ActionOnGetSun,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 头部粒子池：基础对象池逻辑
        headEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 手部粒子池：基础对象池逻辑
        handEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 音频源池：基础对象池逻辑
        sourcePool = new ObjectPool<GameObject>(
            CreateSource,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );
    }

    // 创建豌豆子弹实例（对象池调用，生成新子弹）
    GameObject CreatePeaBullet() {
        return Instantiate(peaBullet);
    }

    // 创建豌豆子弹粒子实例
    GameObject CreatePeaBulletPartical() {
        return Instantiate(peaBulletPartical);
    }

    // 创建阳光实例
    GameObject CreateSun() {
        return Instantiate(sun);
    }

    // 创建头部粒子实例
    GameObject CreateHeadEmissionPartical() {
        return Instantiate(headEmissionPartical);
    }

    // 创建手部粒子实例
    GameObject CreateHandEmissionPartical() {
        return Instantiate(handEmissionPartical);
    }

    // 创建音频源实例（动态添加AudioSource组件，无需预制体）
    GameObject CreateSource() {
        GameObject obj = new GameObject("AudioSource"); // 命名对象，方便调试
        obj.AddComponent<AudioSource>();                // 给对象添加音频源组件
        return obj;
    }

    // 基础获取回调：从池里拿对象时，激活对象
    void ActionOnGet(GameObject obj) {
        obj.SetActive(true);
    }

    // 豌豆子弹获取回调：激活对象前，重置子弹状态（避免复用旧状态）
    void ActionOnGetPeaBullet(GameObject obj) {
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.liveTimer = 0;    // 重置生命周期计时器
        peaBullet.isAttack = false; // 重置攻击状态
        peaBullet.isRelease = false;// 重置回收状态
        obj.SetActive(true);        // 激活对象
    }

    // 阳光获取回调：激活对象前，重置阳光状态
    void ActionOnGetSun(GameObject obj) {
        Sun sun = obj.GetComponent<Sun>();
        sun.liveTimer = 0;          // 重置生命周期计时器
        sun.isClick = false;        // 重置点击状态
        sun.sunCollider2D.enabled = true; // 启用碰撞体（确保能被收集）
        obj.SetActive(true);        // 激活对象
    }

    // 基础回收回调：回收对象到池时，禁用对象（不销毁，留待复用）
    void ActionOnRelease(GameObject obj) {
        obj.SetActive(false);
    }

    // 对象销毁回调：对象超出池最大容量时，彻底销毁（释放内存）
    void ActionOnDestroy(GameObject obj) {
        Destroy(obj);
    }

    // 对外提供：获取豌豆子弹（外部调用，如豌豆射手发射时）
    public GameObject GetPeaBullet() {
        return peaBulletPool.Get();
    }

    // 对外提供：回收豌豆子弹（外部调用，如子弹命中或超时后）
    public void ReleasePeaBullet(GameObject gameObject) {
        peaBulletPool.Release(gameObject);              // 回收到对象池
        gameObject.GetComponent<PeaBullet>().isRelease = true; // 标记为已回收
    }

    // 对外提供：获取豌豆子弹粒子
    public GameObject GetPeaBulletPartical() {
        return peaBulletParticalPool.Get();
    }

    // 对外提供：回收豌豆子弹粒子
    public void ReleasePeaBulletPartical(GameObject gameObject) {
        peaBulletParticalPool.Release(gameObject);
    }

    // 对外提供：获取阳光（外部调用，如向日葵生成阳光时）
    public GameObject GetSun() {
        return sunPool.Get();
    }

    // 对外提供：回收阳光（外部调用，如阳光被收集或超时后）
    public void ReleaseSun(GameObject gameObject) {
        sunPool.Release(gameObject);
    }

    // 对外提供：获取头部粒子
    public GameObject GetHeadEmissionPartical() {
        return headEmissionParticalPool.Get();
    }

    // 对外提供：回收头部粒子
    public void ReleaseHeadEmissionPartical(GameObject gameObject) {
        headEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取手部粒子
    public GameObject GetHandEmissionPartical() {
        return handEmissionParticalPool.Get();
    }

    // 对外提供：回收手部粒子
    public void ReleaseHandEmissionPartical(GameObject gameObject) {
        handEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取音频源（外部调用，如播放音效时）
    public GameObject GetSource() {
        return sourcePool.Get();
    }

    // 对外提供：回收音频源（外部调用，如音效播放完成后）
    public void ReleaseSource(GameObject gameObject) {
        sourcePool.Release(gameObject);
    }

    // 对外提供：播放豌豆子弹粒子效果（启动协程，控制粒子播放时长）
    public void PlayPeaBulletParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayPeaBulletPartical(transform));
    }

    // 对外提供：播放头部粒子效果（带渲染层级参数，避免粒子遮挡）
    public void PlayHeadEmissionIEnumrator(Transform transform, int sort) {
        StartCoroutine(PlayHeadEmissionPartical(transform, sort));
    }

    // 对外提供：播放手部粒子效果（带渲染层级参数）
    public void PlayHandEmissionIEnumrator(Transform transform, int sort) {
        StartCoroutine(PlayHandEmissionPartical(transform, sort));
    }

    // 豌豆子弹粒子播放协程：播放粒子后，等待0.6秒再回收
    public IEnumerator PlayPeaBulletPartical(Transform transform) {
        GameObject obj = GetPeaBulletPartical();       // 从池获取粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        particle.transform.position = transform.position; // 设为目标位置（如子弹命中点）
        particle.Play();                                // 播放粒子

        yield return new WaitForSeconds(0.6f);          // 等待粒子播放完成

        particle.Clear();                               // 清除残留粒子
        ReleasePeaBulletPartical(obj);                  // 回收粒子到池
    }

    // 头部粒子播放协程：按指定层级播放，播放完成后回收
    public IEnumerator PlayHeadEmissionPartical(Transform transform, int sort) {
        GameObject obj = GetHeadEmissionPartical();
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置粒子渲染层级
        particle.transform.position = transform.position;
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration); // 等待粒子自然播放完成

        particle.Clear();
        ReleaseHeadEmissionPartical(obj);
    }

    // 手部粒子播放协程：按指定层级播放，播放完成后回收
    public IEnumerator PlayHandEmissionPartical(Transform transform, int sort) {
        GameObject obj = GetHandEmissionPartical();
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置粒子渲染层级
        particle.transform.position = transform.position;
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration); // 等待粒子自然播放完成

        particle.Clear();
        ReleaseHandEmissionPartical(obj);
    }
}