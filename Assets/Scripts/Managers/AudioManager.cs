using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;
using System;
using System.Runtime.InteropServices;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    public bool SoundOn {
        get { return PrefItoB(PlayerPrefs.GetInt("Sound", 1)); }
        private set { PlayerPrefs.SetInt("Sound", PrefBtoI(value)); }
    }

    [FormerlySerializedAs("Sounds")]
    public SoundEventSO[] OneShots;
    private Dictionary<string, SoundEventSO> oneShotsDict;

    public SoundEventSO[] Loops;
    private Dictionary<string, StudioEventEmitter> loopsDict;

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

        SetMute();
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

    public void SetLoopParameter(string loopName, string paramName, float value) => GetLoop(loopName)?.SetParameter(paramName, value);

    public void SetLoopCallback(
        string loopName,
        FMOD.Studio.EVENT_CALLBACK callback,
        FMOD.Studio.EVENT_CALLBACK_TYPE callbackmask = FMOD.Studio.EVENT_CALLBACK_TYPE.ALL
    ) 
        => GetLoop(loopName)?.EventInstance.setCallback(callback, callbackmask);

    public bool IsLoopPlaying(string name) {
        bool? playing = GetLoop(name)?.IsPlaying();
        return playing.HasValue && playing.Value;
    }

    private StudioEventEmitter GetLoop(string name) {
        if (loopsDict.ContainsKey(name)) {
            return loopsDict[name];
        }
        Debug.LogWarning($"Couldn't find Loop: {name}");
        return null;
    }

    public void MMSToMenu() {
        SetLoopParameter("MMS", "Menu_or_Gym", 0);
        if (!IsLoopPlaying("MMS")) {
            PlayLoop("MMS");
        }
    }
    public void MMSToPreGame() {
        SetLoopParameter("MMS", "Menu_or_Gym", 1);
    }
    public void MMSStop() {
        StopLoop("MMS");
    }


    private static bool _fromKO;
    private static FMOD.RESULT InGameEndingCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr) {
        
        Inst.PlayOneShot(_fromKO ? "InGameFinishKO" : "InGameFinishDecision");

        Inst.StopLoop("InGame");
        
        return FMOD.RESULT.OK;
    }

    public void StartInGame() {
        Inst.SetLoopCallback("InGame", null);
        Inst.PlayLoop("InGame");
    }
    public void StopInGame(bool fromKO) {
        _fromKO = fromKO;
        Inst.GetLoop("InGame").EventInstance.setCallback(InGameEndingCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }


    private static Action _startingAction;
    private const string StartingMarkerName = "Curtain_Close";
    private static FMOD.RESULT StartingCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, FMOD.Studio.EventInstance instance, IntPtr parameterPtr) {

        Debug.Log("StartingCallback fired!");

        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
        
        if (parameter.name == StartingMarkerName) _startingAction?.Invoke();
        
        return FMOD.RESULT.OK;
    }

    public void PlayStartingSound(Action startingAction) {
        _startingAction = startingAction;
        var bootEmitter = GetLoop("BootMusic");
        bootEmitter.Play();
        bootEmitter.EventInstance.setCallback(StartingCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
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

    public void ToggleMute() {
        SoundOn = !SoundOn;
        SetMute();
    }

    private void SetMute() {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.audioMasterMute = !SoundOn;
#else
        RuntimeManager.MuteAllEvents(!SoundOn);
#endif
    }

    private bool PrefItoB(int i) => i != 0;
    private int PrefBtoI(bool b) => b ? 1 : 0;
}
