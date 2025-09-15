using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 粒子碰撞处理脚本，用于粒子与其他物体碰撞后，调整粒子系统的参数（停止发射、禁用旋转等）
public class Partical : MonoBehaviour {

    // 粒子与其他物体发生碰撞时触发（每碰撞一次执行一次）
    private void OnParticleCollision(GameObject other) {
        // 获取当前物体上的粒子系统组件
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        // 1. 禁用粒子生命周期内的旋转效果（避免碰撞后粒子仍旋转）
        var rotationOverLifetimeModule = particleSystem.rotationOverLifetime;
        rotationOverLifetimeModule.enabled = false;

        // 2. 重置粒子初始旋转：设为0度，禁用3D旋转（确保碰撞后粒子旋转统一）
        var mainModule = particleSystem.main;
        mainModule.startRotation = new ParticleSystem.MinMaxCurve(0f); // 初始旋转固定为0
        mainModule.startRotation3D = false; // 关闭3D旋转（使用2D平面旋转）

        // 3. 禁用粒子碰撞后的弹跳效果（碰撞后直接停止，不反弹）
        var collisionmModule = particleSystem.collision;
        collisionmModule.bounce = 0f; // 弹跳系数设为0，无弹跳

        // 4. 停止粒子发射（碰撞后不再生成新粒子，仅保留已存在的粒子自然消失）
        var main = particleSystem.emission;
        main.enabled = false;
    }
}