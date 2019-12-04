using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    [EventRef] public string OpenEvent;
    [EventRef] public string CloseEvent;

    [EventRef] public string PunchHook;
    [EventRef] public string PunchJab;

    //public Sound[] sounds;

    //private List<string> jabs = new List<string>();
    //private List<string> hooks = new List<string>();

    //private List<string> crowdAmbients = new List<string>();
    //private List<string> crowdOofs = new List<string>();
    //private List<string> crowdCheers = new List<string>();


    //private Dictionary<string, Sound> soundsDict = new Dictionary<string, Sound>();

    void Awake() {

        if (Inst != null) {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);

        //foreach(var sound in sounds) {
        //    sound.Source = gameObject.AddComponent<AudioSource>();
        //    soundsDict.Add(sound.Name, sound);

        //    void AddToList(List<string> list) => list.Add(sound.Name);

        //    switch (sound.Tag) {
        //        case SoundTag.Hook:         AddToList(hooks); break;
        //        case SoundTag.Jab:          AddToList(jabs); break;
        //        case SoundTag.CrowdAmbient: AddToList(crowdAmbients); break;
        //        case SoundTag.CrowdOof:     AddToList(crowdOofs); break;
        //        case SoundTag.CrowdCheer:   AddToList(crowdCheers); break;
        //        default: break;
        //    }
        //}
    }

    //public bool IsPlaying(string name) {
    //    if (soundsDict.ContainsKey(name)) {
    //        return soundsDict[name].Source.isPlaying;
    //    }
    //    return false;
    //}
    
    //public void PlaySound(string name) {
    //    if (soundsDict.ContainsKey(name)) {
    //        soundsDict[name].Source.Play();
    //        Debug.Log($"Playing Sound: {name}");
    //    }
    //}

    //public void StopSound(string name) {
    //    if (soundsDict.ContainsKey(name)) {
    //        soundsDict[name].Source.Stop();
    //        Debug.Log($"Stopping Sound: {name}");
    //    }
    //}

    //public void OnJab() => PlaySound(GetRandomString(jabs));
    

    //public void OnHook() => PlaySound(GetRandomString(hooks));

    //private string GetRandomString(List<string> strArr) => strArr[Random.Range(0, strArr.Count - 1)];


    public void PlayCurtain(bool open) {
        RuntimeManager.PlayOneShot(open ? OpenEvent : CloseEvent);
    }

    public void PlayPunch(HitType type) {
        switch(type) {
            case HitType.Hook:
                RuntimeManager.PlayOneShot(PunchHook);
                break;
            case HitType.Jab:
                RuntimeManager.PlayOneShot(PunchJab);
                break;
        }
       // RuntimeManager.PlayOneShot();
    }
}
