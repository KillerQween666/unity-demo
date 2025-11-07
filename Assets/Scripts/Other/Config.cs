using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 配置类，存储项目中所有音频资源的路径常量（集中管理，方便统一修改和调用）
public class Config : MonoBehaviour {

    // 背景音乐路径 (1)
    public const string bgm1 = "Audio/Music/bgm1";
    // 背景音乐路径 (1)
    public const string bgm2 = "Audio/Music/bgm2";
    // 背景音乐路径 (1)
    public const string bgm3 = "Audio/Music/bgm3";
    // 背景音乐路径 (3)
    public const string prepareBgm = "Audio/Music/prepareBgm";
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
    // 音效路径：大嘴花吃僵尸音效
    public const string eatZombie = "Audio/Sound/bigchomp";
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
    // 音效路径：僵尸叫喊音效（1）
    public const string groan = "Audio/Sound/groan";
    // 音效路径：僵尸叫喊音效（2）
    public const string groan2 = "Audio/Sound/groan2";
    // 音效路径：僵尸叫喊音效（3）
    public const string groan3 = "Audio/Sound/groan3";
    // 音效路径：僵尸叫喊音效（4）
    public const string groan4 = "Audio/Sound/groan4";
    // 音效路径：僵尸叫喊音效（5）
    public const string groan5 = "Audio/Sound/groan5";
    // 音效路径：僵尸叫喊音效（6）
    public const string groan6 = "Audio/Sound/groan6";
    // 音效路径：土豆雷出土音效
    public const string potatoRise = "Audio/Sound/dirt_rise";
    // 音效路径：土豆雷爆炸音效
    public const string potatoBoom = "Audio/Sound/Potato_mine";
    // 音效路径：樱桃炸弹即将爆炸音效
    public const string reverseBoom = "Audio/Sound/Reverse_explosion";
    // 音效路径：樱桃炸弹爆炸音效
    public const string cherryBombBoom = "Audio/Sound/cherrybomb";
    // 音效路径：寒冰射手冰冻音效
    public const string Frozen = "Audio/Sound/frozen";
    // 音效路径：铁通受击音效（1）
    public const string bucket = "Audio/Sound/shieldhit";
    // 音效路径：铁桶受击音效（2）
    public const string bucket2 = "Audio/Sound/shieldhit2";
    // 音效路径： 撑杆跳僵尸跳跃音效
    public const string jump = "Audio/Sound/polevault";
    public const string doomShroomBoom = "Audio/Sound/DoomShroom";
    public const string puff = "Audio/Sound/puff";
    public const string fume = "Audio/Sound/fume";
    public const string paper = "Audio/Sound/paper";
    public const string paperCry = "Audio/Sound/newspaper_rarrgh";
    public const string paperCry2 = "Audio/Sound/newspaper_rarrgh2";
    public const string graveButton = "Audio/Sound/gravebutton";
    public const string hypnoZombie = "Audio/Sound/mindcontrolled";
    public const string dance = "Audio/Sound/Dancer";
    public const string spawnGrave = "Audio/Sound/gravestone_rumble";
    public const string busterGrave = "Audio/Sound/gravebusterchomp";
    public const string jalapeno = "Audio/Sound/jalapeno";

    public const string squashHmm = "Audio/Sound/squash_hmm";
    public const string squashHmm2 = "Audio/Sound/squash_hmm2";
    public const string squashThump = "Audio/Sound/gargantuar_thump";
    public const string plantWater = "Audio/Sound/plant_water";

    public const string dolphinAppear = "Audio/Sound/dolphin_appears";
    public const string dolphinJumping = "Audio/Sound/dolphin_before_jumping";
    public const string firepea = "Audio/Sound/firepea";
    public const string zamboni = "Audio/Sound/zamboni";
    public const string explosion = "Audio/Sound/explosion";
    public const string balloon_pop = "Audio/Sound/balloon_pop";
    public const string enterWater = "Audio/Sound/zombie_entering_water";
    public const string bonk = "Audio/Sound/bonk";
    public const string poolCar = "Audio/Sound/pool_cleaner";
}