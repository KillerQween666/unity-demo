using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 卡牌列表UI，控制卡牌显示和点击取消逻辑
public class CardListUI : MonoBehaviour {

    // 显示卡牌列表（带动画），显示完成后启用卡牌
    public void Show() {
        GetComponent<RectTransform>().DOLocalMoveY(452, 0.5f)
            .OnComplete(CardManager.Instance.EnableCards);
    }

    // 点击卡牌列表时：取消当前选中的植物或铲子
    public void OnClick() {
        // 取消选中的植物
        if (HandManager.Instance.currentPlant != null) {
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2);
            Destroy(HandManager.Instance.currentPlant.gameObject);
            HandManager.Instance.currentPlant = null;
        }
        // 取消选中的铲子
        if (HandManager.Instance.shovel.activeSelf) {
            AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.tap : Config.tap2);
            HandManager.Instance.ReturnShovel();
        }
    }
}