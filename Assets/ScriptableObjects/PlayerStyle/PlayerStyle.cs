using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Style", menuName = "Styles/PlayerStyle", order = 0)]
public class PlayerStyle : ScriptableObject
{
    public Color UIColor;

    [Header("Head")]
    public Sprite HeadIdle;
    public Sprite HeadHitUp;
    public Sprite HeadHitDown;
    public Sprite HeadDown;
    public Sprite HeadStunned;

    [Header("Body")]
    public Sprite BodyIdle;
    public Sprite BodyHit;

    [Header("Arm")]
    public Sprite FPArmIdle;
    public Sprite FPArmBlock;
    public Sprite FPArmPunch;
    public Sprite TPArmIdle;
    public Sprite TPArmBlock;
    public Sprite TPArmPunch;
}
