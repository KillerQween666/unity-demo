using DG.Tweening;
using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SocialPlatforms;

// 对象池管理器：通过复用高频创建/销毁的对象（子弹、阳光、粒子、音频源等）减少性能消耗，优化内存使用
public class ObjectPoolManager : MonoBehaviour {

    // 单例实例：全局唯一访问点，外部通过此获取或回收对象
    public static ObjectPoolManager Instance { get; private set; }

    // 各类可复用对象的预制体（需在Inspector面板赋值，用于初始化对象池）
    public GameObject peaBulletPartical;         // 豌豆子弹命中粒子效果预制体
    public GameObject puffPeaBulletPartical;
    public GameObject snowPeaBulletPartical;     // 寒冰豌豆子弹命中粒子效果预制体
    public GameObject fumeAttackPartical;
    public GameObject peaBullet;                 // 豌豆子弹预制体
    public GameObject puffPeaBullet;
    public GameObject snowPeaBullet;             // 寒冰豌豆子弹预制体
    public GameObject firePeaBullet;             
    public GameObject sun;                       // 阳光预制体
    public GameObject smallSun;                       // 阳光预制体
    public GameObject headEmissionPartical;      // 头部发射粒子效果预制体（emission：发射）
    public GameObject handEmissionPartical;      // 手部发射粒子效果预制体（emission：发射）
    public GameObject poleHeadEmissionPartical;  // 撑杆僵尸头部发射粒子预制体
    public GameObject poleHandEmissionPartical;  // 撑杆僵尸手部发射粒子预制体
    public GameObject coneEmissionPartical;      // 路障僵尸发射粒子预制体
    public GameObject bucketEmissionPartical;    // 铁桶僵尸发射粒子预制体
    public GameObject flagEmissionPartical;      // 旗帜僵尸发射粒子预制体
    public GameObject potatoBoomPartical;        // 土豆地雷爆炸粒子预制体
    public GameObject iceShroomBoomPartical;
    public GameObject doomShroomBoomPartical;
    public GameObject zombieCarBoomPartical;
    public GameObject iceCrackPartical;
    public GameObject hypnoPartical;
    public GameObject dirtSmallPartical;
    public GameObject dirtBigPartical;
    public GameObject rockSmallPartical;
    public GameObject waterFallPartical;
    public GameObject wallnutHurtSmallPartical;  // 坚果墙轻伤粒子预制体
    public GameObject wallnutHurtLargePartical;  // 坚果墙重伤粒子预制体
    public GameObject zombieBoomSwf;             // 僵尸被炸飞动画预制体
    public GameObject zombieZamboniBoomSwf;
    public GameObject firePeaBulletSwf;
    public GameObject fallWaterSwf;
    public GameObject cherryBombBoomPartical;    // 樱桃炸弹爆炸粒子预制体
    public GameObject doorEmissionPartical;
    public GameObject footballHeadEmissionPartical;
    public GameObject footballHandEmissionPartical;
    public GameObject footballHelmetEmissionPartical;
    public GameObject paperHeadEmissionPartical;
    public GameObject paperHandEmissionPartical;
    public GameObject paperEmissionPartical;
    public GameObject dancerHeadEmissionPartical;
    public GameObject dancerHandEmissionPartical;
    public GameObject jaksonHeadEmissionPartical;
    public GameObject jaksonHandEmissionPartical;
    public GameObject bobsledHeadEmissionPartical;
    public GameObject bobsledHandEmissionPartical;
    public GameObject dolphinriderHeadEmissionPartical;
    public GameObject dolphinriderHandEmissionPartical;
    public GameObject snorkleHeadEmissionPartical;
    public GameObject snorkleHandEmissionPartical;

    // 各类对象对应的对象池（管理对象的创建、复用、销毁全生命周期）
    private ObjectPool<GameObject> peaBulletParticalPool;
    private ObjectPool<GameObject> puffPeaBulletParticalPool;
    private ObjectPool<GameObject> snowPeaBulletParticalPool;
    private ObjectPool<GameObject> fumeAttackParticalPool;
    private ObjectPool<GameObject> peaBulletPool;
    private ObjectPool<GameObject> puffPeaBulletPool;
    private ObjectPool<GameObject> snowPeaBulletPool;
    private ObjectPool<GameObject> firePeaBulletPool;
    private ObjectPool<GameObject> sunPool;
    private ObjectPool<GameObject> smallSunPool;
    private ObjectPool<GameObject> headEmissionParticalPool;      // 头部发射粒子池
    private ObjectPool<GameObject> handEmissionParticalPool;      // 手部发射粒子池
    private ObjectPool<GameObject> sourcePool;                    // 音频源对象池
    private ObjectPool<GameObject> poleHeadEmissionParticalPool;  // 撑杆僵尸头部发射粒子池
    private ObjectPool<GameObject> poleHandEmissionParticalPool;  // 撑杆僵尸手部发射粒子池
    private ObjectPool<GameObject> coneEmissionParticalPool;      // 路障僵尸发射粒子池
    private ObjectPool<GameObject> bucketEmissionParticalPool;    // 铁桶僵尸发射粒子池
    private ObjectPool<GameObject> flagEmissionParticalPool;      // 旗帜僵尸发射粒子池
    private ObjectPool<GameObject> potatoBoomParticalPool;        // 土豆地雷爆炸粒子池
    private ObjectPool<GameObject> iceShroomBoomParticalPool;
    private ObjectPool<GameObject> doomShroomBoomParticalPool;
    private ObjectPool<GameObject> zombieCarBoomParticalPool;
    private ObjectPool<GameObject> iceCrackParticalPool;
    private ObjectPool<GameObject> hypnoParticalPool;
    private ObjectPool<GameObject> dirtSmallParticalPool;
    private ObjectPool<GameObject> dirtBigParticalPool;
    private ObjectPool<GameObject> rockSmallParticalPool;
    private ObjectPool<GameObject> waterFallParticalPool;
    private ObjectPool<GameObject> wallnutHurtSmallParticalPool;  // 坚果墙轻伤粒子池
    private ObjectPool<GameObject> wallnutHurtLargeParticalPool;  // 坚果墙重伤粒子池
    private ObjectPool<GameObject> zombieBoomSwfPool;             // 僵尸被炸飞动画池
    private ObjectPool<GameObject> zombieZamboniBoomSwfPool;
    private ObjectPool<GameObject> firePeaBulletSwfPool;
    private ObjectPool<GameObject> fallWaterSwfPool;
    private ObjectPool<GameObject> cherryBombBoomParticalPool;    // 樱桃炸弹爆炸粒子池
    private ObjectPool<GameObject> doorEmissionParticalPool;
    private ObjectPool<GameObject> footballHeadEmissionParticalPool;
    private ObjectPool<GameObject> footballHandEmissionParticalPool;
    private ObjectPool<GameObject> footballHelmetEmissionParticalPool;
    private ObjectPool<GameObject> paperHeadEmissionParticalPool;
    private ObjectPool<GameObject> paperHandEmissionParticalPool;
    private ObjectPool<GameObject> paperEmissionParticalPool;
    private ObjectPool<GameObject> dancerHeadEmissionParticalPool;
    private ObjectPool<GameObject> dancerHandEmissionParticalPool;
    private ObjectPool<GameObject> jaksonHeadEmissionParticalPool;
    private ObjectPool<GameObject> jaksonHandEmissionParticalPool;
    private ObjectPool<GameObject> bobsledHeadEmissionParticalPool;
    private ObjectPool<GameObject> bobsledHandEmissionParticalPool;
    private ObjectPool<GameObject> dolphinriderHeadEmissionParticalPool;
    private ObjectPool<GameObject> dolphinriderHandEmissionParticalPool;
    private ObjectPool<GameObject> snorkleHeadEmissionParticalPool;
    private ObjectPool<GameObject> snorkleHandEmissionParticalPool;

