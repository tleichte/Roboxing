using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeadType { Idle, HitUp, HitDown, Stunned, Down }

public class Head : MonoBehaviour
{

    public Player Player;

    public HeadType Type;

    public GameObject Idle;
    public GameObject HitUp;
    public GameObject HitDown;
    public GameObject Stunned;
    public GameObject Down;
    
    void LateUpdate()
    {

        Idle.SetActive(!Player.IsDown && Type == HeadType.Idle);
        HitUp.SetActive(!Player.IsDown && Type == HeadType.HitUp);
        HitDown.SetActive(!Player.IsDown && Type == HeadType.HitDown);
        Stunned.SetActive(!Player.IsDown && Type == HeadType.Stunned);
        Down.SetActive(Player.IsDown);
    }
}
