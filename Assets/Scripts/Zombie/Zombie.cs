using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;

public class Zombie : MonoBehaviour {

    public Animator animator; // 动画组件

    // 身体部位渲染器列表
    public List<SpriteRenderer> headRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> handRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> hideRenderers = new List<SpriteRenderer>();

    public float HP = 100; // 生命值
    public float minSpeed = 0.8f; // 最小移动速度
    public float maxSpeed = 1.2f; // 最大移动速度
    public int atkValue = 10; // 攻击力

    private Plant currentEatPlant; // 当前攻击的植物

    // 身体部位隐藏标记
    private bool isHideHead = false;
    private bool isHideHand = false;

    private List<SpriteRenderer> spriteList = new List<SpriteRenderer>(); // 所有渲染器

    // 闪烁效果参数
    private float originalBright = 1f;
    private float flashBright = 2f;
    public Coroutine flashCoroutine;

    // 特效位置
    public Transform headEmissionTransform;
    public Transform handEmissionTransform;
    public GameObject shadow; // 影子

    // 初始化组件和状态
    public void Awake() {
        animator = GetComponent<Animator>();

        // 将动画里除普通僵尸以外的部分隐藏（铁桶，铁门，旗帜等）
        foreach (var render in hideRenderers) render.enabled = false;

        // 收集所有渲染器
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) spriteList.Add(sprite);

        RandomWalk(); // 随机初始化行走动画
    }

    // 设置动画速度
    public void SetAnimatorSpeed(float speed) {
        animator.speed = speed;
    }

    // 进食状态更新（攻击逻辑）
    public void EatPlant() {
        currentEatPlant?.TakeDamage(atkValue); // 攻击植物
        // 随机播放进食音效
        var clips = new[] { Config.eatPlant, Config.eatPlant2, Config.eatPlant3 };
        AudioManager.Instance.PlayClip(clips[Random.Range(0, 3)]);
    }

    // 触发进入（碰撞检测）
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            currentEatPlant = collision.GetComponent<Plant>();
            animator.SetBool("isEat", true);
        }
        else if (collision.CompareTag("House")) {
            animator.SetBool("isEat", true);
            GameManager.Instance.GameEndFail(); // 触发游戏失败
        }
    }

    // 触发退出（碰撞检测）
    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            animator.SetBool("isEat", false);
            currentEatPlant = null;
        }
    }

    // 随机行走动画和速度
    void RandomWalk() {
        int randomIndex = Random.Range(0, 2);
        animator.SetInteger("walkIndex", randomIndex);
        SetAnimatorSpeed(Random.Range(minSpeed, maxSpeed));
    }

    // 随机死亡动画
    void RandomDead() {
        int randomIndex = Random.Range(0, 7);
        animator.SetInteger("deadIndex", randomIndex < 3 ? 0 : (randomIndex < 6 ? 1 : 2));
    }

    // 隐藏头部
    public void HideHead() {
        if (isHideHead) return;
        isHideHead = true;

        headRenderers.ForEach(r => r.enabled = false);
        // 播放头部掉落特效
        ObjectPoolManager.Instance.PlayHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100);
    }

    // 隐藏手部
    public void HideHand() {
        if (isHideHand) return;
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false);
        // 播放手部掉落特效
        ObjectPoolManager.Instance.PlayHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100);
    }

    // 受到伤害（虚方法，允许子类重写）
    public virtual void TakeDamage(float damage) {
        HP -= damage;
        // 播放闪烁效果
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // 按生命值隐藏身体部位
        if (HP <= 70) HideHand();
        if (HP <= 20) HideHead();
        // 死亡判断
        if (HP <= 0) ToDead();
    }

    // 死亡处理
    public void ToDead() {
        SetAnimatorSpeed(1);
        RandomDead();
        shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetBool("isDead", true);
    }

    // 被压倒处理（如被小推车压）
    public void Push() {
        SetAnimatorSpeed(1);
        shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetBool("isPush", true);
    }

    // 完全死亡（移除对象）
    public void Dead() {
        Destroy(gameObject);
        ZombieManager.Instance.RemoveZombie();
    }

    // 受伤闪烁效果
    public virtual IEnumerator PlayFlash() {
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", flashBright));
        yield return new WaitForSeconds(0.2f);
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", originalBright));
    }
}