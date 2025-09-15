using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌管理器，负责收集、管理所有植物卡牌，控制卡牌的启用/禁用状态
public class CardManager : MonoBehaviour {

    // 单例实例，全局唯一访问点（外部通过此获取管理器）
    public static CardManager Instance { get; private set; }

    // 存储植物类型与对应卡牌的映射（通过植物类型快速找到卡牌）
    private Dictionary<PlantType, Card> plantCards = new Dictionary<PlantType, Card>();

    // 存储场景中所有卡牌的数组
    private Card[] cards;

    // 初始化单例，收集场景中所有卡牌
    private void Awake() {
        Instance = this;
        CollectAllCards();
    }

    // 收集场景中所有挂载Card脚本的卡牌，并存入字典和数组
    private void CollectAllCards() {
        // 找到场景中所有Card组件
        cards = FindObjectsOfType<Card>();
        // 遍历卡牌，按植物类型存入字典（避免重复添加同一类型卡牌）
        foreach (var card in cards) {
            if (!plantCards.ContainsKey(card.plantType)) {
                plantCards.Add(card.plantType, card);
            }
        }
    }

    // 通过植物类型获取对应的卡牌（外部调用，比如根据类型拿卡牌信息）
    public Card GetCardByPlantType(PlantType type) {
        // 尝试从字典中获取卡牌，存在则返回，不存在返回null
        if (plantCards.TryGetValue(type, out Card card)) {
            return card;
        }
        return null;
    }

    // 禁用所有卡牌（设置卡牌状态为禁用，无法点击使用）
    public void DisableCards() {
        foreach (var card in cards) {
            card.cardState = CardState.Disable;
        }
    }

    // 启用所有卡牌（将卡牌切换到"准备就绪"状态，可点击使用）
    public void EnableCards() {
        foreach (var card in cards) {
            card.TransitionToReady();
        }
    }
}