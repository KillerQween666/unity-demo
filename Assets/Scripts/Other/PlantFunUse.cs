using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantFunUse : MonoBehaviour {

    //����plant����ĸ�����
    public Plant fatherPlant;

    //������������Ķ����е��ø������ֲ�﹦�ܣ���̫��/�����㶹�ȣ�
    public void UsePlantFun() {
        fatherPlant.GetComponent<Plant>().PlantFun();
    }
}
