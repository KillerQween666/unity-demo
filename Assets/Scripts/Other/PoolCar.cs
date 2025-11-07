using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolCar : Car {

    private Animator animator;

    private bool isEnterWater = false;
    private bool isExitWater = false;

    public float enterWaterXPosition = 1f;
    public float exitWaterXposition = 8f;

    public Transform fallWaterTransform;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    protected override void MoveUpdate() {
        base.MoveUpdate();

        if (isEnterWater == false) {
            if (transform.position.x > enterWaterXPosition) {
                isEnterWater = true;
                animator.SetBool("isWater", true);
                ObjectPoolManager.Instance.PlayFallWaterSwfIEnumrator(fallWaterTransform);
            }
        }
        
        if (isExitWater == false) {
            if (transform.position.x > exitWaterXposition) {
                isExitWater = true;
                animator.SetBool("isWater", false);
            }
        }
       
    }

    protected override void KillZombie(Zombie zombie) {
        zombie.Dead();
        animator.SetTrigger("eatTrigger");
    }

    protected override void StartMove() {
        animator.SetTrigger("moveTrigger");
        AudioManager.Instance.PlayClip(Config.poolCar);
    }
}
