using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Gravebuster : Plant {
    public float busterTime = 5f;
    private float busterTimer = 0;

    private bool isBuster = false;

    public Transform rockParticalPosition;

    private GameObject obj;
    private AudioSource source;

    private bool isGraveBad = false;

    ParticleSystem[] particals;

    private new void Start() {
        base.Start();

        obj = ObjectPoolManager.Instance.GetRockSmallPartical();
        particals = obj.GetComponentsInChildren<ParticleSystem>(true);
        source = GetComponent<AudioSource>();
    }

    protected override void EnableUpdate() { 
        if (isBuster == false) {
            busterTimer += Time.deltaTime;

            if (busterTimer >= busterTime * 0.5) {
                if (isGraveBad == false) {
                    isGraveBad = true;

                    selfCell.BadTombstone();
                }
            }

            if (busterTimer > busterTime) {
                isBuster = true;

                selfCell.BusterTombstone();
                Dead();
            }
        }    
    }

    public override void PlantFun() {
        StartCoroutine(PlayClip(Config.busterGrave));

        Vector3 position = transform.position;
        position.y += 0.5f;

        obj.transform.position = position;        // 将粒子位置设置为爆炸发生的位置
        obj.transform.DOMoveY(position.y - 0.7f, 4f).SetEase(Ease.Linear);

        foreach (var ps in particals) {
            ps.Play();                                  // 遍历并启动所有粒子系统，触发完整爆炸效果
        }
    }

    public override void Dead() {
        foreach (var ps in particals) {
            ps.Clear();                                 // 清除所有粒子系统的残留粒子
        }                          // 清除粒子系统中残留的粒子，避免下次复用时有残影
        ObjectPoolManager.Instance.ReleaseRockSmallPartical(obj);

        if (isBuster == false) selfCell.GoodTombstone();

        source.Stop();
        base.Dead();
    }

    IEnumerator PlayClip(string path) {

        source.volume = AudioManager.Instance.clipVolume;
        source.ignoreListenerPause = true;

        AudioClip audioClip = Resources.Load<AudioClip>(path);
        source.clip = audioClip;
        source.Play();

        yield return new WaitForSeconds(audioClip.length);

        source.Stop();
    }
}
