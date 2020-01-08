using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum MainMenuOptionType { Play, Sound, Quit, HowToPlay }

public class MainMenuOption : MonoBehaviour
{

    public MainMenuOptionType Type;

    //public GameObject LeftSelect;
    //public GameObject RightSelect;

    //public Animator Animator;

    public SwooshAnimator Animator;

    public TMP_Text MenuText;

    public Color inColor;
    public Color outColor;


    public void Highlight() {
        Animator.In();
        MenuText.color = inColor;
    }
    public void DeHighlight() {
        Animator.Out();
        MenuText.color = outColor;
    }
}
