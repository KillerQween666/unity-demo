using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 僵尸显示控制类（用于初始化僵尸显示状态和动画速度）
public class ZombieShow : MonoBehaviour {

    public List<SpriteRenderer> hideRenderers = new List<SpriteRenderer>(); // 需隐藏的身体部位渲染器列表

    private void Awake() {
        // 隐藏非普通僵尸的附加部件（如铁桶、铁门、旗帜等额外装饰）
        foreach (var render in hideRenderers) render.enabled = false;

        // 随机设置动画速度（控制僵尸移动/动作快慢，范围0.7-1.3）
        float speed = Random.Range(0.7f, 1.3f);
        GetComponent<Animator>().speed = speed;
    }
}