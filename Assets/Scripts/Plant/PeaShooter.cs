using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : Plant {
    // �����㶹�ļ��ʱ�䣨�룩
    public float shootDuration;
    // �����㶹�ļ�ʱ��
    private float shootTimer;

    // �㶹����㣨�����㶹�����������
    public Transform shootPointTransform;
    // ��⽩ʬ����ײ�壨�޶���ⷶΧ��
    public Collider2D coll2D;

    // ����״̬�߼�����ʱ����ʱ���⽩ʬ���н�ʬ�򴥷���������
    protected override void EnableUpdate() {
        shootTimer += Time.deltaTime;

        if (shootTimer > shootDuration) {
            shootTimer = 0;

            // �����ײ�巶Χ�ڵ����ж����������Ѿ�ר�������˽�ʬ�㣩
            Bounds bounds = coll2D.bounds;
            Collider2D[] hitColliders = Physics2D.OverlapBoxAll(
                bounds.center,
                bounds.size,
                coll2D.transform.rotation.eulerAngles.z,
                LayerMask.GetMask("Zombie")
            );

            // ֻҪ��⵽�κ���ײ�壨��ʬ�����ʹ�����������
            if (hitColliders.Length > 0) // ����Ƿ��м�⵽�Ķ���
            {
                grandAnimator.SetTrigger("attackTrigger");
            }
        }
    }

    // �����㶹���ɹ����������¼����ã�
    public override void PlantFun() {
        // ����������ַ�����Ч
        AudioManager.Instance.PlayClip(Random.value > 0.5f ? Config.shoot : Config.shoot2);

        // �Ӷ���ػ�ȡ�㶹�����÷���λ��
        GameObject obj = ObjectPoolManager.Instance.GetPeaBullet();
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.transform.position = shootPointTransform.position;
    }

}