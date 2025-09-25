using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardChooserUI : MonoBehaviour {
    public GameObject cardChooser;
    public GameObject cardPlantList;

    public void Show() {
        GetComponent<RectTransform>().DOLocalMoveY(-75.6f, 0.2f);
    }

    public void Hide() {
        cardChooser.SetActive(false);
    }
}
