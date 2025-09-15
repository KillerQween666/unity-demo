using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ײ����ű�����������������������ײ�󣬵�������ϵͳ�Ĳ�����ֹͣ���䡢������ת�ȣ�
public class Partical : MonoBehaviour {

    // �������������巢����ײʱ������ÿ��ײһ��ִ��һ�Σ�
    private void OnParticleCollision(GameObject other) {
        // ��ȡ��ǰ�����ϵ�����ϵͳ���
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();

        // 1. �����������������ڵ���תЧ����������ײ����������ת��
        var rotationOverLifetimeModule = particleSystem.rotationOverLifetime;
        rotationOverLifetimeModule.enabled = false;

        // 2. �������ӳ�ʼ��ת����Ϊ0�ȣ�����3D��ת��ȷ����ײ��������תͳһ��
        var mainModule = particleSystem.main;
        mainModule.startRotation = new ParticleSystem.MinMaxCurve(0f); // ��ʼ��ת�̶�Ϊ0
        mainModule.startRotation3D = false; // �ر�3D��ת��ʹ��2Dƽ����ת��

        // 3. ����������ײ��ĵ���Ч������ײ��ֱ��ֹͣ����������
        var collisionmModule = particleSystem.collision;
        collisionmModule.bounce = 0f; // ����ϵ����Ϊ0���޵���

        // 4. ֹͣ���ӷ��䣨��ײ�������������ӣ��������Ѵ��ڵ�������Ȼ��ʧ��
        var main = particleSystem.emission;
        main.enabled = false;
    }
}