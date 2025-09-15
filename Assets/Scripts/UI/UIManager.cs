using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI�������������������й�������UI���������
public class UIManager : MonoBehaviour {

    // ����ʵ����ȫ��Ψһ���ʵ�
    public static UIManager Instance { get; private set; }

    // ����UIģ������ã���Inspector�и�ֵ��
    public ShovelUI shovelUI;       // ����UI
    public CardListUI cardListUI;   // �����б�UI
    public PrepareUI prepareUI;     // ׼������UI
    public FailUI failUI;           // ʧ�ܽ���UI
    public WinUI winUI;             // ʤ������UI
    public EndUI endUI;             // ��������UI
    public FlagMeterUI flagMeterUI; // ���Ľ�����UI
    public GameEndUI gameEndUI;     // ��Ϸ�����ܽ���UI
    public MenuUI menuUI;           // �˵�����UI

    // ��ʼ������
    private void Awake() {
        Instance = this;
    }
}