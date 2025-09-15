using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// UI管理器（单例），集中管理所有UI组件的引用
public class UIManager : MonoBehaviour {

    // 单例实例，全局唯一访问点
    public static UIManager Instance { get; private set; }

    // 各种UI模块的引用（在Inspector中赋值）
    public ShovelUI shovelUI;       // 铲子UI
    public CardListUI cardListUI;   // 卡牌列表UI
    public PrepareUI prepareUI;     // 准备界面UI
    public FailUI failUI;           // 失败界面UI
    public WinUI winUI;             // 胜利界面UI
    public EndUI endUI;             // 结束界面UI
    public FlagMeterUI flagMeterUI; // 旗帜进度条UI
    public GameEndUI gameEndUI;     // 游戏结束总界面UI
    public MenuUI menuUI;           // 菜单界面UI

    // 初始化单例
    private void Awake() {
        Instance = this;
    }
}