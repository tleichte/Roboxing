using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameFPAssetSetter : MonoBehaviour
{
    public bool Player1;

    [Header("Left")]
    public SpriteRenderer IdleL;
    public SpriteRenderer PunchL;
    public SpriteRenderer BlockL;

    [Header("Right")]
    public SpriteRenderer IdleR;
    public SpriteRenderer PunchR;
    public SpriteRenderer BlockR;

    void Start() {
        int i = ((Player1) ? GameData.P1Data : GameData.P2Data).Style;
        var style = AssetManager.Inst.PlayerStyles[i];

        IdleL.sprite = style.FPArmIdle;
        PunchL.sprite = style.FPArmPunch;
        BlockL.sprite = style.FPArmBlock;

        IdleR.sprite = style.FPArmIdle;
        PunchR.sprite = style.FPArmPunch;
        BlockR.sprite = style.FPArmBlock;
    }

}
