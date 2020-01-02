﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

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
