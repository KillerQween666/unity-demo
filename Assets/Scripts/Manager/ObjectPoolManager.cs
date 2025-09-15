using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// ����ع�������ͨ������ظ��ø�Ƶ����/���ٵĶ����ӵ������⡢���ӡ���ƵԴ����������������
public class ObjectPoolManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ㣨�ⲿͨ���˻�ȡ/���ն���
    public static ObjectPoolManager Instance { get; private set; }

    // ��������Ԥ���壨��Inspector��ֵ�����ڴ�������س�ʼʵ����
    public GameObject peaBulletPartical;  // �㶹�ӵ�����Ч��Ԥ����
    public GameObject peaBullet;          // �㶹�ӵ�Ԥ����
    public GameObject sun;                // ����Ԥ����
    public GameObject headEmissionPartical;// ͷ����������Ч��Ԥ����
    public GameObject handEmissionPartical;// �ֲ���������Ч��Ԥ����
    private GameObject audioSource;       // ��ƵԴ���󣨶�̬����������Ԥ���壩

    // ��������Ӧ�Ķ���أ��������Ĵ��������á����٣�
    private ObjectPool<GameObject> peaBulletParticalPool;
    private ObjectPool<GameObject> peaBulletPool;
    private ObjectPool<GameObject> sunPool;
    private ObjectPool<GameObject> headEmissionParticalPool;
    private ObjectPool<GameObject> handEmissionParticalPool;
    private ObjectPool<GameObject> sourcePool; // ��ƵԴ�����

    // ��ʼ������
    private void Awake() {
        Instance = this;
    }

    // ��Ϸ����ʱ��ʼ�����ж���أ����ô�������ȡ�����ա����ٵĻص��߼���
    private void Start() {
        // �㶹�ӵ����ӳأ���ʼ10�������300��������������������ٶ���
        peaBulletParticalPool = new ObjectPool<GameObject>(
            CreatePeaBulletPartical,  // ��������ķ���
            ActionOnGet,               // �ӳػ�ȡ����ʱ�Ļص�
            ActionOnRelease,           // ���ն��󵽳�ʱ�Ļص�
            ActionOnDestroy,           // ���󳬳����������ʱ�����ٻص�
            true, 10, 300              // ��������������ʼ�������������
        );

        // �㶹�ӵ��أ�ͬ�ϣ���ȡʱ���������ӵ�״̬
        peaBulletPool = new ObjectPool<GameObject>(
            CreatePeaBullet,
            ActionOnGetPeaBullet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // ����أ�ͬ�ϣ���ȡʱ������������״̬
        sunPool = new ObjectPool<GameObject>(
            CreateSun,
            ActionOnGetSun,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // ͷ�����ӳأ�����������߼�
        headEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHeadEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // �ֲ����ӳأ�����������߼�
        handEmissionParticalPool = new ObjectPool<GameObject>(
            CreateHandEmissionPartical,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );

        // ��ƵԴ�أ�����������߼�
        sourcePool = new ObjectPool<GameObject>(
            CreateSource,
            ActionOnGet,
            ActionOnRelease,
            ActionOnDestroy,
            true, 10, 300
        );
    }

    // �����㶹�ӵ�ʵ��������ص��ã��������ӵ���
    GameObject CreatePeaBullet() {
        return Instantiate(peaBullet);
    }

    // �����㶹�ӵ�����ʵ��
    GameObject CreatePeaBulletPartical() {
        return Instantiate(peaBulletPartical);
    }

    // ��������ʵ��
    GameObject CreateSun() {
        return Instantiate(sun);
    }

    // ����ͷ������ʵ��
    GameObject CreateHeadEmissionPartical() {
        return Instantiate(headEmissionPartical);
    }

    // �����ֲ�����ʵ��
    GameObject CreateHandEmissionPartical() {
        return Instantiate(handEmissionPartical);
    }

    // ������ƵԴʵ������̬���AudioSource���������Ԥ���壩
    GameObject CreateSource() {
        GameObject obj = new GameObject("AudioSource"); // �������󣬷������
        obj.AddComponent<AudioSource>();                // �����������ƵԴ���
        return obj;
    }

    // ������ȡ�ص����ӳ����ö���ʱ���������
    void ActionOnGet(GameObject obj) {
        obj.SetActive(true);
    }

    // �㶹�ӵ���ȡ�ص����������ǰ�������ӵ�״̬�����⸴�þ�״̬��
    void ActionOnGetPeaBullet(GameObject obj) {
        PeaBullet peaBullet = obj.GetComponent<PeaBullet>();
        peaBullet.liveTimer = 0;    // �����������ڼ�ʱ��
        peaBullet.isAttack = false; // ���ù���״̬
        peaBullet.isRelease = false;// ���û���״̬
        obj.SetActive(true);        // �������
    }

    // �����ȡ�ص����������ǰ����������״̬
    void ActionOnGetSun(GameObject obj) {
        Sun sun = obj.GetComponent<Sun>();
        sun.liveTimer = 0;          // �����������ڼ�ʱ��
        sun.isClick = false;        // ���õ��״̬
        sun.sunCollider2D.enabled = true; // ������ײ�壨ȷ���ܱ��ռ���
        obj.SetActive(true);        // �������
    }

    // �������ջص������ն��󵽳�ʱ�����ö��󣨲����٣��������ã�
    void ActionOnRelease(GameObject obj) {
        obj.SetActive(false);
    }

    // �������ٻص������󳬳����������ʱ���������٣��ͷ��ڴ棩
    void ActionOnDestroy(GameObject obj) {
        Destroy(obj);
    }

    // �����ṩ����ȡ�㶹�ӵ����ⲿ���ã����㶹���ַ���ʱ��
    public GameObject GetPeaBullet() {
        return peaBulletPool.Get();
    }

    // �����ṩ�������㶹�ӵ����ⲿ���ã����ӵ����л�ʱ��
    public void ReleasePeaBullet(GameObject gameObject) {
        peaBulletPool.Release(gameObject);              // ���յ������
        gameObject.GetComponent<PeaBullet>().isRelease = true; // ���Ϊ�ѻ���
    }

    // �����ṩ����ȡ�㶹�ӵ�����
    public GameObject GetPeaBulletPartical() {
        return peaBulletParticalPool.Get();
    }

    // �����ṩ�������㶹�ӵ�����
    public void ReleasePeaBulletPartical(GameObject gameObject) {
        peaBulletParticalPool.Release(gameObject);
    }

    // �����ṩ����ȡ���⣨�ⲿ���ã������տ���������ʱ��
    public GameObject GetSun() {
        return sunPool.Get();
    }

    // �����ṩ���������⣨�ⲿ���ã������ⱻ�ռ���ʱ��
    public void ReleaseSun(GameObject gameObject) {
        sunPool.Release(gameObject);
    }

    // �����ṩ����ȡͷ������
    public GameObject GetHeadEmissionPartical() {
        return headEmissionParticalPool.Get();
    }

    // �����ṩ������ͷ������
    public void ReleaseHeadEmissionPartical(GameObject gameObject) {
        headEmissionParticalPool.Release(gameObject);
    }

    // �����ṩ����ȡ�ֲ�����
    public GameObject GetHandEmissionPartical() {
        return handEmissionParticalPool.Get();
    }

    // �����ṩ�������ֲ�����
    public void ReleaseHandEmissionPartical(GameObject gameObject) {
        handEmissionParticalPool.Release(gameObject);
    }

    // �����ṩ����ȡ��ƵԴ���ⲿ���ã��粥����Чʱ��
    public GameObject GetSource() {
        return sourcePool.Get();
    }

    // �����ṩ��������ƵԴ���ⲿ���ã�����Ч������ɺ�
    public void ReleaseSource(GameObject gameObject) {
        sourcePool.Release(gameObject);
    }

    // �����ṩ�������㶹�ӵ�����Ч��������Э�̣��������Ӳ���ʱ����
    public void PlayPeaBulletParticalIEnumrator(Transform transform) {
        StartCoroutine(PlayPeaBulletPartical(transform));
    }

    // �����ṩ������ͷ������Ч��������Ⱦ�㼶���������������ڵ���
    public void PlayHeadEmissionIEnumrator(Transform transform, int sort) {
        StartCoroutine(PlayHeadEmissionPartical(transform, sort));
    }

    // �����ṩ�������ֲ�����Ч��������Ⱦ�㼶������
    public void PlayHandEmissionIEnumrator(Transform transform, int sort) {
        StartCoroutine(PlayHandEmissionPartical(transform, sort));
    }

    // �㶹�ӵ����Ӳ���Э�̣��������Ӻ󣬵ȴ�0.6���ٻ���
    public IEnumerator PlayPeaBulletPartical(Transform transform) {
        GameObject obj = GetPeaBulletPartical();       // �ӳػ�ȡ���Ӷ���
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        particle.transform.position = transform.position; // ��ΪĿ��λ�ã����ӵ����е㣩
        particle.Play();                                // ��������

        yield return new WaitForSeconds(0.6f);          // �ȴ����Ӳ������

        particle.Clear();                               // �����������
        ReleasePeaBulletPartical(obj);                  // �������ӵ���
    }

    // ͷ�����Ӳ���Э�̣���ָ���㼶���ţ�������ɺ����
    public IEnumerator PlayHeadEmissionPartical(Transform transform, int sort) {
        GameObject obj = GetHeadEmissionPartical();
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // ����������Ⱦ�㼶
        particle.transform.position = transform.position;
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration); // �ȴ�������Ȼ�������

        particle.Clear();
        ReleaseHeadEmissionPartical(obj);
    }

    // �ֲ����Ӳ���Э�̣���ָ���㼶���ţ�������ɺ����
    public IEnumerator PlayHandEmissionPartical(Transform transform, int sort) {
        GameObject obj = GetHandEmissionPartical();
        ParticleSystem particle = obj.GetComponent<ParticleSystem>();
        ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = sort;                   // ����������Ⱦ�㼶
        particle.transform.position = transform.position;
        particle.Play();

        yield return new WaitForSeconds(particle.main.duration); // �ȴ�������Ȼ�������

        particle.Clear();
        ReleaseHandEmissionPartical(obj);
    }
}