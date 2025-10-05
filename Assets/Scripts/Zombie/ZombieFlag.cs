using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ´øÆìÖÄµÄ½©Ê¬£¨¼Ì³Ğ×ÔÆÕÍ¨½©Ê¬£©
public class ZombieFlag : ZombieCommon {
    // ÆìÖÄÏà¹ØäÖÈ¾Æ÷
    public SpriteRenderer goodFlag; // ÍêºÃÆìÖÄ
    public SpriteRenderer badFlag; // ÆÆËğÆìÖÄ

    public Transform flagEmissionTransfrom;

    // ÆìÖÄ×´Ì¬±ê¼Ç
    bool isFlagBad = false;
    bool isFlagDead = false;

    private new void Awake() {
        base.Awake();

        badFlag.enabled = false;
    }

    // ÖØĞ´ÊÜÉËÂß¼­£¨Ôö¼ÓÆìÖÄ×´Ì¬´¦Àí£©
    public override void TakeCommonDamage(float damage) {
        base.TakeCommonDamage(damage);

        // ÆìÖÄ×´Ì¬±ä»¯
        if (HP <= 100) {
            FlagBad(); // ÆìÖÄÆÆËğ
        }
        if (HP <= 20) {
            FlagDead(); // ÆìÖÄÏûÊ§
        }
    }

    // ÆìÖÄÆÆËğ
    private void FlagBad() {
        if (isFlagBad) return;
        isFlagBad = true;
        goodFlag.enabled = false;
        badFlag.enabled = true;
    }

    // ÆìÖÄÏûÊ§
    private void FlagDead() {
        if (isFlagDead) return;
        isFlagDead = true;
        badFlag.enabled = false;
        // ²¥·ÅÆìÖÄÌØĞ§
        ObjectPoolManager.Instance.PlayFlagEmissionIEnumrator(flagEmissionTransfrom, spriteList[0].sortingOrder + 100, isCroze);

        // ÇĞ»»äÖÈ¾Æ÷ÏÔÊ¾×´Ì¬
        hideSprite.ForEach(r => r.enabled = true);
        showSprite.ForEach(r => r.enabled = false);
    }
}