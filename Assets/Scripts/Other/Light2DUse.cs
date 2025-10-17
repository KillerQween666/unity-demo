using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light2DUse : MonoBehaviour {
    // 引用Light 2D组件
    public Light2D light2D;

    // 记录当前颜色在数组中的索引
    private int currentColorIndex = 0;

    private bool isLightOn = false;
    public float colorChangeTime = 0.5f;
    private float colorChangeTimer;

    // 定义要切换的颜色数组
    private Color[] colors = new Color[]
   {
        new Color(1f, 0f, 0f),       // 红色
        new Color(1f, 0.5f, 0f),     // 橙色
        new Color(1f, 1f, 0f),       // 黄色
        new Color(0f, 1f, 0f),       // 绿色
        new Color(0f, 1f, 1f),       // 青色
        new Color(0f, 0f, 1f),       // 蓝色
        new Color(1f, 0f, 1f)        // 紫色
   };

    void Start() {
        // 启动协程进行颜色切换
        StopLight();
    }

    private void Update() {
        if (isLightOn) {
            colorChangeTimer += Time.deltaTime;
            if (colorChangeTimer >= colorChangeTime) {
                colorChangeTimer = 0;

                light2D.color = colors[currentColorIndex];
                currentColorIndex = (currentColorIndex + 1) % colors.Length;
            }
        }
    }

    public void PlayLight() {
        isLightOn = true;
        light2D.enabled = true;
    }

    public void StopLight() {
        isLightOn = false;
        light2D.enabled = false;
    }

}