using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// С��������������С���б����ʾ����
public class CarManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ�
    public static CarManager instance { get; private set; }

    // �洢С�������飨��Inspector�и�ֵ��
    public Car[] carList;

    // ��ʼ������
    private void Awake() {
        instance = this;
    }

    // ��ʾС���б�ͨ��DoTween����������С���ƶ���ָ��Xλ��
    public void ShowCarList() {
        foreach (Car car in carList) {
            // С���ֲ�X���ƶ���-0.2f������ʱ��0.5��
            car.transform.DOLocalMoveX(-0.2f, 0.5f);
        }
    }
}