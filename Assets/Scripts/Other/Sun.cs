using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ����ű�������������ƶ�������/��Ծ�����������ڡ�����ռ�������ػ���
public class Sun : MonoBehaviour {

    // �����ƶ���Ŀ��λ�õ�Ĭ��ʱ��������ռ�ʱʹ�ã�
    public float moveDuration = 1.5f;
    // �ռ�����ɻ�õ��������
    public int point = 50;

    // ���������ʱ�䣨��ʱδ�ռ����Զ����գ�
    private float liveTime = 5;
    // �������ʱ������¼�Ѵ��ʱ�䣩
    public float liveTimer;

    // �����Ƿ��ѱ�����ռ��ı�ǣ���ֹ�ظ��ռ���
    public bool isClick = false;

    // �������ײ�壨���ڼ��������Awake���Զ���ȡ��
    public Collider2D sunCollider2D;

    // ��ʼ������ȡ�������ײ�����
    private void Awake() {
        sunCollider2D = GetComponent<Collider2D>();
    }

    // ÿ֡���£����������ʱ�䣬��ʱδ�ռ�����յ������
    private void Update() {
        liveTimer += Time.deltaTime;
        // ���ʱ�䳬��������δ���ռ�����ֹ��ǰ�ƶ�����������
        if (liveTimer > liveTime && isClick == false) {
            transform.DOKill(); // ��ֹDOTween�ƶ��������������
            ObjectPoolManager.Instance.ReleaseSun(this.gameObject);
        }
    }

    // ���������ƶ���Ŀ��λ�ã����Զ����ɵ������䵽���棩
    public void LinearTo(Vector3 targetPos) {
        // �����ƶ�������ٶȣ�ȷ����ͬ�������ƶ��ٶ�һ��
        float distance = Vector3.Distance(transform.position, targetPos);
        float moveSpeed = 1.5f; // �̶��ƶ��ٶ�
        float actualMoveDuration = distance / moveSpeed; // ʵ���ƶ�ʱ��������ԽԶʱ��Խ����

        // ִ�������ƶ�����
        transform.DOMove(targetPos, actualMoveDuration);
    }

    // ������Ծʽ�ƶ���Ŀ��λ�ã�������·���������տ����ɵ����⣩
    public void JumpTo(Vector3 targetPos) {
        // ����·���м�㣨���ζ��㣬Y�����̧�ߣ�ģ����Ծ�켣��
        Vector3 centerPos = (transform.position + targetPos) / 2; // ·���е�
        centerPos.y += Random.Range(0.3f, 0.7f); // ���̧��Y�ᣬ��·������Ȼ

        // ������Ծ·���������㣺��� �� ���ζ��� �� �յ�
        Vector3[] pathPoints = new Vector3[] { transform.position, centerPos, targetPos };

        // ����·���ܳ��ȣ�ȷ���ƶ��ٶ�һ��
        float pathLength = 0;
        float moveSpeed = 0.75f; // ��Ծ�ƶ��ٶȣ������������������Ӿ�Ԥ�ڣ�
        for (int i = 0; i < pathPoints.Length - 1; i++) {
            pathLength += Vector3.Distance(pathPoints[i], pathPoints[i + 1]);
        }
        float totalDuration = pathLength / moveSpeed; // ��Ծ��ʱ��

        // ִ�л���·���ƶ���CatmullRom������·����ƽ����OutQuad�����ý�β���٣�
        transform.DOPath(pathPoints, totalDuration, PathType.CatmullRom)
            .SetEase(Ease.OutQuad);
    }

    // �������ռ��¼�����ҵ������ʱ������
    public void OnClick() {
        if (isClick == true) return; // ���ռ����������ظ�ִ��
        isClick = true; // ���Ϊ���ռ�

        AudioManager.Instance.PlayClip(Config.sunClick); // �����ռ��������Ч
        sunCollider2D.enabled = false; // ������ײ�壬��ֹ�ٴα����

        transform.DOKill(); // ��ֹ��ǰ�ƶ�����������֮ǰ�����Ի�����Ծ��

        // ���������������λ���ƶ����̶�Ŀ��λ�ã�(-5.62f, 4.31f, 0)��
        transform.DOMove(new Vector3(-5.62f, 4.31f, 0), moveDuration)
            .SetUpdate(true) // ��ʹ��Ϸ��ͣ������Ҳ������ȷ���ռ�����������
            .SetEase(Ease.Linear) // �����ƶ������ٵ���Ŀ��
            .OnComplete( // �ƶ���ɺ󣬽�������յ������
            () => {
                ObjectPoolManager.Instance.ReleaseSun(this.gameObject);
            });

        SunManager.Instance.AddSun(point); // ������ҵ��������
    }
}