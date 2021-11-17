using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlay : MonoBehaviour
{

    public GameObject[] Pages;
    public TMP_Text PageText;
    public GameObject LeftPageArrow;
    public GameObject RightPageArrow;

    public GameObject ControllerLayout;
    public GameObject CabinetLayout;

    private int currPage;
    private int CurrPage {
        get { return currPage; }
        set {
            currPage = value;
            
            for (int i = 0; i < Pages.Length; i++)
                Pages[i].SetActive(i == currPage);

            PageText.text = $"{currPage+1}/{Pages.Length}";
            LeftPageArrow.SetActive(currPage > 0);
            RightPageArrow.SetActive(currPage < Pages.Length-1);
        }
    }


    void Start() {

        CurrPage = 0;
        CurtainTransition.Inst.Open();
        ControllerLayout.SetActive(true);
        CabinetLayout.SetActive(false);
        //ControllerLayout.SetActive(InputManager.UsingController);
        //CabinetLayout.SetActive(!InputManager.UsingController);
    }


    void Update() {

        if (
            !CurtainTransition.Inst.InProgress
            && (
                InputManager.GetKeyDown(true, InputType.Confirm)
                || InputManager.GetKeyDown(true, InputType.Back)
            )
        ) {
            AudioManager.Inst.PlayOneShot("HowToPlay_Back");
            CurtainTransition.Inst.Close(() => SceneManager.LoadScene("MainMenu"));
        }
        if (InputManager.GetKeyDown(true, InputType.Right) && CurrPage < Pages.Length-1) {
            AudioManager.Inst.PlayOneShot("HowToPlay_RuleChange");
            CurrPage++;
        }
        if (InputManager.GetKeyDown(true, InputType.Left) && CurrPage > 0) {
            AudioManager.Inst.PlayOneShot("HowToPlay_RuleChange");
            CurrPage--;
        }
    }
}
