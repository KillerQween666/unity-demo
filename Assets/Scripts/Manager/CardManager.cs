using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

// 卡牌管理器：统一
public class CardManager : MonoBehaviour {

// 单例实例，全局唯一访问点
public static CardManager Instance { get; private set; }

// 植物类型与对应卡牌的映射表（快速查找用）
private Dictionary<PlantType, Card> plantCards = new Dictionary<PlantType, Card>();

// 场景中所有卡牌的数组
private Card[] cards;

// 初始化单例并收集所有卡牌
private void Awake() {
    Instance = this;
    CollectAllCards();
}

// 收集场景中所有卡牌，存入数组和字典
private void CollectAllCards() {
    // 获取场景中所有Card组件
    cards = FindObjectsOfType<Card>();
    // 按植物类型存入字典（避免重复）
    foreach (var card in cards) {
        if (!plantCards.ContainsKey(card.plantType)) {
            plantCards.Add(card.plantType, card);
        }
    }
}

// 通过植物类型获取对应的卡牌
public Card GetCardByPlantType(PlantType type) {
    // 尝试从字典中获取卡牌，存在则返回
    if (plantCards.TryGetValue(type, out Card card)) {
        return card;
    }
    return null;
}

// 禁用所有卡牌（设为不可用状态）
public void DisableCards() {
    foreach (var card in cards) {
        card.TransitionToDisable();
    }
}

// 启用所有卡牌（恢复到默认状态）
public void EnableCards() {
    foreach (var card in cards) {
        card.TransitionToDefaultState();
    }
}

// 将所有卡牌切换到选中状态
public void SelectCards() {
    foreach (var card in cards) {
        card.TransitionToSelectState();
    }
}
}