using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyType { Idle, Hit_Down, }

public class Body : MonoBehaviour
{

    public BodyType Type;

    public GameObject Idle;
    public GameObject HitDown;

    // Update is called once per frame
    void LateUpdate()
    {
        Idle.SetActive(Type == BodyType.Idle);
        HitDown.SetActive(Type == BodyType.Hit_Down);
    }
}
