using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmType { Idle, Punch, Block }

public class Arm : MonoBehaviour
{
    public ArmType Type;
    public GameObject Idle;
    public GameObject Punch;
    public GameObject Block;
    // Update is called once per frame
    void LateUpdate()
    {
        Idle.SetActive(Type == ArmType.Idle);
        Punch.SetActive(Type == ArmType.Punch);
        Block.SetActive(Type == ArmType.Block);
    }
}
