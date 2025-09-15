using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFunUse : MonoBehaviour {

    //含有plant组件的父物体
    public Plant fatherPlant;

    //用于在子物体的动画中调用父物体的植物功能（产太阳/发射豌豆等）
    public void UsePlantFun() {
        fatherPlant.GetComponent<Plant>().PlantFun();
    }
}
