using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HeadType { Idle, HitUp, HitDown, Stunned, Down }

public class Head : MonoBehaviour
{

    public HeadType Type;

    public GameObject Idle;
    public GameObject HitUp;
    public GameObject HitDown;
    public GameObject Stunned;
    public GameObject Down;
    
    void LateUpdate()
    {
        Idle.SetActive(Type == HeadType.Idle);
        HitUp.SetActive(Type == HeadType.HitUp);
        HitDown.SetActive(Type == HeadType.HitDown);
        Stunned.SetActive(Type == HeadType.Stunned);
        Down.SetActive(Type == HeadType.Down);
    }
}
