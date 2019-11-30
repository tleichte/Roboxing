using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainMenuOptionType { Play, Credits, Quit }

public class MainMenuOption : MonoBehaviour
{

    public MainMenuOptionType Type;

    //public GameObject LeftSelect;
    //public GameObject RightSelect;

    public Animator Animator;

    public void Highlight() {
        //Animator.Play("Highlighted");
        Animator.SetBool("Highlighted", true);
        //LeftSelect.SetActive(true);
        //RightSelect.SetActive(true);
    }
    public void DeHighlight() {
        //Animator.Play("DeHighlighted");
        Animator.SetBool("Highlighted", false);
        //LeftSelect.SetActive(false);
        //RightSelect.SetActive(false);
    }
}
