using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum OneShotSound {
//    Curtain_Open, Curtain_Close,
//    Hook_Impact, Hook_Throw, Jab_Impact, Jab_Throw,
//    Downed,
//    Round_Start, Round_End, Round_TKO, Round_KO
//}

[CreateAssetMenu(fileName = "New One Shot", menuName = "Sound/One Shot", order = 0)]
public class OneShotSoundSO : ScriptableObject
{
    public string Name;
    [EventRef] public string Event;
}
