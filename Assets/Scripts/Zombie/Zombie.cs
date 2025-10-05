using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;
using UnityEngine.UI;

// 僵尸类，处理移动、攻击、受击等行为
public class Zombie : MonoBehaviour {

    public Animator animator; // 控制僵尸动画的组件

    // 头部和手部的渲染器，用于单独控制显示
    public List<SpriteRenderer> headRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> handRenderers = new List<SpriteRenderer>();

    public float HP = 100; // 僵尸的生命值
    public float minSpeed = 0.8f; // 移动速度最小值
    public float maxSpeed = 1.2f; // 移动速度最大值
    public int atkValue = 10; // 攻击造成的伤害

    private Plant currentEatPlant; // 正在攻击的植物
    private Coroutine coroutine; // 控制减速效果的协程

    public List<SpriteRenderer> spriteList = new List<SpriteRenderer>(); // 所有渲染组件，用于统一效果
    private Dictionary<SpriteRenderer, Color> originColors = new Dictionary<SpriteRenderer, Color>(); // 保存原始颜色，用于恢复

    // 受伤闪烁效果的亮度参数
    protected float originalBright = 1f; // 正常亮度
    protected float flashBright = 2f; // 闪烁时的亮度
    public Coroutine flashCoroutine; // 闪烁效果的协程

    // 特效生成的位置（头部和手部）
    public Transform headEmissionTransform;
    public Transform handEmissionTransform;
    public GameObject shadow; // 僵尸的影子对象

    protected float originSpeed; // 初始移动速度（随机生成后保存）

    public bool isHaveHead = true; // 标记是否有头部

    private Color crozeColor = new Color(0.4f, 0.5568f, 1); // 冻结状态的颜色
    protected bool isCroze = false; // 是否处于冻结状态

    // 初始化组件和基础状态
    protected void Awake() {
        animator = GetComponent<Animator>();

        // 随机设置初始移动速度
        originSpeed = Random.Range(minSpeed, maxSpeed);
        SetAnimatorSpeed(originSpeed);

        // 收集所有渲染器并记录原始颜色
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            originColors.Add(sprite, sprite.color);
            spriteList.Add(sprite);
        }
    }

    // 设置动画播放速度（影响移动和攻击快慢）
    public void SetAnimatorSpeed(float speed) {
        animator.speed = speed;
    }

    // 攻击植物的逻辑
    public void EatPlant() {
        currentEatPlant?.TakeDamage(atkValue); // 对当前目标植物造成伤害
        // 播放进食音效
        AudioManager.Instance.PlayClip(Config.eatPlant3);
    }

    protected virtual void EnterEat() {
        animator.SetBool("isEat", true);
    }

    protected virtual void ExitEat() {
        animator.SetBool("isEat", false); 
    }

    // 碰撞进入时触发（检测到植物或房子）
    protected virtual void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            currentEatPlant = collision.GetComponent<Plant>();
            EnterEat(); // 切换到攻击动画
        }
        else if (collision.CompareTag("House")) {
            animator.SetBool("isEat", true);
            GameManager.Instance.GameEndFail(); // 攻击房子，游戏失败
        }
    }

    // 碰撞退出时触发（离开植物）
    protected virtual void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Plant")) {
            ExitEat();  // 停止攻击动画
            currentEatPlant = null; // 清空目标
        }
    }

    public virtual void TakeCommonDamage(float damage) {
        if (isCroze) PlaySlowSpeed();

        HP -= damage;
        PlayAttackSource(); // 播放受击音效

        // 播放受伤闪烁效果
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // 生命值为0时死亡
        if (HP <= 0) ToDead();
    }

    // 受到伤害的处理（可被重写）
    public void TakeDamage(float damage, int hurtType = 0) {
        if (hurtType == 1 && damage >= HP) {
            Dead();
            ObjectPoolManager.Instance.PlayZombieBoomSwfIEnumrator(transform, isHaveHead, spriteList[0].sortingOrder + 100);
        } else if (hurtType == 2 && damage >= HP) {
            Dead();
        } else {
            if (hurtType == 3) isCroze = true;
            TakeCommonDamage(damage);     
        }
    }

    // 死亡处理（播放死亡动画）
    public virtual void ToDead() {
        shadow.SetActive(false); // 隐藏影子
        GetComponent<Collider2D>().enabled = false; // 关闭碰撞
        animator.SetBool("isDead", true); // 切换到死亡动画
    }

    // 被碾压的处理（如被小推车压）
    public void Push() {
        SetAnimatorSpeed(1);
        shadow.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        animator.SetBool("isPush", true); // 播放被碾压动画
    }

    // 完全死亡（移除对象并通知管理器）
    public void Dead() {
        Destroy(gameObject);
        ZombieManager.Instance.RemoveZombie(); // 从管理器中移除
    }

    // 受伤闪烁效果的协程
    public virtual IEnumerator PlayFlash() {
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", flashBright));
        yield return new WaitForSeconds(0.2f);
        spriteList.ForEach(s => s.material.SetFloat("_Brightness", originalBright));
    }

    // 触发减速效果（如被冰冻）
    public void PlaySlowSpeed() {
        if (coroutine != null) {
            StopCoroutine(coroutine); // 终止当前减速协程，避免叠加
            coroutine = StartCoroutine(SlowSpeed());
        }
        else {
            AudioManager.Instance.PlayClip(Config.Frozen); // 播放冻结音效
            coroutine = StartCoroutine(SlowSpeed());
        }

    }

    // 减速效果的协程（持续4秒）
    IEnumerator SlowSpeed() {
        spriteList.ForEach(s => s.color = crozeColor); // 切换为冻结色
        SetAnimatorSpeed(originSpeed * 0.5f); // 速度减半
        isCroze = true;

        yield return new WaitForSeconds(4f);

        // 恢复正常状态
        isCroze = false;
        SetAnimatorSpeed(originSpeed);
        foreach (var pair in originColors) {
            pair.Key.color = pair.Value; // 恢复原始颜色
        }
    }

    // 播放受击音效（随机选一个）
    protected virtual void PlayAttackSource() {
        var clips = new[] { Config.splat, Config.splat2, Config.splat3 };
        AudioManager.Instance.PlayClip(clips[Random.Range(0, 3)]);
    }
}