using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ��Ϸ������������ͳ����Ϸ���̣���ʼ����ͣ�����������ã���������ƺ��������
public class GameManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ㣨�ⲿͨ���˵�����Ϸ�����߼���
    public static GameManager Instance { get; private set; }

    // ��Ϸ�Ƿ�����ı�ǣ���ֹ�ظ����������߼���
    bool isGameEnd = false;

    // ��Ϸ�Ƿ���ͣ�ı��
    bool isPause = false;

    // ��ʼ��������ȷ��������ֻ��һ��GameManager��
    private void Awake() {
        Instance = this;
    }

    // ����������루�ո���ͣ/�ָ���ESC��/�رղ˵���
    private void Update() {
        // ��Ϸ�����󣬲���Ӧ�κ�����
        if (UIManager.Instance.gameEndUI.isGameEnd == true) return;

        // �ո������ͣ/�ָ���Ϸ���˵���ʾʱ����Ӧ��
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (UIManager.Instance.menuUI.menuImage.activeSelf) return;

            if (isPause) {
                ResumeGame(); // ����ͣ��ָ�
            }
            else {
                AudioManager.Instance.PlayClip(Config.pause); // ������ͣ��Ч
                PauseGame(); // δ��ͣ����ͣ
            }
        }

        // ESC������/�رղ˵�
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (UIManager.Instance.menuUI.menuImage.activeSelf) {
                UIManager.Instance.menuUI.Hide(); // �˵�����ʾ������
            }
            else {
                UIManager.Instance.menuUI.Show(); // �˵�δ��ʾ���
            }
        }
    }

    // ��Ϸ����ʱִ�У��Զ�������
    private void Start() {
        GameStart();
    }

    // ��Ϸ��ʼ�����߼�����������ƶ�����
    void GameStart() {
        StartCoroutine(CameraMove());
    }

    // ����ƶ�Э�̣���Ϸ��ʼʱ�����������
    IEnumerator CameraMove() {
        // ��¼�����ʼλ�ã����ں�����ԭ��
        Vector3 position = Camera.main.transform.position;

        // �ȴ�0.5�루�ӳ����������⿪��̫ͻأ��
        yield return new WaitForSeconds(0.5f);

        // ��������ƶ���(4,0,-10)������ʱ��1.5��
        Camera.main.transform.DOMove(new Vector3(4, 0, -10), 1.5f).SetEase(Ease.Linear);

        // �ȴ�2�루ͣ��ʱ�䣬����ҿ��峡����
        yield return new WaitForSeconds(2f);

        // ����ƻس�ʼλ�ã����������󴥷�׼������
        Camera.main.transform.DOMove(position, 1.5f).SetEase(Ease.Linear).OnComplete(OnCameraMoveComplete);
    }

    // ����ƶ�������ɺ�ִ�У���ʾ׼������
    void OnCameraMoveComplete() {
        AudioManager.Instance.PlayClip(Config.prepare); // ����׼����Ч
        UIManager.Instance.prepareUI.Show(); // ��ʾ׼������
    }

    // ׼��������ɺ�ִ�У���PrepareUI�Ķ����¼����ã�����ʽ������Ϸ
    public void OnPrepareUIComplete() {
        SunManager.Instance.StartProduce(); // ������������
        ZombieManager.Instance.StartSpawn(); // ��ʼ���ɽ�ʬ
        UIManager.Instance.cardListUI.Show(); // ��ʾ�����б�
        CarManager.instance.ShowCarList(); // ��ʾС���б�
        AudioManager.Instance.PlayBgm(Config.bgm1); // ���ű�������
        UIManager.Instance.menuUI.ButtonShow(); // ��ʾ�˵���ť
    }

    // ��Ϸʧ�ܽ������ⲿ���ã��罩ʬͻ�Ʒ���ʱ��
    public void GameEndFail() {
        if (isGameEnd == true) return; // �ѽ������ظ�ִ��
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // ���ز˵���ť
        UIManager.Instance.failUI.Show(); // ��ʾʧ�ܽ���
        AudioManager.Instance.PlayClip(Config.loseMusic); // ����ʧ����Ч
    }

    // ��Ϸʤ���������ⲿ���ã���������н�ʬʱ��
    public void GameEndSuccess() {
        if (isGameEnd == true) return; // �ѽ������ظ�ִ��
        isGameEnd = true;

        UIManager.Instance.menuUI.ButtonHide(); // ���ز˵���ť
        UIManager.Instance.winUI.Show(); // ��ʾʤ������
        AudioManager.Instance.PlayClip(Config.winMusic); // ����ʤ����Ч
    }

    // ��ͣ��Ϸ������ʱ������Ϊ0��������Ϸ���̣�
    public void PauseGame() {
        isPause = true;
        Time.timeScale = 0; // ʱ������Ϊ0�����л���Time���߼������ʱ������������ͣ
    }

    // �ָ���Ϸ������ʱ������Ϊ1���ָ���Ϸ���̣�
    public void ResumeGame() {
        isPause = false;
        Time.timeScale = 1; // ʱ�����Żָ�Ϊ1����Ϸ��������
    }

    // ������Ϸ�����¼��ص�ǰ�������ص���ʼ״̬��
    public void ResetGame() {
        ResumeGame(); // �Ȼָ���Ϸ�����ⳡ������ʱʱ�������쳣��
        DOTween.KillAll(); // ��ֹ����DOTween��������ֹ��������Ӱ���³�����

        AudioManager.Instance.PlayClip(Config.buttonClick); // ���Ű�ť�����Ч
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // ���¼��ص�ǰ����
    }
}