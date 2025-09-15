using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// �����������������������ɡ�����ͳ�ơ�UI���¼����������߼�
public class SunManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ㣨�ⲿͨ���˲������⣩
    public static SunManager Instance { get; private set; }

    // ��ǰӵ�е���������
    public int sunPoint;

    // ��ʾ����������UI�ı���TextMeshPro��
    public TextMeshProUGUI sunPointText;

    // �Զ���������ļ��ʱ�䣨�룩
    public float produceTime;
    // �������ɼ�ʱ��
    private float produceTimer;
    // ����Ԥ���壨�Ӷ���ػ�ȡ����Inspector��ֵ��
    public Sun sunPrefab;

    // �Ƿ�ʼ�Զ���������ı�ǣ���GameManager����������
    private bool isStartProduce = false;

    // ��ʼ������
    private void Awake() {
        Instance = this;
    }

    // ��Ϸ����ʱ��������UI����ʾ��ʼ����������
    private void Start() {
        UpdateSunPointText();
    }

    // ÿ֡���£���������Զ����ɣ�ִ�����������߼�
    private void Update() {
        if (isStartProduce) {
            ProduceSun();
        }
    }

    public void StartProduce() {
        isStartProduce = true;
    }

    // ��������������UI�ı���ȷ����ʾ��ʵ������һ�£�
    public void UpdateSunPointText() {
        sunPointText.text = sunPoint.ToString();
    }

    // ����������������ֲֲ��ʱ���ã�
    public void SubSun(int point) {
        sunPoint -= point;
        UpdateSunPointText(); // �������������UI
    }

    // ���������������ռ�����ʱ���ã�
    public void AddSun(int point) {
        sunPoint += point;
        UpdateSunPointText(); // �������������UI
    }

    // �Զ���������ĺ����߼�����ʱ���������Ⲣ�����ƶ���Ŀ��λ�ã�
    void ProduceSun() {
        produceTimer += Time.deltaTime;
        // ��ʱ���ﵽ���ʱ�䣬��������
        if (produceTimer > produceTime) {
            produceTimer = 0; // ���ü�ʱ��

            // �����������ĳ�ʼλ�ã�X��-5��7��Y��6��Z��-3��ȷ������Ļ�Ϸ���
            Vector3 position = new Vector3(Random.Range(-5, 7f), 6f, -3);
            // �Ӷ���ػ�ȡ�������
            GameObject obj = ObjectPoolManager.Instance.GetSun();
            obj.transform.position = position;

            // ��������Ŀ����㣨Y��-4��3���������䵽���渽����
            position.y = Random.Range(-4, 3f);
            // �����������ƶ���Ŀ��λ��
            obj.GetComponent<Sun>().LinearTo(position);
        }
    }
}