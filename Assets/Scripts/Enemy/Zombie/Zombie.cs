using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.U2D;
using UnityEngine.UI;

// 僵尸类，处理移动、攻击、受击等行为
public class Zombie : Enemy {

    public Animator animator; // 控制僵尸动画的组件

    // 头部和手部的渲染器，用于单独控制显示
    public List<SpriteRenderer> headRenderers = new List<SpriteRenderer>();
    public List<SpriteRenderer> handRenderers = new List<SpriteRenderer>();

   
    public float minSpeed = 0.8f; // 移动速度最小值
    public float maxSpeed = 1.2f; // 移动速度最大值
    public int atkValue = 12; // 攻击造成的伤害

    private Plant currentEatPlant; // 正在攻击的植物
    private Coroutine slowCoroutine; // 控制减速效果的协程
    private Coroutine crozeCoroutine;

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
    private Color hypnoColor = new Color(0.9320f, 0, 0.6588f);
    protected bool isCroze = false; // 是否处于减速状态
    protected bool isHypno;
    protected bool isIceCroze = false;
    protected bool isDead = false;

    public GameObject Ice;

    private Enemy currentEatEnemy;

    public Enemy enemy;
    public int row;

    public Collider2D selfCollider;
    private List<Collider2D> colliderList = new List<Collider2D>();

    private float targetXPosition = 20;

    public Transform bodyTransform;

    public bool isCanCroze = true;

    public bool isCanBoom = false;

