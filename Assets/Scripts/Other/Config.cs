using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 配置类，存储项目中所有音频资源的路径常量（集中管理，方便统一修改和调用）
public class Config : MonoBehaviour {

    // 背景音乐路径
    public const string bgm1 = "Audio/Music/bgm1";

    // 音效路径：阳光收集音效
    public const string sunClick = "Audio/Sound/points";
    // 音效路径：游戏胜利音效
    public const string winMusic = "Audio/Sound/winmusic";
    // 音效路径：游戏失败音效
    public const string loseMusic = "Audio/Sound/losemusic";
    // 音效路径：铲子选中音效
    public const string shovelClick = "Audio/Sound/shovel";
    // 音效路径：种植植物音效（1）
    public const string plant = "Audio/Sound/plant";
    // 音效路径：种植植物音效（2）
    public const string plant2 = "Audio/Sound/plant2";
    // 音效路径：点击取消音效（1）
    public const string tap = "Audio/Sound/tap";
    // 音效路径：点击取消音效（2）
    public const string tap2 = "Audio/Sound/tap2";
    // 音效路径：最终波提示音效
    public const string finalwave = "Audio/Sound/finalwave";
    // 音效路径：发射子弹音效（1）
    public const string shoot = "Audio/Sound/throw";
    // 音效路径：发射子弹音效（2）
    public const string shoot2 = "Audio/Sound/throw2";
    // 音效路径：子弹命中僵尸音效（1）
    public const string splat = "Audio/Sound/splat";
    // 音效路径：子弹命中僵尸音效（2）
    public const string splat2 = "Audio/Sound/splat2";
    // 音效路径：子弹命中僵尸音效（3）
    public const string splat3 = "Audio/Sound/splat3";
    // 音效路径：游戏准备阶段音效
    public const string prepare = "Audio/Sound/prepare";
    // 音效路径：巨浪提示音效
    public const string hugewave = "Audio/Sound/hugewave";
    // 音效路径：僵尸开始生成音效
    public const string zombieStartSpawn = "Audio/Sound/awooga";
    // 音效路径：波次警报音效
    public const string waveSiren = "Audio/Sound/siren";
    // 音效路径：僵尸吃植物音效（1）
    public const string eatPlant = "Audio/Sound/chomp";
    // 音效路径：僵尸吃植物音效（2）
    public const string eatPlant2 = "Audio/Sound/chomp2";
    // 音效路径：僵尸吃植物音效（3）
    public const string eatPlant3 = "Audio/Sound/chompsoft";
    // 音效路径：僵尸吃完植物音效
    public const string eatFinish = "Audio/Sound/gulp";
    // 音效路径：小车移动音效
    public const string carMove = "Audio/Sound/lawnmower";
    // 音效路径：选择植物卡牌音效
    public const string selectPlant = "Audio/Sound/seedlift";
    // 音效路径：游戏暂停音效
    public const string pause = "Audio/Sound/pause";
    // 音效路径：按钮点击音效
    public const string buttonClick = "Audio/Sound/buttonclick";
}