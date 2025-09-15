using FTRuntime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// С���ű�������С�����ƶ����������ż��뽩ʬ����ײ����
public class Car : MonoBehaviour {

    // SWF���������������ڲ���С���Ķ���������FTRuntime�����
    private SwfClipController swf;
    // С����2D���壨������������߼�����ǰ������δֱ��ʹ����������
    private Rigidbody2D rgd;

    // С���ƶ���Ŀ��X���꣨�����λ�ú�����С����
    public float targetXPosition = 10;

    // С���ƶ��ٶȣ���λ����λ/�룩
    public float moveSpeed = 3f;

    // С���Ƿ����ƶ�״̬�ı��
    bool isMove = false;

    // ��ʼ������ȡ�����������͸������
    private void Awake() {
        swf = GetComponent<SwfClipController>();
        rgd = GetComponent<Rigidbody2D>();
    }

    // ÿ֡���£����С�������ƶ�״̬�����������ƶ�������Ŀ��λ�ú�����
    private void Update() {
        if (isMove == true) {
            // ���ٶ������ƶ���Translate�����������꣬�ʺϼ������ƶ���
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            // С��X���곬��Ŀ��λ�ã���������
            if (transform.position.x > targetXPosition) {
                Destroy(gameObject);
            }
        }
    }

    // ������ײʱִ�У�����С���ƶ���������ײ���Ľ�ʬִ��"�ƿ�"�߼�
    private void OnTriggerEnter2D(Collider2D collision) {
        CarMove(); // ����С���ƶ������Ŷ�������Ч�����Ϊ�ƶ�״̬��

        // �����ײ�����ǽ�ʬ�����ý�ʬ��"ѹ��"����
        if (collision.CompareTag("Zombie")) {
            collision.GetComponent<Zombie>().Push();
        }
    }

    // С���ƶ��߼������Ŷ�������Ч�����Ϊ�ƶ�״̬
    void CarMove() {
        swf.Play(true); // ����SWF����������true��ʾѭ�����ţ��������������
        AudioManager.Instance.PlayClip(Config.carMove); // ����С���ƶ���Ч
        isMove = true; // ���С��Ϊ�ƶ�״̬������Update�е��ƶ��߼�
    }
}