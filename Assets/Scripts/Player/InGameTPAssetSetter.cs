using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTPAssetSetter : MonoBehaviour
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

    // Start is called before the first frame update
    void Start() {
        int i = ((Player1) ? GameData.P1Data : GameData.P2Data).Style;
        var style = AssetManager.Inst.PlayerStyles[i];

        IdleL.sprite = style.TPArmIdle;
        BlockL.sprite = style.TPArmBlock;
        PunchL.sprite = style.TPArmPunch;

        IdleR.sprite = style.TPArmIdle;
        BlockR.sprite = style.TPArmBlock;
        PunchR.sprite = style.TPArmPunch;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
