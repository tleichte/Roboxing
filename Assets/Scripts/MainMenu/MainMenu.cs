﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public MainMenuOption[] Options;

    private int _option;
    private int CurrOption {
        get { return _option; }
        set {
            foreach (var opt in Options) opt.DeHighlight();
            _option = value;
            if (_option >= Options.Length) _option = 0;
            else if (_option < 0) _option = Options.Length - 1;
            Options[_option].Highlight();
        }
    }
    private MainMenuOption CurrOptionValue => Options[_option]; 

    private int selectedOption;

    private bool starting;

    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.Inst.PlaySound("MainMenuSong");
        CurtainTransition.Inst.Open(() => {
            //AudioManager.Inst.PlaySound("MainMenuSong");
        });
        CurrOption = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CurtainTransition.Inst.InProgress) {
            if (Input.GetKeyDown(HGDCabKeys.P1.Top1)) {
                Confirm();
            }
            if (Input.GetKeyDown(HGDCabKeys.P1.JoyUp)) {
                CurrOption--;
            }
            if (Input.GetKeyDown(HGDCabKeys.P1.JoyDown)) {
                CurrOption++;
            }
        }
        //else {
        //    Debug.Log("Curtain in progress!");
        //}
    }

    private void Confirm() {
        
            switch (CurrOptionValue.Type) {
                case MainMenuOptionType.Play:
                    CurtainTransition.Inst.Close(() => {
                        SceneManager.LoadScene("PreGame");
                    });
                    break;
                case MainMenuOptionType.Credits:

                    CurtainTransition.Inst.Close(() => {
                        CurtainTransition.Inst.Open();
                    });

                    break;
                case MainMenuOptionType.Quit:
                    CurtainTransition.Inst.Close(() => {
                        Application.Quit();
                    });
                    break;
            }
    }
}
