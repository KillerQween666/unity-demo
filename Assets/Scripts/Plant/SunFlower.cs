using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFlower : Plant {
    // ��������ļ��ʱ�䣨�룩
    public float produceDuration = 5;
    // ��������ļ�ʱ��
    private float produceTimer;

    // ����Ԥ���壨�Ӷ���ػ�ȡ�������ֶ�ʵ������
    public Sun sunPrefab;

    // ������Ծ����С/�����루���������
    public float jumpMinDistance = 0.6f;
    public float jumpMaxDistance = 2f;

    // ����״̬�߼�����ʱ����ʱ�䴥�����⶯���������¼�����ProduceSun��
    protected override void EnableUpdate() {
        produceTimer += Time.deltaTime;

        if (produceTimer > produceDuration) {
            produceTimer = 0;
            animator.SetTrigger("glowTrigger");
        }
    }

    // �������⣨�ɷ��⶯�����¼����ã�
    public override void PlantFun() {
        // �����ʼλ�ã����տ��Ϸ�������Z������ڵ���
        Vector3 position = transform.position;
        position.z = -4;

        // �Ӷ���ػ�ȡ���Ⲣ���ó�ʼλ��
        GameObject obj = ObjectPoolManager.Instance.GetSun();
        obj.transform.position = position;

        // ���������Ծ��Ŀ��λ�ã�50%����50%���ң�
        float distance = Random.Range(jumpMinDistance, jumpMaxDistance);
        distance = Random.Range(0, 2) < 1 ? -distance : distance;
        position.x += distance;

        // ����������Ŀ��λ��
        obj.GetComponent<Sun>().JumpTo(position);
    }

}