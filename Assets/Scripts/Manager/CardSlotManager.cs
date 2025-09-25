using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌卡槽管理器（单例）：管理卡牌在卡槽的添加、移除和位置调整
public class CardSlotManager : MonoBehaviour {
    public static CardSlotManager Instance { get; private set; } // 单例实例，供全局调用

    public int maxSlotCount = 8; // 卡槽最大容量
    public List<Transform> slotTransformList = new List<Transform>(); // 卡槽位置列表（每个位置对应一个卡槽）
    private List<Card> slotCards = new List<Card>(); // 当前在卡槽中的卡牌

    public GameObject readyOKButton; // 卡槽满时显示的"准备完成"按钮
    public GameObject readyNOButton; // 卡槽未满时显示的"未准备"按钮
    public Transform plantCardList; // 卡牌放入卡槽后的父物体

    private bool isReadyOK = false; // 标记卡槽是否已准备完成（满槽状态）

    private void Awake() {
        Instance = this; // 初始化单例
    }

    // 将卡牌移到卡槽
    public void MoveToSlot(Card card) {
        if (slotCards.Count >= maxSlotCount) return; // 卡槽已满，不处理

        // 卡槽即将放满时，切换准备按钮显示
        if (slotCards.Count == maxSlotCount - 1 && !isReadyOK) {
            isReadyOK = true;
            readyOKButton.SetActive(true);
            readyNOButton.SetActive(false);
        }

        // 卡牌移动到对应卡槽位置，完成后设置父物体
        card.transform.DOMove(slotTransformList[slotCards.Count].position, 0.3f).OnComplete(() => {
            card.transform.SetParent(plantCardList);
        });

        slotCards.Add(card); // 添加到卡槽列表
    }

    // 将卡牌从卡槽移回卡池
    public void MoveToPool(Card card) {
        if (!slotCards.Contains(card)) return; // 卡牌不在卡槽，不处理

        // 若之前已准备完成，移除后切换按钮显示
        if (isReadyOK) {
            isReadyOK = false;
            readyOKButton.SetActive(false);
            readyNOButton.SetActive(true);
        }

        // 卡牌回到原始父物体和位置
        card.transform.SetParent(card.originParent);
        card.GetComponent<RectTransform>().DOAnchorPos(card.originPosition, 0.3f);

        // 移除卡牌并调整后续卡牌位置
        int removeIndex = slotCards.IndexOf(card);
        slotCards.RemoveAt(removeIndex);

        // 后面的卡牌向前补位，播放移动动画
        for (int i = removeIndex; i < slotCards.Count; i++) {
            slotCards[i].transform.DOMove(slotTransformList[i].position, 0.3f);
        }
    }

}