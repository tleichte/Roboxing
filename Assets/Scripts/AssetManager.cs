using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    [Header("Styles")]
    public PlayerStyle[] PlayerStyles;

    public static AssetManager Inst;
    
    void Awake()
    {
        if (Inst == null) {
            Inst = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            DestroyImmediate(gameObject);
        }
    }
}
