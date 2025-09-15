using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ƹ������������ռ�����������ֲ�￨�ƣ����ƿ��Ƶ�����/����״̬
public class CardManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ㣨�ⲿͨ���˻�ȡ��������
    public static CardManager Instance { get; private set; }

    // �洢ֲ���������Ӧ���Ƶ�ӳ�䣨ͨ��ֲ�����Ϳ����ҵ����ƣ�
    private Dictionary<PlantType, Card> plantCards = new Dictionary<PlantType, Card>();

    // �洢���������п��Ƶ�����
    private Card[] cards;

    // ��ʼ���������ռ����������п���
    private void Awake() {
        Instance = this;
        CollectAllCards();
    }

    // �ռ����������й���Card�ű��Ŀ��ƣ��������ֵ������
    private void CollectAllCards() {
        // �ҵ�����������Card���
        cards = FindObjectsOfType<Card>();
        // �������ƣ���ֲ�����ʹ����ֵ䣨�����ظ����ͬһ���Ϳ��ƣ�
        foreach (var card in cards) {
            if (!plantCards.ContainsKey(card.plantType)) {
                plantCards.Add(card.plantType, card);
            }
        }
    }

    // ͨ��ֲ�����ͻ�ȡ��Ӧ�Ŀ��ƣ��ⲿ���ã�������������ÿ�����Ϣ��
    public Card GetCardByPlantType(PlantType type) {
        // ���Դ��ֵ��л�ȡ���ƣ������򷵻أ������ڷ���null
        if (plantCards.TryGetValue(type, out Card card)) {
            return card;
        }
        return null;
    }

    // �������п��ƣ����ÿ���״̬Ϊ���ã��޷����ʹ�ã�
    public void DisableCards() {
        foreach (var card in cards) {
            card.cardState = CardState.Disable;
        }
    }

    // �������п��ƣ��������л���"׼������"״̬���ɵ��ʹ�ã�
    public void EnableCards() {
        foreach (var card in cards) {
            card.TransitionToReady();
        }
    }
}