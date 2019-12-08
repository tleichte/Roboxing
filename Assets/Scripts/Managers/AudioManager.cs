using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    public OneShotSoundSO[] Sounds;
    private Dictionary<string, OneShotSoundSO> soundsDict;


    //[Header("Curtain")]
    //[EventRef] public string OpenEvent;
    //[EventRef] public string CloseEvent;

    //[Header("Punches")]
    //[EventRef] public string HookImpact;
    //[EventRef] public string HookThrow;
    //[EventRef] public string JabImpact;
    //[EventRef] public string JabThrow;

    //[Header("Down")]
    //[EventRef] public string Downed;

    //[Header("Ready")]
    //[EventRef] public string RoundStart;
    //[EventRef] public string RoundEnd;
    //[EventRef] public string RoundTKO;
    //[EventRef] public string RoundKO;

    //private Dictionary<string, Sound> soundsDict = new Dictionary<string, Sound>();

    void Awake() {

        if (Inst != null) {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);

        soundsDict = new Dictionary<string, OneShotSoundSO>();
        foreach (var sound in Sounds) {
            soundsDict.Add(sound.Name, sound);
        }
    }


    public void PlayOneShot(string name) {
        if (soundsDict.ContainsKey(name)) {
            RuntimeManager.PlayOneShot(soundsDict[name].Event);
        }
        else {
            Debug.LogWarning($"Couldn't Play Sound: {name}");
        }
    }


    public void PlayCurtain(bool open) {
        PlayOneShot($"Curtain_{ (open ? "Open" : "Close") }");
    }

    public void PlayPunchThrow(HitType type) {
        switch (type) {
            case HitType.Hook:
                PlayOneShot("Hook_Throw");
                break;
            case HitType.Jab:
                PlayOneShot("Jab_Throw");
                break;
        }
    }

    public void PlayPunchImpact(HitType type) {
        switch (type) {
            case HitType.Hook:
                PlayOneShot("Hook_Impact");
                break;
            case HitType.Jab:
                PlayOneShot("Jab_Impact");
                break;
        }
    }

    public void PlayPunchBlocked(HitType type) {
        switch (type) {
            case HitType.Hook:
                PlayOneShot("Hook_Blocked");
                break;
            case HitType.Jab:
                PlayOneShot("Jab_Blocked");
                break;
        }
    }

    //public void PlayDowned() {
    //    RuntimeManager.PlayOneShot(Downed);
    //}




    //public void Play

}
