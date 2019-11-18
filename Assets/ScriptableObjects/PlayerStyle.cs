using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Style", menuName = "Styles/PlayerStyle", order = 0)]
public class PlayerStyle : ScriptableObject
{

    [Header("Head")]
    public Sprite HeadIdle;
    public Sprite HeadHitHook;
    public Sprite HeadHitJab;
    public Sprite HeadHitDown;
    public Sprite HeadDown;
    public Sprite HeadStunned;

    [Header("Body")]
    public Sprite BodyIdle;
    public Sprite BodyHitUp;
    public Sprite BodyHitJab;
    public Sprite BodyHitHook;

    [Header("Arm")]
    public Sprite FPArmIdle;
    public Sprite FPArmBlock;
    public Sprite FPArmPunchUp;
    public Sprite FPArmPunchDown;
    public Sprite TPArmIdle;
    public Sprite TPArmBlock;
    public Sprite TPArmPunchUp;
    public Sprite TPArmPunchDown;



}
