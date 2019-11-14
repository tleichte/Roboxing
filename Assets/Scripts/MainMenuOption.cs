using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainMenuOptionType { Play, Quit }

public class MainMenuOption : MonoBehaviour
{

    public MainMenuOptionType Type;

    public GameObject LeftSelect;
    public GameObject RightSelect;

    public void Highlight() {
        LeftSelect.SetActive(true);
        RightSelect.SetActive(true);
    }
    public void DeHighlight() {
        LeftSelect.SetActive(false);
        RightSelect.SetActive(false);
    }
}
