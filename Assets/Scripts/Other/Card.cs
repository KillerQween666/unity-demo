using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ����״̬ö�٣����á���ȴ�С��ȴ����⡢׼������
public enum CardState {
    Disable,
    Cooling,
    WaitingSun,
    Ready
}

// ֲ������ö�٣����տ����㶹����
public enum PlantType {
    sunFlower,
    PeaShooter
}

// ֲ�￨�ƽű������ƿ���״̬����ȴ�������жϣ���UI��ʾ���������
public class Card : MonoBehaviour {

    // ��ǰ����״̬��Ĭ�Ͻ��ã�
    public CardState cardState = CardState.Disable;
    // ���ƶ�Ӧ��ֲ������
    public PlantType plantType = PlantType.sunFlower;

    // ����UIԪ�أ�׼������ʱ�ĸ�����Ч
    public GameObject cardLight;
    // ����UIԪ�أ�δ����/��ȴʱ�Ļ�ɫ����
    public GameObject cardGray;
    // ����UIԪ�أ���ȴʱ�Ľ������֣���ʾ��ȴʣ��ʱ�䣩
    public Image cardMask;

    // ������ȴʱ�䣨��Inspector��ֵ����λ���룩
    [SerializeField]
    private float cdTime = 2;
    // ��ȴ��ʱ��
    private float cdTimer = 0;

    // ��ֲ��Ӧֲ�������������������Inspector��ֵ��
    [SerializeField]
    public int needSunPoint = 50;

    // ÿ֡���£����ݵ�ǰ����״ִ̬�ж�Ӧ�߼�
    private void Update() {
        switch (cardState) {
            case CardState.Disable: // ����״̬���޲���
                break;
            case CardState.Cooling: // ��ȴ״̬��������ȴ����
                CoolingUpdate();
                break;
            case CardState.WaitingSun: // �ȴ�����״̬���ж��Ƿ����㹻����
                WaitingSunUpdate();
                break;
            case CardState.Ready: // ׼������״̬���ж������Ƿ��㹻����ֹ���ⲻ��ʱ�������
                ReadyUpdate();
                break;
            default:
                break;
        }
    }

    // ��ȴ״̬���£�������ȴ���ȣ���ȴ�������л����ȴ�����״̬
    void CoolingUpdate() {
        cdTimer += Time.deltaTime;
        // ������ȴ���ֵ���������ʣ����ȴʱ��/����ȴʱ�䣩
        cardMask.fillAmount = (cdTime - cdTimer) / cdTime;

        // ��ȴʱ�䵽���л����ȴ�����״̬
        if (cdTimer > cdTime) {
            TransitionToWaitingSun();
        }
    }

    // �ȴ�����״̬���£����㹻����ʱ���л���׼������״̬
    void WaitingSunUpdate() {
        if (needSunPoint <= SunManager.Instance.sunPoint) {
            TransitionToReady();
        }
    }

    // ׼������״̬���£����ⲻ��ʱ���лصȴ�����״̬
    void ReadyUpdate() {
        if (needSunPoint > SunManager.Instance.sunPoint) {
            TransitionToWaitingSun();
        }
    }

    // �л�����ȴ״̬����ֲֲ�����ã�
    public void TransitionToCooling() {
        cardState = CardState.Cooling;

        cdTimer = 0; // ������ȴ��ʱ��
        cardLight.SetActive(false); // �رո�����Ч
        cardGray.SetActive(true); // ��ʾ��ɫ����
        cardMask.gameObject.SetActive(true); // ��ʾ��ȴ��������
    }

    // �л����ȴ�����״̬����ȴ����/���ⲻ��ʱ���ã�
    void TransitionToWaitingSun() {
        cardState = CardState.WaitingSun;

        cardLight.SetActive(false); // �رո�����Ч
        cardGray.SetActive(true); // ��ʾ��ɫ����
        cardMask.gameObject.SetActive(false); // ������ȴ��������
    }

    // �л���׼������״̬�������㹻ʱ���ã�
    public void TransitionToReady() {
        cardState = CardState.Ready;

        cardLight.SetActive(true); // ��ʾ������Ч
        cardGray.SetActive(false); // ���ػ�ɫ����
        cardMask.gameObject.SetActive(false); // ������ȴ��������
    }

    // ���Ƶ���¼���ѡ�п��ƣ���ֲ�������꣩
    public void OnClick() {
        HandManager.Instance.AddPlant(plantType);
    }

    // ���̧���¼����ж��Ƿ����ڵ�Ԫ���ϣ�������ֲ�߼���
    public void OnPointerUp() { 
        // �������Ļ����ת��Ϊ�������꣨Z����Ϊ0������㼶��ͻ��
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        // ���߼�����λ�õ���ײ��
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero);

        // �������Ԫ��ʱ�����õ�Ԫ��ĵ���߼�����ֲֲ�
        if (hit) {
            if (hit.collider.CompareTag("Cell")) {
                Cell cell = hit.collider.GetComponent<Cell>();
                cell.OnClick();
            }
        }   
    }
}