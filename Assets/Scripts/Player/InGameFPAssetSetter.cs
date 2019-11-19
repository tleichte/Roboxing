using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameFPAssetSetter : MonoBehaviour
{
    public bool Player1;

    [Header("Left")]
    public SpriteRenderer IdleL;
    public SpriteRenderer PunchL;

    [Header("Right")]
    public SpriteRenderer IdleR;
    public SpriteRenderer PunchR;
    

    void Start() {
        int i = ((Player1) ? GameData.P1Data : GameData.P2Data).Style;
        var style = AssetManager.Inst.PlayerStyles[i];

        IdleL.sprite = style.FPArmIdle;
        PunchL.sprite = style.FPArmPunch;

        IdleR.sprite = style.FPArmIdle;
        PunchR.sprite = style.FPArmPunch;
    }

}
