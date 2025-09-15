using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小车脚本，控制小车的移动、动画播放及与僵尸的碰撞交互
public class Car : MonoBehaviour {

    // SWF动画控制器（用于播放小车的动画，依赖FTRuntime插件）
    private SwfClipController swf;
    // 小车的2D刚体（用于物理相关逻辑，当前代码暂未直接使用物理力）
    private Rigidbody2D rgd;

    // 小车移动的目标X坐标（到达此位置后销毁小车）
    public float targetXPosition = 10;

    // 小车移动速度（单位：单位/秒）
    public float moveSpeed = 3f;

    // 小车是否处于移动状态的标记
    bool isMove = false;

    // 初始化：获取动画控制器和刚体组件
    private void Awake() {
        swf = GetComponent<SwfClipController>();
        rgd = GetComponent<Rigidbody2D>();
    }

    // 每帧更新：如果小车处于移动状态，持续向右移动，到达目标位置后销毁
    private void Update() {
        if (isMove == true) {
            // 按速度向右移动（Translate基于世界坐标，适合简单线性移动）
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            // 小车X坐标超过目标位置，销毁自身
            if (transform.position.x > targetXPosition) {
                Destroy(gameObject);
            }
        }
    }

    // 触发碰撞时执行：启动小车移动，并对碰撞到的僵尸执行"推开"逻辑
    private void OnTriggerEnter2D(Collider2D collision) {
        CarMove(); // 启动小车移动（播放动画、音效，标记为移动状态）

        // 如果碰撞到的是僵尸，调用僵尸的"压扁"方法
        if (collision.CompareTag("Zombie")) {
            collision.GetComponent<Zombie>().Push();
        }
    }

    // 小车移动逻辑：播放动画、音效，标记为移动状态
    void CarMove() {
        swf.Play(true); // 播放SWF动画（参数true表示循环播放，根据需求调整）
        AudioManager.Instance.PlayClip(Config.carMove); // 播放小车移动音效
        isMove = true; // 标记小车为移动状态，触发Update中的移动逻辑
    }
}