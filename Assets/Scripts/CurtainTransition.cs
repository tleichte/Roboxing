using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurtainTransition : MonoBehaviour
{

    public bool StartsOpen;

    public bool InProgress { get; private set; }

    public Animator Animator;

    private Action OnDone;

    private void Start() {
        Animator.Play((StartsOpen) ? "Open" : "Closed");        
    }

    public void Open(Action onOpen) {
        OnDone = onOpen;
        InProgress = true;
        Animator.SetBool("Open", true);
    }

    public void Close(Action onClose) {
        OnDone = onClose;
        InProgress = true;
        Animator.SetBool("Open", false);
    }

    // Called by animator
    public void A_AnimDone() {
        OnDone?.Invoke();
        InProgress = false;
    }
}
