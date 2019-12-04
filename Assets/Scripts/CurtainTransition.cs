using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainTransition : MonoBehaviour {
    public bool InProgress { get; private set; }

    public Animator Animator;

    private Action OnDone;

    public static CurtainTransition Inst { get; private set; }

    private void Awake() {
        if (Inst != null) {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Open(Action onOpen = null) {
        OnDone = onOpen;
        InProgress = true;
        Animator.SetBool("Open", true);

        AudioManager.Inst.PlayCurtain(true);
        // TODO Play open one-shot

    }

    public void Close(Action onClose = null) {
        OnDone = onClose;
        InProgress = true;
        Animator.SetBool("Open", false);

        AudioManager.Inst.PlayCurtain(false);
        // TODO Play close one-shot
    }

    // Called by animator
    public void A_AnimDone() {
        OnDone?.Invoke();
        InProgress = false;
    }


    public void A_PlaySound() {
        
    }
}
