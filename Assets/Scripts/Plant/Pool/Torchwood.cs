using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torchwood : Plant {
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null) {
            if (collision.CompareTag("PeaBullet")) {
                if (collision.TryGetComponent<SnowPeaBullet>(out var snowBullet)) {

                    // 从对象池获取豌豆并设置发射位置
                    GameObject obj = ObjectPoolManager.Instance.GetPeaBullet();
                    PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
                    peaBullet.transform.position = snowBullet.transform.position;

                    StartCoroutine(SnowBecomePeabullet(obj));

                    ObjectPoolManager.Instance.ReleaseSnowPeaBullet(snowBullet.gameObject);
                } else if (collision.TryGetComponent<PeaBullet>(out var bullet)) {
                    // 从对象池获取豌豆并设置发射位置
                    GameObject obj = ObjectPoolManager.Instance.GetFirePeaBullet();
                    PeaBullet peaBullet = obj.GetComponent<PeaBullet>();

                    Vector3 position = bullet.transform.position;
                    position.y += 0.6f;
                    position.x -= 0.4f;

                    peaBullet.transform.position = position;

                    if (bullet.moveTween.IsActive()) {
                        peaBullet.transform.DOKill();

                        Vector3 targetPosition = bullet.targetPos;
                        targetPosition.y += 0.6f;
                        targetPosition.x -= 0.4f;

                        float duration = bullet.moveTween.Duration() - bullet.moveTween.Elapsed();

                        peaBullet.transform.DOMove(targetPosition, duration);
                    }

                    ObjectPoolManager.Instance.ReleasePeaBullet(bullet.gameObject);
                }
            }
        }
    }

    IEnumerator SnowBecomePeabullet(GameObject obj) {
        obj.tag = "Untagged";

        yield return new WaitForSeconds(0.15f);

        obj.tag = "PeaBullet";
    }
}
