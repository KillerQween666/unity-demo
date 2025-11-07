using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LilyPad : Plant {

    private bool isWaterLand = false;

    protected override void EnableUpdate() {
        if (isWaterLand == false) {
            isWaterLand = true;

            selfCell.OpenWaterLand();
        } 
    }

    public override void Dead() {
        base.Dead();
        selfCell.CloseWaterLand();
    }
}
