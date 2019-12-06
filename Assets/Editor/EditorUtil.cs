using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorUtil
{

    [MenuItem("Edit/Reset PlayerPrefs")]
    public static void DeletePlayerPrefs() {
        PlayerPrefs.DeleteAll();
    }
    
}
