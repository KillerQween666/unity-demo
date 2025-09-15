using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����б�UI�����ƿ�����ʾ�͵��ȡ���߼�
public class CardListUI : MonoBehaviour {

    // ��ʾ�����б�������������ʾ��ɺ����ÿ���
    public void Show() {
        GetComponent<RectTransform>().DOLocalMoveY(452, 0.5f)
            .OnComplete(CardManager.Instance.EnableCards);
    }

    // ��������б�ʱ��ȡ����ǰѡ�е�ֲ������
    public void OnClick() {
        // ȡ��ѡ�е�ֲ��
        if (HandManager.Instance.currentPlant != null) {
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2);
            Destroy(HandManager.Instance.currentPlant.gameObject);
            HandManager.Instance.currentPlant = null;
        }
        // ȡ��ѡ�еĲ���
        if (HandManager.Instance.shovel.activeSelf) {
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2);
            HandManager.Instance.ReturnShovel();
        }
    }
}