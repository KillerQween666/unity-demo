using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 普通僵尸基类（继承自僵尸父类Zombie），封装通用身体部位和动画逻辑
public class ZombieCommon : Zombie {

    public List<SpriteRenderer> hideRenderers = new List<SpriteRenderer>(); // 需隐藏的身体部位渲染器（如变种僵尸的附加部件）

    // 变种僵尸的显示切换渲染器列表
    public List<SpriteRenderer> hideSprite = new List<SpriteRenderer>(); // 需隐藏的渲染器
    public List<SpriteRenderer> showSprite = new List<SpriteRenderer>(); // 需显示的渲染器

    // 身体部位隐藏状态标记（避免重复执行隐藏逻辑）
    private bool isHideHead = false; // 头部是否已隐藏
    private bool isHideHand = false; // 手部是否已隐藏

    protected new void Awake() {
        base.Awake(); // 调用父类Zombie的初始化方法
        // 隐藏非普通僵尸的附加部件（铁桶、铁门、旗帜等）
        foreach (var render in hideRenderers) render.enabled = false;

        // 初始化变种僵尸的显示状态：隐藏hideSprite，显示showSprite
        hideSprite.ForEach(r => r.enabled = false);
        showSprite.ForEach(r => r.enabled = true);

        RandomWalk(); // 随机设置行走动画样式
    }

    // 重写受伤逻辑（增加身体部位隐藏判断）
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // 生命值低于对应阈值时隐藏手部/头部
        if (HP <= 70) HideHand();
        if (HP <= 20) HideHead();
    }

    // 随机切换行走动画（两种样式随机选）
    void RandomWalk() {
        int randomIndex = Random.Range(0, 2);
        animator.SetInteger("walkIndex", randomIndex); // 给动画控制器传参切换动画
    }

    // 随机切换死亡动画（三种样式按概率分配）
    void RandomDead() {
        int randomIndex = Random.Range(0, 7);
        // 0-2→样式0，3-5→样式1，6→样式2
        animator.SetInteger("deadIndex", randomIndex < 3 ? 0 : (randomIndex < 6 ? 1 : 2));
    }

    // 头部隐藏处理（含特效播放）
    public void HideHead() {
        if (isHideHead) return; // 已隐藏则直接返回
        isHideHead = true;

        isHaveHead = false; // 更新头部存在状态

        headRenderers.ForEach(r => r.enabled = false); // 隐藏所有头部渲染器
        // 播放头部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayHeadEmissionIEnumrator(headEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 手部隐藏处理（含特效播放）
    public void HideHand() {
        if (isHideHand) return; // 已隐藏则直接返回
        isHideHand = true;

        handRenderers.ForEach(r => r.enabled = false); // 隐藏所有手部渲染器
        // 播放手部掉落特效（指定位置、层级和冻结状态）
        ObjectPoolManager.Instance.PlayHandEmissionIEnumrator(handEmissionTransform, spriteList[0].sortingOrder + 100, isCroze, isHypno);
    }

    // 重写死亡处理（先随机死亡动画，再执行父类逻辑）
    public override void ToDead() {
        RandomDead(); // 随机选择死亡动画样式
        base.ToDead(); // 执行父类死亡逻辑（隐藏影子、关闭碰撞等）
    }
}