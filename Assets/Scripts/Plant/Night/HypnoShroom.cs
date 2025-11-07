using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypnoShroom : Plant {

    private bool isUseHypno = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null) {
            if (collision.CompareTag("Zombie")) {
                if (collision.TryGetComponent<Zombie>(out var zombie)) {
                    if (isUseHypno) return;
                    isUseHypno = true;

                    ObjectPoolManager.Instance.PlayHypnoParticalIEnumrator(transform);
                    AudioManager.Instance.PlayClip(Config.hypnoZombie);
                    zombie.Hypnoed();

                    Dead();
                }
            }
        }
    }
}
