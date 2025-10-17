using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlashUI : MonoBehaviour {

    public Image flashImage;

    private Color crozeColor = new Color(0.4f, 0.5568f, 1, 0.1f);
    private Color doomColor = new Color(0.85f, 0, 1, 0.1f);

    private void Start() {
        flashImage.color = new Color(1, 1, 1, 0);
    }

    public void PlayFlash(int flashType) {
        if (flashType == 0) {
            StartCoroutine(CrozeFlash());
        } else if (flashType == 1) {
            StartCoroutine(DoomFlash());
        }

    }

    IEnumerator CrozeFlash() {
        flashImage.color = crozeColor;
        yield return new WaitForSeconds(0.2f);
        flashImage.color = new Color(1, 1, 1, 0);
    }

    IEnumerator DoomFlash() {
        flashImage.color = doomColor;
        yield return new WaitForSeconds(0.2f);
        flashImage.color = new Color(1, 1, 1, 0);
    }
}
