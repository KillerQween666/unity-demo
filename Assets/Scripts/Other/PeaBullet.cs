using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �㶹�ӵ��ű��������ӵ��ƶ����������ڡ���ײ�˺�������ػ���
public class PeaBullet : MonoBehaviour {

    // �ӵ��ƶ��ٶȣ���λ����λ/�룬���ҷ��У�
    public float speed = 3;
    // �ӵ������������н�ʬʱ��ɵ��˺�ֵ��
    public float atkValue = 30;

    // �ӵ������ʱ�䣨��ʱ���Զ����գ������ڴ�й©��
    public float liveTime = 5;
    // �ӵ�����ʱ������¼�Ѵ��ʱ�䣩
    public float liveTimer;

    // �ӵ��Ƿ�������Ŀ��ı�ǣ���ֹ�ظ������˺���
    public bool isAttack = false;
    // �ӵ��Ƿ��ѻ��յı�ǣ���϶���أ������ظ����գ�
    public bool isRelease = false;

    // �̶�ʱ�䲽���£������ӵ��ƶ���FixedUpdate�ʺ���������߼����ƶ����ȶ���
    private void FixedUpdate() {
        // �ӵ����������ƶ��������������꣬������תӰ�죩
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    // ÿ֡���£�����ӵ����ʱ�䣬��ʱδ���������
    private void Update() {
        liveTimer += Time.deltaTime;
        // ���ʱ�䳬�����ޣ���δ���С�δ����ʱ�����յ������
        if (liveTimer > liveTime && isAttack == false && isRelease == false) {
            ObjectPoolManager.Instance.ReleasePeaBullet(this.gameObject);
        }
    }

    // ��ײ�������ӵ����н�ʬʱִ���˺��߼�
    private void OnTriggerEnter2D(Collider2D collision) {
        // ֻ��"Zombie"��ǩ�Ķ�����Ч
        if (collision.CompareTag("Zombie")) {
            if (isAttack == true) return; // �����й��������ظ��˺�
            isAttack = true; // ���Ϊ������

            // �����ӵ���������Ч�����Ӷ���ػ�ȡ���ӣ�
            ObjectPoolManager.Instance.PlayPeaBulletParticalIEnumrator(transform);

            // �������һ��������Ч��������Ч�����ԣ�
            var clips = new[] { Config.splat, Config.splat2, Config.splat3 };
            AudioManager.Instance.PlayClip(clips[Random.Range(0, 3)]);

            // �����еĽ�ʬ����˺�
            collision.GetComponent<Zombie>().TakeDamage(atkValue);

            // �˺�������ɺ󣬽��ӵ����յ������
            ObjectPoolManager.Instance.ReleasePeaBullet(this.gameObject);
        }
    }

}