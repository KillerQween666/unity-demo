using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Enemy : MonoBehaviour {

    public float HP = 100; // ½©Ê¬µÄÉúÃüÖµ

    public virtual void TakeDamage(float damage, int hurtType = 0) {
        HP -= damage;
    }
}
