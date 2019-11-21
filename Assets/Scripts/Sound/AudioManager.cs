using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound {

    public string Name;

    public AudioClip Clip;
    [Range(0, 1)]
    public float Volume;
    [Range(0, 1)]
    public float Pitch;

    public bool Loop;

    private AudioSource _source;
    public AudioSource Source {
        get => _source;
        set {
            _source = value;
            _source.volume = Volume;
            _source.pitch = Pitch;
            _source.loop = Loop;
        }
    }
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Inst;

    public Sound[] sounds;

    public string[] JabSounds;
    public string[] HookSounds;

    public string[] CrowdAmbientSounds;
    public string[] CrowdOofSounds;
    public string[] CrowdCheerSounds;


    private Dictionary<string, Sound> soundsDict = new Dictionary<string, Sound>();

    void Awake() {
        if (Inst != null) {
            Destroy(gameObject);
            return;
        }
        Inst = this;
        DontDestroyOnLoad(gameObject);
        AddFromArray(sounds);
    }

    private void AddFromArray(Sound[] arr) {
        foreach (Sound s in arr) {
            s.Source = gameObject.AddComponent<AudioSource>();
            soundsDict.Add(s.Name, s);
        }
    }


    public void PlaySound(string name) {
        if (soundsDict.ContainsKey(name)) {
            soundsDict[name].Source.Play();
        }
    }

    public void StopSound(string name) {
        if (soundsDict.ContainsKey(name)) {
            soundsDict[name].Source.Stop();
        }
    }

    public void OnJab() {
        PlaySound(GetRandomString(JabSounds));
    }

    public void OnHook() {
        PlaySound(GetRandomString(HookSounds));
    }

    private string GetRandomString(string[] strArr) {
        return strArr[Random.Range(0, strArr.Length - 1)];
    }
}