    // 初始化组件和基础状态
    protected void Awake() {
        if (animator == null)  animator = GetComponent<Animator>();
        if (selfCollider == null)   selfCollider = GetComponent<Collider2D>();

        // 随机设置初始移动速度
        originSpeed = UnityEngine.Random.Range(minSpeed, maxSpeed);
        SetAnimatorSpeed(originSpeed);

        // 收集所有渲染器并记录原始颜色
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var sprite in sprites) {
            originColors.Add(sprite, sprite.color);
            spriteList.Add(sprite);
        }
    }

    protected virtual void FixedUpdate() {
        if (transform.position.x > targetXPosition) {
            Dead();
        }
        if (isHaveHead == false) {
            HP -= 0.25f;
            if (HP <= 0) {
                ToDead();
            }
        }
    }

    // 设置动画播放速度（影响移动和攻击快慢）
    public void SetAnimatorSpeed(float speed) {
        animator.speed = speed;
    }

    // 攻击植物的逻辑
    public void EatPlant() {
        if (currentEatEnemy)   currentEatEnemy.TakeDamage(atkValue, 4);
        if (currentEatPlant)    currentEatPlant.TakeDamage(atkValue);

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
        if (collision.TryGetComponent<Plant>(out var plant) && plant.isCanEat == false) {
            return;
        }

        if (isHypno) {
            if (collision.CompareTag("Zombie")) {
                if (!colliderList.Contains(collision)) {
                    colliderList.Add(collision);

                    if (colliderList.Count == 1) {
                        OnObjectEnter(collision);
                    }
                }
            }
        }
        else { 
            if (collision.CompareTag("Plant")) {
                if (!colliderList.Contains(collision)) {
                    colliderList.Add(collision);

                    if (colliderList.Count == 1) {
                        OnObjectEnter(collision);
                    }
                }
            } else if (collision.CompareTag("House")) {
                EnterEat();
                GameManager.Instance.GameEndFail(); // 攻击房子，游戏失败

            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (colliderList.Contains(collision)) {
            OnObjectExit();
            colliderList.Remove(collision);

            if (isHypno) {
                IsAttackZombie();
            } else {
                IsAttackPlant();
            }

            if (colliderList.Count > 0) {
                OnObjectEnter(colliderList[0]);
            }
        }
        
    }

    private void OnObjectEnter(Collider2D collision) {
        if (collision != null) {
            if (collision.TryGetComponent<Plant>(out var plant)) {
                currentEatPlant = plant;
                EnterEat(); // 切换到攻击动画
            }
            if (collision.TryGetComponent<Enemy>(out var enemy)) {
                currentEatEnemy = enemy;
                EnterEat(); // 切换到攻击动画
            }     
        }   
    }

    private void OnObjectExit() {
        ExitEat();  // 停止攻击动画
        currentEatPlant = null; // 清空目标 
        currentEatEnemy = null;
    }

    public void ClearColliderList() {
        colliderList.Clear();
    }

    public virtual void TakeCommonDamage(float damage) {
        if (isCroze) PlaySlowSpeed();

        HP -= damage;

        // 播放受伤闪烁效果
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(PlayFlash());
        // 生命值为0时死亡
        if (HP <= 0) ToDead();
    }

    // 受到伤害的处理（可被重写）
    public override void TakeDamage(float damage, int hurtType = 0) {
        if (isDead == true) return;

        if (hurtType == 1 && damage >= HP) {
            Dead();
            PlayBoomSwf(transform, isHaveHead, spriteList[0].sortingOrder + 100);
        } else if (hurtType == 2 && damage >= HP) {
            Dead();
        } else {
            if (hurtType == 3) isCroze = true;
            if (hurtType != 4) PlayAttackSource(); // 播放受击音效

            if (hurtType == 4 && enemy != null) {
                enemy.TakeDamage(damage);
            } else {
                TakeCommonDamage(damage);
            }
        }
    }

    protected virtual void PlayBoomSwf(Transform transform, bool isHaveHead, int sort) {
        ObjectPoolManager.Instance.PlayZombieBoomSwfIEnumrator(transform, isHaveHead, sort);
    }

    // 被碾压的处理（如被小推车压）
    public void Push() {
        if (isDead) return;
        isDead = true;

        DeadPrepare();

        animator.SetBool("isPush", true); // 播放被碾压动画
        StartCoroutine(PushAnimation());
    }

    public virtual void ToDead() {
        if (isDead) return;
        isDead = true;

        DeadPrepare();

        animator.SetBool("isDead", true); // 切换到死亡动画
    }

    public void DeadPrepare() {
        animator.SetBool("isEat", false);

        originSpeed = 1;
        SetAnimatorSpeed(1);
        if (isIceCroze) {
            StopCoroutine(crozeCoroutine);
            IceCrack();
            PlaySlowSpeed();
        }
        else {
            SetAnimatorSpeed(1);
        }

        if (Ice != null) Ice.SetActive(false);
        if (shadow != null) shadow.SetActive(false); // 隐藏影子
        GetComponent<Collider2D>().enabled = false; // 关闭碰撞
    }

    // 完全死亡（移除对象并通知管理器）
    public virtual void Dead() {
        Destroy(gameObject);
        if (isHypno == false)  ZombieManager.Instance.RemoveZombie(); // 从管理器中移除
    }

    // 受伤闪烁效果的协程
    public virtual IEnumerator PlayFlash() {
        foreach (var sprite in spriteList) {
            if (sprite != null) {
                sprite.material.SetFloat("_Brightness", flashBright);
            }
        }
        yield return new WaitForSeconds(0.2f);
        foreach (var sprite in spriteList) {
            if (sprite != null) {
                sprite.material.SetFloat("_Brightness", originalBright);
            }
        }
    }

    // 触发减速效果（如被冰冻）
    public void PlaySlowSpeed() {
        if (isCanCroze == false) return;

        if (isIceCroze || isDead)   return;
        
        if (slowCoroutine != null) {
            StopCoroutine(slowCoroutine); // 终止当前减速协程，避免叠加
            slowCoroutine = StartCoroutine(SlowSpeed());
        } else {
            slowCoroutine = StartCoroutine(SlowSpeed());
        }

    }

    // 触发减速效果（如被冻结）
    public void PlayCrozeSpeed() {
        if (isCanCroze == false) return;
        if (isDead) return;

        if (crozeCoroutine != null) {
            if (slowCoroutine != null) StopCoroutine(slowCoroutine);

            StopCoroutine(crozeCoroutine); // 终止当前减速协程，避免叠加
            crozeCoroutine = StartCoroutine(CrozeSpeed());
        } else {
            crozeCoroutine = StartCoroutine(CrozeSpeed());
        }
    }

    // 冻结效果的协程（持续4秒）
    IEnumerator CrozeSpeed() {
        spriteList.ForEach(s => s.color = crozeColor); // 切换为冻结色
        SetAnimatorSpeed(0);
        isCroze = true;
        isIceCroze = true;

        Ice.SetActive(true);

        yield return new WaitForSeconds(4f);

        IceCrack();
        PlaySlowSpeed();
    }

    public void IceCrack() {
        if (crozeCoroutine != null) StopCoroutine(crozeCoroutine);
        Ice.SetActive(false);
        isIceCroze = false;
        ObjectPoolManager.Instance.PlayIceCrackParticalIEnumrator(shadow.transform);
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
    public virtual void PlayAttackSource() {
        var clips = new[] { Config.splat, Config.splat2, Config.splat3 };
        AudioManager.Instance.PlayClip(clips[UnityEngine.Random.Range(0, 3)]);
    }

    public bool IsCroze() {
        return isCroze;
    }

    public void Hypnoed() {
        isHypno = true;
        ZombieManager.Instance.RemoveZombie();

        if (slowCoroutine != null) {
            StopCoroutine(slowCoroutine);
            isCroze = false;
        }
        if (crozeCoroutine != null) {
            StopCoroutine(crozeCoroutine);
            isCroze = false;
            IceCrack();
        }
        SetAnimatorSpeed(originSpeed);

        spriteList.ForEach(s => s.color = hypnoColor);

        gameObject.tag = "Plant";
        gameObject.layer = 8;

        if (enemy != null) {
            enemy.gameObject.tag = "Plant";
            enemy.gameObject.layer = 8;
        }

        OnObjectExit();
        ClearColliderList();

        IsAttackZombie();

        Vector3 position = transform.position;
        position.x -= 0.7f;
        transform.position = position;

        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    public void IsAttackPlant() {

        // 检测爆炸范围内的所有僵尸（仅检测"Plant"层）
        Bounds bounds = selfCollider.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,       // 爆炸范围中心（碰撞体中心点）
            bounds.size,         // 爆炸范围大小（碰撞体尺寸）
            selfCollider.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
            LayerMask.GetMask("Plant") // 目标检测层：僵尸层
        );

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            if (coll != null) { // 避免空引用异常
                if (coll.TryGetComponent<Plant>(out var plant) && plant.isCanEat == false) return;

                if (!colliderList.Contains(coll)) {
                    colliderList.Add(coll);
                    if (colliderList.Count == 1) {
                        OnObjectEnter(coll);
                    }
                }

                if (coll.TryGetComponent<Zombie>(out var zombie)) {
                    if (!zombie.colliderList.Contains(selfCollider)) {
                        zombie.IsAttackZombie();
                    }
                }
            }
        }
    }

    public void IsAttackZombie() {

        // 检测爆炸范围内的所有僵尸（仅检测"Zombie"层）
        Bounds bounds = selfCollider.bounds;
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
            bounds.center,       // 爆炸范围中心（碰撞体中心点）
            bounds.size,         // 爆炸范围大小（碰撞体尺寸）
            selfCollider.transform.rotation.eulerAngles.z, // 爆炸范围旋转角度
            LayerMask.GetMask("Zombie") // 目标检测层：僵尸层
        );

        // 对爆炸范围内的僵尸执行伤害逻辑
        foreach (var coll in hitColliders) {
            
            if (coll != null) { // 避免空引用异常
                if (!colliderList.Contains(coll)) {
                    colliderList.Add(coll);
                    if (colliderList.Count == 1) {
                        OnObjectEnter(coll);
                    }
                }
                
                if (coll.TryGetComponent<Zombie>(out var zombie)) {
                    if (!zombie.colliderList.Contains(selfCollider)) {
                        zombie.IsAttackPlant();
                    }
                }
            }
        }
    }

    IEnumerator PushAnimation() {
        // 总时长：仅0.2秒（旋转+移动）
        float totalDuration = 0.33f;

        // 动画参数（可自定义）
        float rotateAngle = -97f; // 旋转角度
        Vector3 moveOffset = new Vector3(0.1f, 0.7f, 0); // 移动距离

        // 记录初始状态
        Quaternion startRot = transform.rotation;
        Vector3 startPos = transform.position;
        // 计算目标状态
        Vector3 targetPos = startPos + moveOffset;
        Quaternion targetRot = Quaternion.Euler(0, 0, rotateAngle);

        float timer = 0;
        while (timer < totalDuration) {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / totalDuration);

            // 插值更新位置和旋转
            transform.position = Vector3.Lerp(startPos, targetPos, progress);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, progress);

            yield return null;
        }

        if (this.gameObject != null) {
            Dead();
        }
    }

    public void PushDeadIenumrator() {
        StartCoroutine(PushDead());
    }

    private IEnumerator PushDead() {
        if (isDead != true) {
            animator.SetBool("isEat", false);

            originSpeed = 0;
            SetAnimatorSpeed(0);
            if (slowCoroutine != null) StopCoroutine(slowCoroutine);
            if (isIceCroze) {
                StopCoroutine(crozeCoroutine);
                IceCrack();
            }

            if (Ice != null) Ice.SetActive(false);
            if (shadow != null) shadow.SetActive(false);
            GetComponent<Collider2D>().enabled = false;
        }

        Vector3 currentScale = transform.localScale;
        Vector3 squashScale = new Vector3(1.04f * currentScale.x, 0.56f * currentScale.y, 0.8f * currentScale.z);
        transform.localScale = squashScale;

        yield return new WaitForSeconds(1f);

        Dead();
    }

}