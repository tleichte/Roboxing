using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameReady : MonoBehaviour
{

    public CanvasGroup CanvasGroup;
    public GameObject Check;


    // Start is called before the first frame update
    void Start()
    {
        OnEdit();   
    }

    public void OnEdit() {
        CanvasGroup.alpha = 0.5f;
        Check.SetActive(false);
    }

    public void OnNotReady() {
        CanvasGroup.alpha = 1;
        Check.SetActive(false);
    }

    public void OnReady() {
        Check.SetActive(true);
    }
}
