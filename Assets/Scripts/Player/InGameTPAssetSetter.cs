using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameTPAssetSetter : MonoBehaviour
{
    public bool Player1;

    [Header("Body")]
    public SpriteRenderer HeadHitUp;
    public SpriteRenderer HeadHitDown;
    public SpriteRenderer HeadIdle;
    public SpriteRenderer HeadStunned;
    public SpriteRenderer HeadDown;

    [Header("Body")]
    public SpriteRenderer BodyIdle;
    public SpriteRenderer BodyHit;

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

        HeadHitUp.sprite = style.HeadHitUp;
        HeadHitDown.sprite = style.HeadHitDown;
        HeadIdle.sprite = style.HeadIdle;
        HeadDown.sprite = style.HeadDown;
        HeadStunned.sprite = style.HeadStunned;

        BodyIdle.sprite = style.BodyIdle;
        BodyHit.sprite = style.BodyHit;

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
