using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    [FormerlySerializedAs("Sounds")]
    public SoundEventSO[] OneShots;
    private Dictionary<string, SoundEventSO> oneShotsDict;


    public SoundEventSO[] Loops;
    private Dictionary<string, StudioEventEmitter> loopsDict;

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

        oneShotsDict = new Dictionary<string, SoundEventSO>();
        foreach (var oneShot in OneShots) {
            oneShotsDict.Add(oneShot.Name, oneShot);
        }

        loopsDict = new Dictionary<string, StudioEventEmitter>();
        foreach (var loop in Loops) {
            var emitter = gameObject.AddComponent<StudioEventEmitter>();
            emitter.Event = loop.Event;
            loopsDict.Add(loop.Name, emitter);
        }
    }


    public void PlayOneShot(string name) {
        if (oneShotsDict.ContainsKey(name)) {
            RuntimeManager.PlayOneShot(oneShotsDict[name].Event);
        }
        else {
            Debug.LogWarning($"Couldn't find One Shot: {name}");
        }
    }

    public void PlayLoop(string name) => GetLoop(name)?.Play();
    public void StopLoop(string name) => GetLoop(name)?.Stop();

    private StudioEventEmitter GetLoop(string name) {
        if (loopsDict.ContainsKey(name)) {
            return loopsDict[name];
        }
        Debug.LogWarning($"Couldn't find Loop: {name}");
        return null;
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
