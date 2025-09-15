using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���Ľ�����UI��������Ϸʱ��ͽ�����ʾ��
public class FlagMeterUI : MonoBehaviour {

    public Image[] flagMeter;       // ����������ͼƬ��
    public Image filgMeterMask;     // �������֣�������ʾ������
    public Image filgMeterHead;     // ������ͷ��ͼ��
    public Transform endPosition;   // ͷ��ͼ����յ�λ��
    public float gameTime = 10;     // ��Ϸ��ʱ�����룩
    private float gameTimer;        // ��ʱ��

    private bool isGameStart = false; // ��Ϸ�Ƿ�ʼ
    private bool isGameEnd = false;   // ��Ϸ�Ƿ����

    private void Start() {
        Hide(); // ��ʼ����
    }

    private void Update() {
        if (isGameStart && !isGameEnd) {
            gameTimer += Time.deltaTime;
            // ���½������ֱ�������ʱ����٣�
            filgMeterMask.fillAmount = (gameTime - gameTimer) / gameTime;

            // ʱ�䵽����ʾ��������
            if (gameTimer > gameTime) {
                UIManager.Instance.endUI.Show();
                isGameEnd = true;
            }
        }
    }

    // ��Ϸ��ʼ����ʾ������������ͷ��ͼ�궯��
    public void GameStart() {
        isGameStart = true;
        Show();
        // ͷ��ͼ�������ƶ����յ㣨ʱ��=��Ϸ��ʱ�䣩
        filgMeterHead.transform.DOMove(endPosition.position, gameTime).SetEase(Ease.Linear);
    }

    // ��ʾ�������������ͼƬ��
    void Show() {
        foreach (Image image in flagMeter) {
            image.gameObject.SetActive(true);
        }
    }

    // ���ؽ����������ñ���ͼƬ��
    void Hide() {
        foreach (Image image in flagMeter) {
            image.gameObject.SetActive(false);
        }
    }
}