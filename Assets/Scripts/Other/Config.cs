using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����࣬�洢��Ŀ��������Ƶ��Դ��·�����������й�������ͳһ�޸ĺ͵��ã�
public class Config : MonoBehaviour {

    // ��������·��
    public const string bgm1 = "Audio/Music/bgm1";

    // ��Ч·���������ռ���Ч
    public const string sunClick = "Audio/Sound/points";
    // ��Ч·������Ϸʤ����Ч
    public const string winMusic = "Audio/Sound/winmusic";
    // ��Ч·������Ϸʧ����Ч
    public const string loseMusic = "Audio/Sound/losemusic";
    // ��Ч·��������ѡ����Ч
    public const string shovelClick = "Audio/Sound/shovel";
    // ��Ч·������ֲֲ����Ч��1��
    public const string plant = "Audio/Sound/plant";
    // ��Ч·������ֲֲ����Ч��2��
    public const string plant2 = "Audio/Sound/plant2";
    // ��Ч·�������ȡ����Ч��1��
    public const string tap = "Audio/Sound/tap";
    // ��Ч·�������ȡ����Ч��2��
    public const string tap2 = "Audio/Sound/tap2";
    // ��Ч·�������ղ���ʾ��Ч
    public const string finalwave = "Audio/Sound/finalwave";
    // ��Ч·���������ӵ���Ч��1��
    public const string shoot = "Audio/Sound/throw";
    // ��Ч·���������ӵ���Ч��2��
    public const string shoot2 = "Audio/Sound/throw2";
    // ��Ч·�����ӵ����н�ʬ��Ч��1��
    public const string splat = "Audio/Sound/splat";
    // ��Ч·�����ӵ����н�ʬ��Ч��2��
    public const string splat2 = "Audio/Sound/splat2";
    // ��Ч·�����ӵ����н�ʬ��Ч��3��
    public const string splat3 = "Audio/Sound/splat3";
    // ��Ч·������Ϸ׼���׶���Ч
    public const string prepare = "Audio/Sound/prepare";
    // ��Ч·����������ʾ��Ч
    public const string hugewave = "Audio/Sound/hugewave";
    // ��Ч·������ʬ��ʼ������Ч
    public const string zombieStartSpawn = "Audio/Sound/awooga";
    // ��Ч·�������ξ�����Ч
    public const string waveSiren = "Audio/Sound/siren";
    // ��Ч·������ʬ��ֲ����Ч��1��
    public const string eatPlant = "Audio/Sound/chomp";
    // ��Ч·������ʬ��ֲ����Ч��2��
    public const string eatPlant2 = "Audio/Sound/chomp2";
    // ��Ч·������ʬ��ֲ����Ч��3��
    public const string eatPlant3 = "Audio/Sound/chompsoft";
    // ��Ч·������ʬ����ֲ����Ч
    public const string eatFinish = "Audio/Sound/gulp";
    // ��Ч·����С���ƶ���Ч
    public const string carMove = "Audio/Sound/lawnmower";
    // ��Ч·����ѡ��ֲ�￨����Ч
    public const string selectPlant = "Audio/Sound/seedlift";
    // ��Ч·������Ϸ��ͣ��Ч
    public const string pause = "Audio/Sound/pause";
    // ��Ч·������ť�����Ч
    public const string buttonClick = "Audio/Sound/buttonclick";
}