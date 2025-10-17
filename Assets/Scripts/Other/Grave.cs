using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grave : MonoBehaviour {
    public GameObject goodGrave;
    public GameObject badGrave;
    public int row;

    public void BadGrave() {
        goodGrave.SetActive(false);
        badGrave.SetActive(true);
    }

    public void GoodGrave() {
        goodGrave.SetActive(true);
        badGrave.SetActive(false);
    }
}