    private Color crozeColor = new Color(0.4f, 0.5568f, 1);  // 特殊发射粒子颜色（如冰冻效果）
    private Color hypnoColor = new Color(0.9320f, 0, 0.6588f);

    // 初始化单例，确保全局唯一实例
    private void Awake() {
        Instance = this;
    }

    // 游戏启动时初始化所有对象池，配置创建、获取、回收、销毁的回调逻辑
    private void Start() {
        // 豌豆子弹粒子池：初始10个实例，最大300个，超出最大数量则销毁对象
        peaBulletParticalPool = new ObjectPool<GameObject>(
            CreatePeaBulletPartical,  // 创建新对象的方法
            ActionOnGet,               // 从池获取对象时的回调（激活对象）
            ActionOnRelease,           // 回收对象到池时的回调（禁用对象）
            ActionOnDestroy,           // 对象超出池最大容量时的销毁回调
            true, 10, 300              // 允许池自动收缩、初始容量、最大容量
        );

        puffPeaBulletParticalPool = new ObjectPool<GameObject>(
            CreatePuffPeaBulletPartical,  // 创建新对象的方法
            ActionOnGet,               // 从池获取对象时的回调（激活对象）
            ActionOnRelease,           // 回收对象到池时的回调（禁用对象）
            ActionOnDestroy,           // 对象超出池最大容量时的销毁回调
            true, 10, 300              // 允许池自动收缩、初始容量、最大容量
        );

        // 寒冰豌豆子弹粒子池：配置同普通豌豆粒子池
        snowPeaBulletParticalPool = new ObjectPool<GameObject>(
            CreateSnowPeaBulletPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        fumeAttackParticalPool = new ObjectPool<GameObject>(
            CreateFumeAttackPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 樱桃炸弹爆炸粒子池：基础对象池配置
        cherryBombBoomParticalPool = new ObjectPool<GameObject>(
            CreateCherryBombBoomPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 土豆地雷爆炸粒子池：基础对象池配置
        potatoBoomParticalPool = new ObjectPool<GameObject>(
            CreatePotatoBoomPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        iceShroomBoomParticalPool = new ObjectPool<GameObject>(
            CreateIceShroomBoomPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        iceCrackParticalPool = new ObjectPool<GameObject>(
            CreateIceCrackPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        doomShroomBoomParticalPool = new ObjectPool<GameObject>(
            CreateDoomShroomBoomPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        zombieCarBoomParticalPool = new ObjectPool<GameObject>(
            CreateZombieCarBoomPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        hypnoParticalPool = new ObjectPool<GameObject>(
            CreateHypnoPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dirtSmallParticalPool = new ObjectPool<GameObject>(
            CreateDirtSmallPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dirtBigParticalPool = new ObjectPool<GameObject>(
            CreateDirtBigPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        rockSmallParticalPool = new ObjectPool<GameObject>(
            CreateRockSmallPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        waterFallParticalPool = new ObjectPool<GameObject>(
            CreateWaterFallPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 坚果墙轻伤粒子池：基础对象池配置
        wallnutHurtSmallParticalPool = new ObjectPool<GameObject>(
            CreateWallnutHurtSmallPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 坚果墙重伤粒子池：基础对象池配置
        wallnutHurtLargeParticalPool = new ObjectPool<GameObject>(
            CreateWallnutHurtLargePartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 豌豆子弹池：获取时需额外重置子弹状态（避免复用旧状态）
        peaBulletPool = new ObjectPool<GameObject>(
            CreatePeaBullet,
            ActionOnGetPeaBullet,  // 子弹专属获取回调（重置计时器等）
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        puffPeaBulletPool = new ObjectPool<GameObject>(
            CreatePuffPeaBullet,
            ActionOnGetPeaBullet,  // 子弹专属获取回调（重置计时器等）
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 寒冰豌豆子弹池：配置同普通豌豆子弹池
        snowPeaBulletPool = new ObjectPool<GameObject>(
            CreateSnowPeaBullet,
            ActionOnGetPeaBullet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 寒冰豌豆子弹池：配置同普通豌豆子弹池
        firePeaBulletPool = new ObjectPool<GameObject>(
            CreateFirePeaBullet,
            ActionOnGetPeaBullet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 阳光池：获取时需重置阳光状态（生命周期、点击状态等）
        sunPool = new ObjectPool<GameObject>(
            CreateSun,
            ActionOnGetSun,  // 阳光专属获取回调
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        smallSunPool = new ObjectPool<GameObject>(
            CreateSmallSun,
            ActionOnGetSun,  // 阳光专属获取回调
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 头部发射粒子池：基础对象池配置
        headEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 手部发射粒子池：基础对象池配置
        handEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 音频源池：基础对象池配置（动态创建音频源）
        sourcePool = new ObjectPool<GameObject>(
            CreateSource,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 撑杆僵尸头部发射粒子池：基础对象池配置
        poleHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreatePoleHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 撑杆僵尸手部发射粒子池：基础对象池配置
        poleHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreatePoleHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 路障僵尸发射粒子池：基础对象池配置
        coneEmissionParticalPool = new ObjectPool<GameObject>(
            CreateConeEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 铁桶僵尸发射粒子池：基础对象池配置
        bucketEmissionParticalPool = new ObjectPool<GameObject>(
            CreateBucketEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 旗帜僵尸发射粒子池：初始容量2（出现频率低），最大300
        flagEmissionParticalPool = new ObjectPool<GameObject>(
            CreateFlagEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // 僵尸被炸飞动画池：基础对象池配置
        zombieBoomSwfPool = new ObjectPool<GameObject>(
            CreateZombieBoomSwf,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        zombieZamboniBoomSwfPool = new ObjectPool<GameObject>(
            CreateZombieZamboniBoomSwf,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        firePeaBulletSwfPool = new ObjectPool<GameObject>(
           CreateFirePeaBulletSwf,
           ActionOnGet,
           ActionOnRelease,
           ActionOnDestroy,
           true, 10, 300
        );

        fallWaterSwfPool = new ObjectPool<GameObject>(
           CreateFallWaterSwf,
           ActionOnGet,
           ActionOnRelease,
           ActionOnDestroy,
           true, 10, 300
        );

        doorEmissionParticalPool = new ObjectPool<GameObject>(
            CreateDoorEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        footballHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateFootballHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        footballHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateFootballHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        footballHelmetEmissionParticalPool = new ObjectPool<GameObject>(
            CreateFootballHelmetEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        paperHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreatePaperHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        paperHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreatePaperHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dancerHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateDancerHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dancerHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateDancerHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        jaksonHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateJaksonHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        jaksonHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateJaksonHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        bobsledHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateBobsledHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        bobsledHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateBobsledHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dolphinriderHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateDolphinriderHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        dolphinriderHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateDolphinriderHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        snorkleHeadEmissionParticalPool = new ObjectPool<GameObject>(
            CreateSnorkleHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        snorkleHandEmissionParticalPool = new ObjectPool<GameObject>(
            CreateSnorkleHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        paperEmissionParticalPool = new ObjectPool<GameObject>(
            CreatePaperEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

    }

    // 创建豌豆子弹实例（对象池调用，通过预制体生成新子弹）
    GameObject CreatePeaBullet() {
        return Instantiate(peaBullet);
    }

    GameObject CreatePuffPeaBullet() {
        return Instantiate(puffPeaBullet);
    }

    // 创建寒冰豌豆子弹实例
    GameObject CreateSnowPeaBullet() {
        return Instantiate(snowPeaBullet);
    }

    // 创建寒冰豌豆子弹实例
    GameObject CreateFirePeaBullet() {
        return Instantiate(firePeaBullet);
    }

    // 创建土豆地雷爆炸粒子实例
    GameObject CreatePotatoBoomPartical() {
        return Instantiate(potatoBoomPartical);
    }

    GameObject CreateIceShroomBoomPartical() {
        return Instantiate(iceShroomBoomPartical);
    }

    GameObject CreateIceCrackPartical() {
        return Instantiate(iceCrackPartical);
    }

    GameObject CreateDoomShroomBoomPartical() {
        return Instantiate(doomShroomBoomPartical);
    }

    GameObject CreateZombieCarBoomPartical() {
        return Instantiate(zombieCarBoomPartical);
    }

    GameObject CreateHypnoPartical() {
        return Instantiate(hypnoPartical);
    }

    GameObject CreateDirtSmallPartical() {
        return Instantiate(dirtSmallPartical);
    }

    GameObject CreateDirtBigPartical() {
        return Instantiate(dirtBigPartical);
    }

    GameObject CreateRockSmallPartical() {
        return Instantiate(rockSmallPartical);
    }

    GameObject CreateWaterFallPartical() {
        return Instantiate(waterFallPartical);
    }

    // 创建樱桃炸弹爆炸粒子实例
    GameObject CreateCherryBombBoomPartical() {
        return Instantiate(cherryBombBoomPartical);
    }

    // 创建坚果墙轻伤粒子实例
    GameObject CreateWallnutHurtSmallPartical() {
        return Instantiate(wallnutHurtSmallPartical);
    }

    // 创建坚果墙重伤粒子实例
    GameObject CreateWallnutHurtLargePartical() {
        return Instantiate(wallnutHurtLargePartical);
    }

    // 创建豌豆子弹命中粒子实例
    GameObject CreatePeaBulletPartical() {
        return Instantiate(peaBulletPartical);
    }

    GameObject CreatePuffPeaBulletPartical() {
        return Instantiate(puffPeaBulletPartical);
    }

    GameObject CreateFumeAttackPartical() {
        return Instantiate(fumeAttackPartical);
    }

    // 创建寒冰豌豆子弹命中粒子实例
    GameObject CreateSnowPeaBulletPartical() {
        return Instantiate(snowPeaBulletPartical);
    }

    // 创建阳光实例
    GameObject CreateSun() {
        return Instantiate(sun);
    }

    GameObject CreateSmallSun() {
        return Instantiate(smallSun);
    }

    // 创建头部发射粒子实例（emission：发射）
    GameObject CreateHeadEmissionPartical() {
        return Instantiate(headEmissionPartical);
    }

    // 创建手部发射粒子实例（emission：发射）
    GameObject CreateHandEmissionPartical() {
        return Instantiate(handEmissionPartical);
    }

    // 创建撑杆僵尸头部发射粒子实例
    GameObject CreatePoleHeadEmissionPartical() {
        return Instantiate(poleHeadEmissionPartical);
    }

    // 创建撑杆僵尸手部发射粒子实例
    GameObject CreatePoleHandEmissionPartical() {
        return Instantiate(poleHandEmissionPartical);
    }

    // 创建路障僵尸发射粒子实例
    GameObject CreateConeEmissionPartical() {
        return Instantiate(coneEmissionPartical);
    }

    // 创建铁桶僵尸发射粒子实例
    GameObject CreateBucketEmissionPartical() {
        return Instantiate(bucketEmissionPartical);
    }

    // 创建旗帜僵尸发射粒子实例
    GameObject CreateFlagEmissionPartical() {
        return Instantiate(flagEmissionPartical);
    }

    GameObject CreateDoorEmissionPartical() {
        return Instantiate(doorEmissionPartical);
    }

    // 创建僵尸被炸飞动画实例
    GameObject CreateZombieBoomSwf() {
        return Instantiate(zombieBoomSwf);
    }

    GameObject CreateZombieZamboniBoomSwf() {
        return Instantiate(zombieZamboniBoomSwf);
    }

    GameObject CreateFirePeaBulletSwf() {
        return Instantiate(firePeaBulletSwf);
    }

    GameObject CreateFallWaterSwf() {
        return Instantiate(fallWaterSwf);
    }

    GameObject CreateFootballHeadEmissionPartical() {
        return Instantiate(footballHeadEmissionPartical);
    }

    GameObject CreateFootballHandEmissionPartical() {
        return Instantiate(footballHandEmissionPartical);
    }

    GameObject CreateFootballHelmetEmissionPartical() {
        return Instantiate(footballHelmetEmissionPartical);
    }

    GameObject CreatePaperHeadEmissionPartical() {
        return Instantiate(paperHeadEmissionPartical);
    }

    GameObject CreatePaperHandEmissionPartical() {
        return Instantiate(paperHandEmissionPartical);
    }

    GameObject CreateDancerHeadEmissionPartical() {
        return Instantiate(dancerHeadEmissionPartical);
    }

    GameObject CreateDancerHandEmissionPartical() {
        return Instantiate(dancerHandEmissionPartical);
    }

    GameObject CreateJaksonHeadEmissionPartical() {
        return Instantiate(jaksonHeadEmissionPartical);
    }

    GameObject CreateJaksonHandEmissionPartical() {
        return Instantiate(jaksonHandEmissionPartical);
    }

    GameObject CreateBobsledHeadEmissionPartical() {
        return Instantiate(bobsledHeadEmissionPartical);
    }

    GameObject CreateBobsledHandEmissionPartical() {
        return Instantiate(bobsledHandEmissionPartical);
    }

    GameObject CreateDolphinriderHeadEmissionPartical() {
        return Instantiate(dolphinriderHeadEmissionPartical);
    }

    GameObject CreateDolphinriderHandEmissionPartical() {
        return Instantiate(dolphinriderHandEmissionPartical);
    }

    GameObject CreateSnorkleHeadEmissionPartical() {
        return Instantiate(snorkleHeadEmissionPartical);
    }

    GameObject CreateSnorkleHandEmissionPartical() {
        return Instantiate(snorkleHandEmissionPartical);
    }

    GameObject CreatePaperEmissionPartical() {
        return Instantiate(paperEmissionPartical);
    }

    // 创建音频源实例（动态添加AudioSource组件，无需预制体）
    GameObject CreateSource() {
        GameObject obj = new GameObject("AudioSource"); // 命名对象便于调试
        obj.AddComponent<AudioSource>();                // 为对象添加音频播放组件
        return obj;
    }

    // 基础获取回调：从池里获取对象时激活对象（通用逻辑）
    void ActionOnGet(GameObject obj) {
        obj.SetActive(true);
    }

    // 豌豆子弹获取回调：从池获取子弹时，重置状态避免复用旧数据
    void ActionOnGetPeaBullet(GameObject obj) {
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.liveTimer = 0;    // 重置生命周期计时器（防止刚取出就超时消失）
        peaBullet.isAttack = false; // 重置攻击状态（确保能重新检测命中）
        peaBullet.isRelease = false;// 重置回收标记（避免被误判为已回收）
        obj.SetActive(true);        // 激活子弹对象
    }

    // 阳光获取回调：从池获取阳光时，重置状态确保正常交互
    void ActionOnGetSun(GameObject obj) {
        Sun sun = obj.GetComponent<Sun>();
        sun.liveTimer = 0;          // 重置生命周期计时器（防止刚生成就消失）
        sun.isClick = false;        // 重置点击状态（确保能被玩家重新点击收集）
        sun.sunCollider2D.enabled = true; // 启用碰撞体（允许玩家点击交互）
        obj.SetActive(true);        // 激活阳光对象
    }

    // 基础回收回调：对象回收至池时禁用（保留对象实例，留待下次复用）
    void ActionOnRelease(GameObject obj) {
        obj.SetActive(false);
    }

    // 对象销毁回调：当对象数量超出池最大容量时，彻底销毁对象释放内存
    void ActionOnDestroy(GameObject obj) {
        Destroy(obj);
    }

    // 对外提供：获取普通豌豆子弹（如豌豆射手发射时调用）
    public GameObject GetPeaBullet() {
        return peaBulletPool.Get();
    }

    // 对外提供：回收普通豌豆子弹（如子弹命中目标或超时后调用）
    public void ReleasePeaBullet(GameObject gameObject) {
        if (gameObject.GetComponent<PeaBullet>().isRelease == true) return;
        
        peaBulletPool.Release(gameObject);              // 将子弹回收到对象池
        gameObject.GetComponent<PeaBullet>().isRelease = true; // 标记子弹为已回收状态
    }

    public GameObject GetPuffPeaBullet() {
        return puffPeaBulletPool.Get();
    }

    public void ReleasePuffPeaBullet(GameObject gameObject) {
        puffPeaBulletPool.Release(gameObject);              // 将子弹回收到对象池
        gameObject.GetComponent<PeaBullet>().isRelease = true; // 标记子弹为已回收状态
    }

    // 对外提供：获取寒冰豌豆子弹（如寒冰射手发射时调用）
    public GameObject GetSnowPeaBullet() {
        return snowPeaBulletPool.Get();
    }

    // 对外提供：回收寒冰豌豆子弹（如子弹命中目标或超时后调用）
    public void ReleaseSnowPeaBullet(GameObject gameObject) {
        snowPeaBulletPool.Release(gameObject);              // 将子弹回收到对象池
        gameObject.GetComponent<PeaBullet>().isRelease = true; // 标记子弹为已回收状态
    }
    public GameObject GetFirePeaBullet() {
        return firePeaBulletPool.Get();
    }

    public void ReleaseFirePeaBullet(GameObject gameObject) {
        firePeaBulletPool.Release(gameObject);              // 将子弹回收到对象池
        gameObject.GetComponent<PeaBullet>().isRelease = true; // 标记子弹为已回收状态
    }

    // 对外提供：获取普通豌豆子弹命中粒子（如子弹命中僵尸时调用）
    public GameObject GetPeaBulletPartical() {
        return peaBulletParticalPool.Get();
    }

    // 对外提供：回收普通豌豆子弹命中粒子（粒子效果播放完成后调用）
    public void ReleasePeaBulletPartical(GameObject gameObject) {
        peaBulletParticalPool.Release(gameObject);
    }

    public GameObject GetPuffPeaBulletPartical() {
        return puffPeaBulletParticalPool.Get();
    }

    public void ReleasePuffPeaBulletPartical(GameObject gameObject) {
        puffPeaBulletParticalPool.Release(gameObject);
    }

    // 对外提供：获取寒冰豌豆子弹命中粒子（如子弹命中僵尸时调用）
    public GameObject GetSnowPeaBulletPartical() {
        return snowPeaBulletParticalPool.Get();
    }

    // 对外提供：回收寒冰豌豆子弹命中粒子（粒子效果播放完成后调用）
    public void ReleaseSnowPeaBulletPartical(GameObject gameObject) {
        snowPeaBulletParticalPool.Release(gameObject);
    }

    public GameObject GetFumeAttackPartical() {
        return fumeAttackParticalPool.Get();
    }

    public void ReleaseFumeAttackPartical(GameObject gameObject) {
        fumeAttackParticalPool.Release(gameObject);
    }

    // 对外提供：获取阳光对象（如向日葵生成阳光时调用）
    public GameObject GetSun() {
        return sunPool.Get();
    }

    // 对外提供：回收阳光对象（如阳光被收集或超时消失后调用）
    public void ReleaseSun(GameObject gameObject) {
        sunPool.Release(gameObject);
    }

    // 对外提供：获取阳光对象（如向日葵生成阳光时调用）
    public GameObject GetSmallSun() {
        return smallSunPool.Get();
    }

    // 对外提供：回收阳光对象（如阳光被收集或超时消失后调用）
    public void ReleaseSmallSun(GameObject gameObject) {
        smallSunPool.Release(gameObject);
    }

    // 对外提供：获取头部发射粒子（如普通僵尸攻击时调用）
    public GameObject GetHeadEmissionPartical() {
        return headEmissionParticalPool.Get();
    }

    // 对外提供：回收头部发射粒子（粒子效果播放完成后调用）
    public void ReleaseHeadEmissionPartical(GameObject gameObject) {
        headEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取手部发射粒子（如普通僵尸攻击时调用）
    public GameObject GetHandEmissionPartical() {
        return handEmissionParticalPool.Get();
    }

    // 对外提供：回收手部发射粒子（粒子效果播放完成后调用）
    public void ReleaseHandEmissionPartical(GameObject gameObject) {
        handEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取音频源对象（如播放音效时调用）
    public GameObject GetSource() {
        return sourcePool.Get();
    }

    // 对外提供：回收音频源对象（如音效播放完成后调用）
    public void ReleaseSource(GameObject gameObject) {
        sourcePool.Release(gameObject);
    }

    // 对外提供：获取撑杆僵尸头部发射粒子（如撑杆僵尸攻击时调用）
    public GameObject GetPoleHeadEmissionPartical() {
        return poleHeadEmissionParticalPool.Get();
    }

    // 对外提供：回收撑杆僵尸头部发射粒子（粒子效果播放完成后调用）
    public void ReleasePoleHeadEmissionPartical(GameObject gameObject) {
        poleHeadEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取撑杆僵尸手部发射粒子（如撑杆僵尸攻击时调用）
    public GameObject GetPoleHandEmissionPartical() {
        return poleHandEmissionParticalPool.Get();
    }

    // 对外提供：回收撑杆僵尸手部发射粒子（粒子效果播放完成后调用）
    public void ReleasePoleHandEmissionPartical(GameObject gameObject) {
        poleHandEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取路障僵尸发射粒子（如路障僵尸攻击时调用）
    public GameObject GetConeEmissionPartical() {
        return coneEmissionParticalPool.Get();
    }

    // 对外提供：回收路障僵尸发射粒子（粒子效果播放完成后调用）
    public void ReleaseConeEmissionPartical(GameObject gameObject) {
        coneEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取铁桶僵尸发射粒子（如铁桶僵尸攻击时调用）
    public GameObject GetBucketEmissionPartical() {
        return bucketEmissionParticalPool.Get();
    }

    // 对外提供：回收铁桶僵尸发射粒子（粒子效果播放完成后调用）
    public void ReleaseBucketEmissionPartical(GameObject gameObject) {
        bucketEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取旗帜僵尸发射粒子（如旗帜僵尸攻击时调用）
    public GameObject GetFlagEmissionPartical() {
        return flagEmissionParticalPool.Get();
    }

    public void ReleaseDoorEmissionPartical(GameObject gameObject) {
        doorEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetDoorEmissionPartical() {
        return doorEmissionParticalPool.Get();
    }

    // 对外提供：回收旗帜僵尸发射粒子（粒子效果播放完成后调用）
    public void ReleaseFlagEmissionPartical(GameObject gameObject) {
        flagEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetFootballHeadEmissionPartical() {
        return footballHeadEmissionParticalPool.Get();
    }
    public void ReleaseFootballHeadEmissionPartical(GameObject gameObject) {
        footballHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetFootballHandEmissionPartical() {
        return footballHandEmissionParticalPool.Get();
    }
    public void ReleaseFootballHandEmissionPartical(GameObject gameObject) {
        footballHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetFootballHelmetEmissionPartical() {
        return footballHelmetEmissionParticalPool.Get();
    }
    public void ReleaseFootballHelmetEmissionPartical(GameObject gameObject) {
        footballHelmetEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetPaperHeadEmissionPartical() {
        return paperHeadEmissionParticalPool.Get();
    }
    public void ReleasePaperHeadEmissionPartical(GameObject gameObject) {
        paperHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetPaperHandEmissionPartical() {
        return paperHandEmissionParticalPool.Get();
    }
    public void ReleasePaperHandEmissionPartical(GameObject gameObject) {
        paperHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetDancerHeadEmissionPartical() {
        return dancerHeadEmissionParticalPool.Get();
    }
    public void ReleaseDancerHeadEmissionPartical(GameObject gameObject) {
        dancerHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetJaksonHeadEmissionPartical() {
        return jaksonHeadEmissionParticalPool.Get();
    }
    public void ReleaseJaksonHeadEmissionPartical(GameObject gameObject) {
        jaksonHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetBobsledHeadEmissionPartical() {
        return bobsledHeadEmissionParticalPool.Get();
    }
    public void ReleaseBobsledHeadEmissionPartical(GameObject gameObject) {
        bobsledHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetDolphinriderHeadEmissionPartical() {
        return dolphinriderHeadEmissionParticalPool.Get();
    }
    public void ReleaseDolphinriderHeadEmissionPartical(GameObject gameObject) {
        dolphinriderHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetSnorkleHeadEmissionPartical() {
        return snorkleHeadEmissionParticalPool.Get();
    }
    public void ReleaseSnorkleHeadEmissionPartical(GameObject gameObject) {
        snorkleHeadEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetDancerHandEmissionPartical() {
        return dancerHandEmissionParticalPool.Get();
    }
    public void ReleaseDancerHandEmissionPartical(GameObject gameObject) {
        dancerHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetJaksonHandEmissionPartical() {
        return jaksonHandEmissionParticalPool.Get();
    }
    public void ReleaseJaksonHandEmissionPartical(GameObject gameObject) {
        jaksonHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetBobsledHandEmissionPartical() {
        return bobsledHandEmissionParticalPool.Get();
    }
    public void ReleaseBobsledHandEmissionPartical(GameObject gameObject) {
        bobsledHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetDolphinriderHandEmissionPartical() {
        return dolphinriderHandEmissionParticalPool.Get();
    }
    public void ReleaseDolphinriderHandEmissionPartical(GameObject gameObject) {
        dolphinriderHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetSnorkleHandEmissionPartical() {
        return snorkleHandEmissionParticalPool.Get();
    }
    public void ReleaseSnorkleHandEmissionPartical(GameObject gameObject) {
        snorkleHandEmissionParticalPool.Release(gameObject);
    }

    public GameObject GetPaperEmissionPartical() {
        return paperEmissionParticalPool.Get();
    }
    public void ReleasePaperEmissionPartical(GameObject gameObject) {
        paperEmissionParticalPool.Release(gameObject);
    }

    // 对外提供：获取土豆地雷爆炸粒子（如土豆地雷触发爆炸时调用）
    public GameObject GetPotatoBoomPartical() {
        return potatoBoomParticalPool.Get();
    }

    // 对外提供：回收土豆地雷爆炸粒子（爆炸效果播放完成后调用）
    public void ReleasePotatoBoomPartical(GameObject gameObject) {
        potatoBoomParticalPool.Release(gameObject);
    }

    public GameObject GetIceShroomBoomPartical() {
        return iceShroomBoomParticalPool.Get();
    }

    // 对外提供：回收土豆地雷爆炸粒子（爆炸效果播放完成后调用）
    public void ReleaseIceShroomBoomPartical(GameObject gameObject) {
        iceShroomBoomParticalPool.Release(gameObject);
    }

    public GameObject GetIceCrackPartical() {
        return iceCrackParticalPool.Get();
    }

    public void ReleaseIceCrackPartical(GameObject gameObject) {
        iceCrackParticalPool.Release(gameObject);
    }

    public GameObject GetDoomShroomBoomPartical() {
        return doomShroomBoomParticalPool.Get();
    }

    public void ReleaseDoomShroomBoomPartical(GameObject gameObject) {
        doomShroomBoomParticalPool.Release(gameObject);
    }

    public GameObject GetZombieCarBoomPartical() {
        return zombieCarBoomParticalPool.Get();
    }

    public void ReleaseZombieCarBoomPartical(GameObject gameObject) {
        zombieCarBoomParticalPool.Release(gameObject);
    }

    public GameObject GetHypnoPartical() {
        return hypnoParticalPool.Get();
    }

    public void ReleaseHypnoPartical(GameObject gameObject) {
        hypnoParticalPool.Release(gameObject);
    }

    public GameObject GetDirtSmallPartical() {
        return dirtSmallParticalPool.Get();
    }

    public void ReleaseDirtSmallPartical(GameObject gameObject) {
        dirtSmallParticalPool.Release(gameObject);
    }

    public GameObject GetDirtBigPartical() {
        return dirtBigParticalPool.Get();
    }

    public void ReleaseDirtBigPartical(GameObject gameObject) {
        dirtBigParticalPool.Release(gameObject);
    }

    public GameObject GetRockSmallPartical() {
        return rockSmallParticalPool.Get();
    }

    public void ReleaseRockSmallPartical(GameObject gameObject) {
        rockSmallParticalPool.Release(gameObject);
    }

    public GameObject GetWaterFallPartical() {
        return waterFallParticalPool.Get();
    }

    public void ReleaseWaterFallPartical(GameObject gameObject) {
        waterFallParticalPool.Release(gameObject);
    }

    // 对外提供：获取坚果墙轻伤粒子（如坚果墙受轻微攻击时调用）
    public GameObject GetWallnutHurtSmallPartical() {
        return wallnutHurtSmallParticalPool.Get();
    }

    // 对外提供：回收坚果墙轻伤粒子（粒子效果播放完成后调用）
    public void ReleaseWallnutHurtSmallPartical(GameObject gameObject) {
        wallnutHurtSmallParticalPool.Release(gameObject);
    }

    // 对外提供：获取坚果墙重伤粒子（如坚果墙受严重攻击或被摧毁时调用）
    public GameObject GetWallnutHurtLargePartical() {
        return wallnutHurtLargeParticalPool.Get();
    }

    // 对外提供：回收坚果墙重伤粒子（粒子效果播放完成后调用）
    public void ReleaseWallnutHurtLargePartical(GameObject gameObject) {
        wallnutHurtLargeParticalPool.Release(gameObject);
    }

    // 对外提供：获取僵尸被炸飞动画（如僵尸被爆炸类植物击杀时调用）
    public GameObject GetZombieBoomSwf() {
        return zombieBoomSwfPool.Get();
    }

    // 对外提供：回收僵尸被炸飞动画（动画播放完成后调用）
    public void ReleaseZombieBoomSwf(GameObject gameObject) {
        zombieBoomSwfPool.Release(gameObject);
    }

    // 对外提供：获取僵尸被炸飞动画（如僵尸被爆炸类植物击杀时调用）
    public GameObject GetZombieZamboniBoomSwf() {
        return zombieZamboniBoomSwfPool.Get();
    }

    // 对外提供：回收僵尸被炸飞动画（动画播放完成后调用）
    public void ReleaseZombieZamboniBoomSwf(GameObject gameObject) {
        zombieZamboniBoomSwfPool.Release(gameObject);
    }

    public GameObject GetFirePeaBulletSwf() {
        return firePeaBulletSwfPool.Get();
    }

    public void ReleaseFirePeaBulletSwf(GameObject gameObject) {
        firePeaBulletSwfPool.Release(gameObject);
    }

    public GameObject GetFallWaterSwf() {
        return fallWaterSwfPool.Get();
    }

    public void ReleaseFallWaterSwf(GameObject gameObject) {
        fallWaterSwfPool.Release(gameObject);
    }

    // 对外提供：获取樱桃炸弹爆炸粒子（如樱桃炸弹触发爆炸时调用）
    public GameObject GetCherryBombBoomPartical() {
        return cherryBombBoomParticalPool.Get();
    }

    // 对外提供：回收樱桃炸弹爆炸粒子（爆炸效果播放完成后调用）
    public void ReleaseCherryBombBoomPartical(GameObject gameObject) {
        cherryBombBoomParticalPool.Release(gameObject);
    }

    // 对外提供：播放僵尸被炸飞动画（启动协程控制播放逻辑）
    // 参数：动画位置、僵尸是否有头（切换对应动画）、渲染层级（避免遮挡）
    public void PlayZombieBoomSwfIEnumrator(Transform transform, bool isHaveHead, int sort) {
        StartCoroutine(PlayZombieBoomSwf(transform, isHaveHead, sort));
    }

    public void PlayZombieZamboniBoomSwfIEnumrator(Transform transform, int sort) {
        StartCoroutine(PlayZombieZamboniBoomSwf(transform, sort));
    }

    public void PlayFirePeaBulletSwfIEnumrator(Transform transform) {
        StartCoroutine(PlayFirePeaBulletSwf(transform));
    }

    public void PlayFallWaterSwfIEnumrator(Transform transform) {
        StartCoroutine(PlayFallWaterSwf(transform));
    }

    // 对外提供：播放普通豌豆子弹命中粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如子弹命中点）
    public void PlayPeaBulletParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayPeaBulletPartical(transform));
    }

    public void PlayPuffPeaBulletParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayPuffPeaBulletPartical(transform));
    }

    public void PlayFumeAttackParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayFumeAttackPartical(transform));
    }

    // 对外提供：播放寒冰豌豆子弹命中粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如子弹命中点）
    public void PlaySnowPeaBulletParticalIEnumrator(Transform transform) {
        StartCoroutine(PlaySnowPeaBulletPartical(transform));
    }

    // 对外提供：播放普通僵尸头部发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色（如冰冻效果）
    public void PlayHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放普通僵尸手部发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayDoorEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayDoorEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayFootballHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayFootballHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayFootballHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayFootballHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayFootballHelmetEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayFootballHelmetEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayPaperHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayPaperHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayPaperHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayPaperHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayDancerHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayDancerHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayDancerHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayDancerHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayJaksonHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayJaksonHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayJaksonHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayJaksonHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayBobsledHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayBobsledHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayBobsledHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayBobsledHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayDolphinriderHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayDolphinriderHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayDolphinriderHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayDolphinriderHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlaySnorkleHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlaySnorkleHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlaySnorkleHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlaySnorkleHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    public void PlayPaperEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayPaperEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放撑杆僵尸头部发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayPoleHeadEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayPoleHeadEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放撑杆僵尸手部发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayPoleHandEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayPoleHandEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放路障僵尸发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayConeEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayConeEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放铁桶僵尸发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayBucketEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayBucketEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放旗帜僵尸发射粒子（启动协程控制播放逻辑）
    // 参数：粒子位置、渲染层级、是否使用特殊颜色
    public void PlayFlagEmissionIEnumrator(Transform transform, int sort, bool isCroze, bool isHypno) {
        StartCoroutine(PlayFlagEmissionPartical(transform, sort, isCroze, isHypno));
    }

    // 对外提供：播放土豆地雷爆炸粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如地雷位置）
    public void PlayPotatoBoomParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayPotatoBoomPartical(transform));
    }

    public void PlayIceShroomBoomParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayIceShroomBoomPartical(transform));
    }

    public void PlayIceCrackParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayIceCrackPartical(transform));
    }

    public void PlayDoomShroomBoomParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayDoomShroomBoomPartical(transform));
    }

    public void PlayZombieCarBoomParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayZombieCarBoomPartical(transform));
    }

    public void PlayHypnoParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayHypnoPartical(transform));
    }

    public void PlayDirtSmallParticalIEnumrator(Vector3 position) {
        StartCoroutine(PlayDirtSmallPartical(position));
    }

    public void PlayDirtBigParticalIEnumrator(Vector3 position) {
        StartCoroutine(PlayDirtBigPartical(position));
    }

    public void PlayRockSmallParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayRockSmallPartical(transform));
    }

    public void PlayWaterFallParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayWaterFallPartical(transform));
    }

    // 对外提供：播放樱桃炸弹爆炸粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如樱桃炸弹位置）
    public void PlayCherryBombBoomParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayCherryBombBoomPartical(transform));
    }

    // 对外提供：播放坚果墙轻伤粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如坚果墙位置）
    public void PlayWallnutHurtSmallParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayWallnutHurtSmallPartical(transform));
    }

    // 对外提供：播放坚果墙重伤粒子（启动协程控制播放逻辑）
    // 参数：粒子播放位置（如坚果墙位置）
    public void PlayWallnutHurtLargeParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayWallnutHurtLargePartical(transform));
    }

    // 僵尸被炸飞动画播放协程（控制动画播放流程）
    // 参数：动画位置、僵尸是否有头（切换动画片段）、渲染层级
    public IEnumerator PlayZombieBoomSwf(Transform transform, bool isHaveHead, int sort) {
        GameObject obj = GetZombieBoomSwf(); // 从池获取动画对象
        obj.transform.position = transform.position; // 设置动画播放位置

        // 获取动画控制器和渲染组件
        SwfClipController[] swfClipControllers = obj.GetComponentsInChildren<SwfClipController>();
        SwfClip[] swfClips = obj.GetComponentsInChildren<SwfClip>();

        // 设置所有动画片段的渲染层级（确保正确显示层级）
        foreach (SwfClip swfClip in swfClips) {
            swfClip.sortingOrder = sort;
        }

        // 根据僵尸是否有头，播放对应动画片段
        if (isHaveHead) {
            swfClipControllers[1].gameObject.SetActive(false); // 禁用无头动画
            swfClipControllers[0].Play(true); // 播放有头动画
        }
        else {
            swfClipControllers[0].gameObject.SetActive(false); // 禁用有头动画
            swfClipControllers[1].Play(true); // 播放无头动画
        }

        yield return new WaitForSeconds(2.8f); // 等待动画播放完成（固定时长2.8秒）

        // 重置动画状态并回收对象
        swfClipControllers[0].gameObject.SetActive(true);
        swfClipControllers[1].gameObject.SetActive(true);
        ReleaseZombieBoomSwf(obj);
    }

    public IEnumerator PlayZombieZamboniBoomSwf(Transform transform, int sort) {
        GameObject obj = GetZombieZamboniBoomSwf(); // 从池获取动画对象
        obj.transform.position = transform.position; // 设置动画播放位置

        // 获取动画控制器和渲染组件
        SwfClipController swfClipController = obj.GetComponentInChildren<SwfClipController>();
        SwfClip swfClip = obj.GetComponentInChildren<SwfClip>();
        swfClip.sortingOrder = sort;

        swfClipController.Play(true);

        yield return new WaitForSeconds(3.3f); // 等待动画播放完成（固定时长2.8秒）

        ReleaseZombieZamboniBoomSwf(obj);
    }

    public IEnumerator PlayFirePeaBulletSwf(Transform transform) {
        GameObject obj = GetFirePeaBulletSwf(); // 从池获取动画对象
        obj.transform.position = transform.position; // 设置动画播放位置

        // 获取动画控制器和渲染组件
        SwfClipController swfClipController = obj.GetComponentInChildren<SwfClipController>();
        SwfClip swfClip = obj.GetComponentInChildren<SwfClip>();

        swfClip.currentFrame = 40;

        swfClipController.Play(false);

        yield return new WaitForSeconds(0.4f);

        ReleaseFirePeaBulletSwf(obj);
    }

    public IEnumerator PlayFallWaterSwf(Transform transform) {
        GameObject obj = GetFallWaterSwf(); // 从池获取动画对象
        
        Vector3 position = transform.position;
        position.x -= 0.5f;
        
        obj.transform.position = position; // 设置动画播放位置

        PlayWaterFallParticalIEnumrator(transform);

        // 获取动画控制器和渲染组件
        SwfClipController swfClipController = obj.GetComponentInChildren<SwfClipController>();

        swfClipController.Play(true);

        yield return new WaitForSeconds(0.7f);

        ReleaseFallWaterSwf(obj);
    }

    // 樱桃炸弹爆炸粒子播放协程（控制单粒子系统的播放与回收）
    // 参数：transform - 粒子播放的目标位置（樱桃炸弹的位置）
    public IEnumerator PlayCherryBombBoomPartical(Transform transform) {
        GameObject obj = GetCherryBombBoomPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        obj.transform.position = transform.position;        // 将粒子位置设置为爆炸发生的位置
        var partical = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件，用于控制播放逻辑

        partical.Play();                                    // 启动粒子播放（触发爆炸效果）

        // 等待粒子播放完成：根据粒子系统自身配置的"总时长"等待，确保效果完整显示
        yield return new WaitForSeconds(partical.main.duration);

        partical.Clear();                                   // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseCherryBombBoomPartical(obj);                  // 将粒子对象回收到池，供下次爆炸时复用
    }

    // 土豆地雷爆炸粒子播放协程（控制多粒子组合系统的播放与回收）
    // 参数：transform - 粒子播放的目标位置（土豆地雷的位置）
    public IEnumerator PlayPotatoBoomPartical(Transform transform) {
        GameObject obj = GetPotatoBoomPartical();       // 从对象池获取土豆地雷爆炸粒子对象
        // 获取对象下所有粒子系统（含子物体，支持多粒子叠加的爆炸效果），true表示包含非激活状态的组件
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }
        ReleasePotatoBoomPartical(obj);                  // 将粒子组合对象回收到池
    }

    public IEnumerator PlayIceShroomBoomPartical(Transform transform) {
        GameObject obj = GetIceShroomBoomPartical();       // 从对象池获取土豆地雷爆炸粒子对象
        // 获取对象下所有粒子系统（含子物体，支持多粒子叠加的爆炸效果），true表示包含非激活状态的组件
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }
        ReleaseIceShroomBoomPartical(obj);                  // 将粒子组合对象回收到池
    }

    public IEnumerator PlayIceCrackPartical(Transform transform) {
        GameObject obj = GetIceCrackPartical();       // 从对象池获取土豆地雷爆炸粒子对象
        // 获取对象下所有粒子系统（含子物体，支持多粒子叠加的爆炸效果），true表示包含非激活状态的组件
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }
        ReleaseIceCrackPartical(obj);                  // 将粒子组合对象回收到池
    }

    public IEnumerator PlayDoomShroomBoomPartical(Transform transform) {
        GameObject obj = GetDoomShroomBoomPartical();       // 从对象池获取土豆地雷爆炸粒子对象
        // 获取对象下所有粒子系统（含子物体，支持多粒子叠加的爆炸效果），true表示包含非激活状态的组件
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }
        ReleaseDoomShroomBoomPartical(obj);                  // 将粒子组合对象回收到池
    }

    public IEnumerator PlayZombieCarBoomPartical(Transform transform) {
        GameObject obj = GetZombieCarBoomPartical();       // 从对象池获取土豆地雷爆炸粒子对象
        // 获取对象下所有粒子系统（含子物体，支持多粒子叠加的爆炸效果），true表示包含非激活状态的组件
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(1.1f);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }
        ReleaseZombieCarBoomPartical(obj);                  // 将粒子组合对象回收到池
    }

    public IEnumerator PlayHypnoPartical(Transform transform) {
        GameObject obj = GetHypnoPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        obj.transform.position = transform.position;        // 将粒子位置设置为爆炸发生的位置
        var partical = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件，用于控制播放逻辑

        partical.Play();                                    // 启动粒子播放（触发爆炸效果）

        // 等待粒子播放完成：根据粒子系统自身配置的"总时长"等待，确保效果完整显示
        yield return new WaitForSeconds(partical.main.duration);

        partical.Clear();                                   // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseHypnoPartical(obj);                // 将粒子组合对象回收到池
    }

    public IEnumerator PlayDirtSmallPartical(Vector3 position) {
        GameObject obj = GetDirtSmallPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }                              // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseDirtSmallPartical(obj);                // 将粒子组合对象回收到池
    }

    public IEnumerator PlayDirtBigPartical(Vector3 position) {
        GameObject obj = GetDirtBigPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = position;    // 将粒子组合的位置设置为爆炸发生的位置

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }                              // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseDirtBigPartical(obj);                // 将粒子组合对象回收到池
    }

    public IEnumerator PlayRockSmallPartical(Transform transform) {
        GameObject obj = GetRockSmallPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        obj.transform.position = transform.position;        // 将粒子位置设置为爆炸发生的位置
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        obj.transform.position = transform.position;    // 将粒子组合的位置设置为爆炸发生的位置

        obj.transform.DOMoveY(obj.transform.position.y - 0.5f, 5f)
            .SetEase(Ease.Linear);

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }                          // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseRockSmallPartical(obj);                // 将粒子组合对象回收到池
    }

    public IEnumerator PlayWaterFallPartical(Transform transform) {
        GameObject obj = GetWaterFallPartical();       // 从对象池获取樱桃炸弹爆炸粒子对象
        obj.transform.position = transform.position;        // 将粒子位置设置为爆炸发生的位置
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);

        Vector3 position = transform.position;
        position.y -= 0.7f;
        position.x += 0.2f;

        obj.transform.position = position;

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }

        // 等待粒子播放完成：默认所有子粒子时长一致，取第一个粒子的时长作为等待依据
        yield return new WaitForSeconds(particals[0].main.duration);

        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }                          // 清除粒子系统中残留的粒子，避免下次复用时有残影

        ReleaseWaterFallPartical(obj);                // 将粒子组合对象回收到池
    }

    // 坚果墙轻伤粒子播放协程（控制多粒子组合系统的播放与回收）
    // 参数：transform - 粒子播放的目标位置（坚果墙的位置）
    public IEnumerator PlayWallnutHurtSmallPartical(Transform transform) {
        GameObject obj = GetWallnutHurtSmallPartical();       // 从对象池获取坚果墙轻伤粒子对象
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);  // 获取所有子粒子系统
        obj.transform.position = transform.position;            // 设置粒子位置为坚果墙位置

        foreach (var ps in particals) {
            ps.Play();                                          // 启动所有轻伤粒子（如碎片、灰尘效果）
        }

        yield return new WaitForSeconds(particals[0].main.duration);  // 等待轻伤效果播放完成

        foreach (var ps in particals) {
            ps.Clear();                                           // 清除残留粒子
        }
        ReleaseWallnutHurtSmallPartical(obj);                      // 回收粒子对象到池
    }

    // 坚果墙重伤粒子播放协程（控制多粒子组合系统的播放与回收）
    // 参数：transform - 粒子播放的目标位置（坚果墙的位置）
    public IEnumerator PlayWallnutHurtLargePartical(Transform transform) {
        GameObject obj = GetWallnutHurtLargePartical();       // 从对象池获取坚果墙重伤粒子对象
        var particals = obj.GetComponentsInChildren<ParticleSystem>(true);  // 获取所有子粒子系统
        obj.transform.position = transform.position;            // 设置粒子位置为坚果墙位置

        foreach (var ps in particals) {
            ps.Play();                                          // 启动所有重伤粒子（如大量碎片、裂纹效果）
        }

        yield return new WaitForSeconds(particals[0].main.duration);  // 等待重伤效果播放完成

        foreach (var ps in particals) {
            ps.Clear();                                           // 清除残留粒子
        }
        ReleaseWallnutHurtLargePartical(obj);                      // 回收粒子对象到池
    }

    // 豌豆子弹粒子播放协程：控制子弹命中粒子的播放（固定等待时长）
    // 参数：transform - 粒子播放的目标位置（子弹命中僵尸/障碍物的位置）
    public IEnumerator PlayPeaBulletPartical(Transform transform) {
        GameObject obj = GetPeaBulletPartical();       // 从对象池获取普通豌豆子弹命中粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        particle.transform.position = transform.position; // 设为子弹命中点位置，确保效果贴合碰撞处
        particle.Play();                                // 启动命中粒子（如火花、冲击效果）

        yield return new WaitForSeconds(0.6f);          // 固定等待0.6秒（命中效果较短，用固定时长更高效）

        particle.Clear();                               // 清除残留粒子
        ReleasePeaBulletPartical(obj);                  // 回收粒子对象到池
    }

    public IEnumerator PlayPuffPeaBulletPartical(Transform transform) {
        GameObject obj = GetPuffPeaBulletPartical();       // 从对象池获取普通豌豆子弹命中粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        particle.transform.position = transform.position; // 设为子弹命中点位置，确保效果贴合碰撞处
        particle.Play();                                // 启动命中粒子（如火花、冲击效果）

        yield return new WaitForSeconds(0.3f);          // 固定等待0.6秒（命中效果较短，用固定时长更高效）

        particle.Clear();                               // 清除残留粒子
        ReleasePuffPeaBulletPartical(obj);                  // 回收粒子对象到池
    }

    // 寒冰豌豆子弹粒子播放协程：控制寒冰子弹命中粒子的播放（固定等待时长）
    // 参数：transform - 粒子播放的目标位置（子弹命中点）
    public IEnumerator PlaySnowPeaBulletPartical(Transform transform) {
        GameObject obj = GetSnowPeaBulletPartical();       // 从对象池获取寒冰豌豆子弹命中粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        particle.transform.position = transform.position; // 设为子弹命中点位置
        particle.Play();                                // 启动寒冰命中粒子（如冰霜、白雾效果）

        yield return new WaitForSeconds(0.6f);          // 固定等待0.6秒，与普通豌豆命中效果时长保持一致

        particle.Clear();                               // 清除残留粒子
        ReleaseSnowPeaBulletPartical(obj);              // 回收粒子对象到池
    }

    public IEnumerator PlayFumeAttackPartical(Transform transform) {
        GameObject obj = GetFumeAttackPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件

        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitUntil(() => particle.isStopped && particle.particleCount == 0);

        particle.Clear();                               // 清除残留粒子
        ReleaseFumeAttackPartical(obj);               // 回收粒子对象到池
    }

    // 普通僵尸头部发射粒子播放协程：支持渲染层级与颜色切换的粒子控制
    // 参数：transform - 粒子位置（僵尸头部）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseHeadEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayFootballHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetFootballHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseFootballHeadEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayFootballHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetFootballHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseFootballHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayPaperHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetPaperHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleasePaperHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayPaperHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetPaperHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleasePaperHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayDancerHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetDancerHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseDancerHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayDancerHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetDancerHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseDancerHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayJaksonHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetJaksonHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseJaksonHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayJaksonHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetJaksonHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseJaksonHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayBobsledHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetBobsledHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseBobsledHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayBobsledHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetBobsledHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseBobsledHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayDolphinriderHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetDolphinriderHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseDolphinriderHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayDolphinriderHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetDolphinriderHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseDolphinriderHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlaySnorkleHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetSnorkleHeadEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseSnorkleHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlaySnorkleHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetSnorkleHandEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseSnorkleHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayPaperEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetPaperEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleasePaperEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayFootballHelmetEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetFootballHelmetEmissionPartical();      // 从对象池获取头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        // 获取粒子渲染组件，用于控制显示层级（避免被僵尸模型或其他元素遮挡）
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，确保粒子在正确的视觉层级显示
        particle.transform.position = transform.position; // 设为僵尸头部位置，贴合攻击动作
        particle.Play();                                // 启动头部发射粒子（如攻击闪光、唾沫效果）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        // 等待粒子自然播放完成：根据粒子自身配置的时长等待
        yield return new WaitForSeconds(particle.main.duration);

        mainModule.startColor = originColor;            // 恢复粒子原始颜色，确保下次复用正常
        particle.Clear();                               // 清除残留粒子
        ReleaseFootballHelmetEmissionPartical(obj);               // 回收粒子对象到池
    }

    // 普通僵尸手部发射粒子播放协程：支持渲染层级与颜色切换的粒子控制
    // 参数：transform - 粒子位置（僵尸手部）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetHandEmissionPartical();      // 从对象池获取手部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，避免遮挡
        particle.transform.position = transform.position; // 设为僵尸手部位置，贴合攻击动作
        particle.Play();                                // 启动手部发射粒子（如攻击闪光、挥拳特效）

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色（如冰冻状态）
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待粒子播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留粒子
        ReleaseHandEmissionPartical(obj);               // 回收粒子对象到池
    }

    public IEnumerator PlayDoorEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetDoorEmissionPartical();      // 从对象池获取手部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();  // 获取粒子系统组件
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级，避免遮挡
        particle.transform.position = transform.position; // 设为僵尸手部位置，贴合攻击动作
        particle.Play();                                // 启动手部发射粒子（如攻击闪光、挥拳特效）

        var mainModule = particle.main;                 // 获取粒子系统的"主模块"（控制颜色、时长等核心参数）
        Color originColor = mainModule.startColor.color; // 保存粒子原始颜色（避免复用后颜色被篡改）
        if (isCroze) mainModule.startColor = crozeColor; // 若需特殊颜色（如僵尸被冰冻时），切换为预设的crozeColor
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待粒子播放完成

        mainModule.startColor = originColor;
        particle.Clear();                               // 清除残留粒子
        ReleaseDoorEmissionPartical(obj);               // 回收粒子对象到池
    }

    // 撑杆僵尸头部发射粒子播放协程：支持渲染层级与颜色切换
    // 参数：transform - 粒子位置（撑杆僵尸头部）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayPoleHeadEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetPoleHeadEmissionPartical();  // 从对象池获取撑杆僵尸头部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级
        particle.transform.position = transform.position; // 设为撑杆僵尸头部位置
        particle.Play();                                // 启动头部发射粒子

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留
        ReleasePoleHeadEmissionPartical(obj);           // 回收对象
    }

    // 撑杆僵尸手部发射粒子播放协程：支持渲染层级与颜色切换
    // 参数：transform - 粒子位置（撑杆僵尸手部）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayPoleHandEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetPoleHandEmissionPartical();  // 从对象池获取撑杆僵尸手部发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级
        particle.transform.position = transform.position; // 设为撑杆僵尸手部位置
        particle.Play();                                // 启动手部发射粒子

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留
        ReleasePoleHandEmissionPartical(obj);           // 回收对象
    }

    // 路障僵尸发射粒子播放协程：支持渲染层级与颜色切换
    // 参数：transform - 粒子位置（路障僵尸攻击部位）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayConeEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetConeEmissionPartical();      // 从对象池获取路障僵尸发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级
        particle.transform.position = transform.position; // 设为路障僵尸攻击部位位置
        particle.Play();                                // 启动发射粒子

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留
        ReleaseConeEmissionPartical(obj);               // 回收对象
    }

    // 铁桶僵尸发射粒子播放协程：支持渲染层级与颜色切换
    // 参数：transform - 粒子位置（铁桶僵尸攻击部位）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayBucketEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetBucketEmissionPartical();    // 从对象池获取铁桶僵尸发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级
        particle.transform.position = transform.position; // 设为铁桶僵尸攻击部位位置
        particle.Play();                                // 启动发射粒子

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留
        ReleaseBucketEmissionPartical(obj);             // 回收对象
    }

    // 旗帜僵尸发射粒子播放协程：支持渲染层级与颜色切换
    // 参数：transform - 粒子位置（旗帜僵尸攻击部位）；sort - 渲染层级；isCroze - 是否启用特殊颜色
    public IEnumerator PlayFlagEmissionPartical(Transform transform, int sort, bool isCroze, bool isHypno) {
        GameObject obj = GetFlagEmissionPartical();      // 从对象池获取旗帜僵尸发射粒子对象
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // 设置渲染层级
        particle.transform.position = transform.position; // 设为旗帜僵尸攻击部位位置
        particle.Play();                                // 启动发射粒子

        var mainModule = particle.main;
        Color originColor = mainModule.startColor.color; // 保存原始颜色
        if (isCroze) mainModule.startColor = crozeColor; // 切换特殊颜色
        if (isHypno) mainModule.startColor = hypnoColor; // 切换特殊颜色

        yield return new WaitForSeconds(particle.main.duration); // 等待播放完成

        mainModule.startColor = originColor;            // 恢复原始颜色
        particle.Clear();                               // 清除残留
        ReleaseFlagEmissionPartical(obj);               // 回收对象
    }
}