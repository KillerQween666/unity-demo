using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class ZombieZamboni : Zombie {
    public SpriteRenderer tin;    // 坚果墙完好状态的脸
    public SpriteRenderer tin2;   // 坚果墙中度损伤的脸
    public SpriteRenderer tin3;   // 坚果墙重度损伤的脸

    bool isShowTin2 = false;  // 标记是否已显示中度损伤的脸
    bool isShowTin3 = false;  // 标记是否已显示重度损伤的脸

    private bool isStartSource = false;

    public Ice iceTrail;

    public float iceSpawnTime = 2f;
    private float iceSpawnTimer = 10f;

    public Transform iceSpawnTransform;

    private bool isIceHave = true;

    private List<Ice> iceList = new List<Ice>();

    public float moveSpeed = 1;

    public ParticleSystem smokePartical;

    public float shakeAmount = 0.1f;
    public float shakeSpeed = 20f;
    private Vector3 basePosition;

    private bool isShake = false;

    private bool isCanMove = true;

    public Transform boomTransform1;
    public Transform boomTransform2;
    private Transform boomTransform;

    private void Start() {
        boomTransform = boomTransform1;
        smokePartical.Stop();
        tin2.enabled = false;
        tin3.enabled = false;
        basePosition = transform.localPosition;
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();

        iceSpawnTimer += Time.deltaTime;
        if (iceSpawnTimer > iceSpawnTime) {
            iceSpawnTimer = 0;

            Ice ice = Instantiate(iceTrail, iceSpawnTransform.position, Quaternion.identity);
            SpriteRenderer sprite = ice.GetComponent<SpriteRenderer>();
            sprite.sortingLayerName = ZombieManager.Instance.layerNames[row];

            iceList.Add(ice);
        }

        //transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        if (isCanMove) {
            basePosition += Vector3.left * Time.deltaTime * moveSpeed;

            if (isShake) {
                transform.localPosition = GetShakePosition(basePosition);
            }
            else {
                transform.localPosition = basePosition;
            }
        }
         
    }

    private Vector3 GetShakePosition(Vector3 basePos) {
        float x = basePos.x + Random.Range(-shakeAmount, shakeAmount);
        float y = basePos.y + Random.Range(-shakeAmount, shakeAmount);
        float z = basePos.z + Random.Range(-shakeAmount, shakeAmount);

        // 平滑过渡到震动位置（保持震动的柔和感）
        return Vector3.Lerp(
            transform.localPosition,
            new Vector3(x, y, z),
            Time.deltaTime * shakeSpeed
        );
    }

    // 重写受伤逻辑（新增路障状态随生命值变化的处理）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 根据生命值区间切换路障状态
        if (HP <= 600) {
            ShowTin2(); // 切换至轻微破损状态
        }

        if (HP <= 300) {
            isHaveHead = false;
            ShowTin3(); // 切换至路障完全损坏状态
        }

        if (HP <= 150) {
            isShake = true;
        }
             
    }

    // 切换到中度损伤状态
    void ShowTin2() {
        if (isShowTin2) return; // 已处于该状态则直接返回
        isShowTin2 = true;

        tin.enabled = false;
        tin2.enabled = true; // 显示中度损伤的脸
    }

    // 切换到重度损伤状态
    void ShowTin3() {
        if (isShowTin3) return; // 已处于该状态则直接返回
        isShowTin3 = true;

        smokePartical.Play();
        tin2.enabled = false;
        tin3.enabled = true; // 显示重度损伤的脸
    }

    protected override void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null) {
            if (collision.CompareTag("Plant")) {
                if (collision.TryGetComponent<Caltrop>(out var caltrop)) {
                    AudioManager.Instance.PlayClip(Config.balloon_pop);
                    if (Random.value  > 0.5f) {
                        animator.SetTrigger("pierceTrigger"); // 触发跳转动画
                    } else {
                        boomTransform = boomTransform2;
                        animator.SetTrigger("pierceTrigger2"); // 触发跳转动画
                    }

                    isCanMove = false;
                    DeadPrepare();
                    caltrop.Dead();
                } else if (collision.TryGetComponent<SpikeRock>(out var spikeRock)) {
                    AudioManager.Instance.PlayClip(Config.balloon_pop);
                    if (Random.value > 0.5f) {
                        animator.SetTrigger("pierceTrigger"); // 触发跳转动画
                    }
                    else {
                        boomTransform = boomTransform2;
                        animator.SetTrigger("pierceTrigger2"); // 触发跳转动画
                    }

                    isCanMove = false;
                    DeadPrepare();
                    spikeRock.PierceZombieZamboni();
                } else {
                    if (collision.TryGetComponent<Plant>(out var plant)) {
                        plant.PushDeadIenumerator();
                    }
                    if (collision.TryGetComponent<Zombie>(out var zombie)) {
                        zombie.PushDeadIenumrator();
                    }
                }
                    
            }
        }
        
    }

    public override void PlayAttackSource() {
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.bucket : Config.bucket2);
    }

    public void PlayStartSource() {
        if (isStartSource == true) return;
        isStartSource = true;

        AudioManager.Instance.PlayClip(Config.zamboni);
    }

    public void PlayExplosion() {
        AudioManager.Instance.PlayClip(Config.explosion);
    }

    public void IceDead() {
        isIceHave = false;

        foreach (var ice in iceList) {
            if (ice != null) {
                ice.Dead();
            }
        }
    }

    public override void Dead() {
        if (isIceHave == true) {
            ZombieManager.Instance.SpawnBobsledZombie(row, transform.position.x);
        }
        smokePartical.Stop();
        ObjectPoolManager.Instance.PlayZombieCarBoomParticalIEnumrator(boomTransform);
        base.Dead();
    }

    protected override void PlayBoomSwf(Transform transform, bool isHaveHead, int sort) {
        ObjectPoolManager.Instance.PlayZombieZamboniBoomSwfIEnumrator(transform, sort);
    }

}
