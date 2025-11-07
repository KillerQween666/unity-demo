using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanglekelp : Plant {
    private bool isAttack = false;

    private Zombie dragZombie;

    public Transform fallWaterTransform;

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision != null) {
            if (collision.CompareTag("Zombie")) {
                if (collision.TryGetComponent<Zombie>(out var zombie)) {
                    if (isAttack == true) return;
                    isAttack = true;


                    animator.SetTrigger("attackTrigger");
                    zombie.SetAnimatorSpeed(0);

                    GetComponent<Collider2D>().enabled = false;
                    zombie.GetComponent<Collider2D>().enabled = false;

                    dragZombie = zombie;
                }
            }
        }
    }

    public override void PlantFun() {
        StartCoroutine(DragZombie(dragZombie));
    }

    IEnumerator DragZombie(Zombie zombie) {

        Vector3 position = transform.position, position2 = zombie.transform.position;

        position.y -= 2f;
        position2.y -= 2f;

        transform.DOMoveY(position.y, 1.3f);
        zombie.transform.DOMoveY(position2.y, 1.3f);

        yield return new WaitForSeconds(0.3f);

        ObjectPoolManager.Instance.PlayFallWaterSwfIEnumrator(fallWaterTransform);
        AudioManager.Instance.PlayClip(Config.enterWater);

        yield return new WaitForSeconds(1f);

        zombie.Dead();
        Dead();
    }
}
