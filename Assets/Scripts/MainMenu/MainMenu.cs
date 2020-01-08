using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public TMP_Text VersionText;

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

    
    void Start()
    {
        AudioManager.Inst.MMSToMenu(); // Before curtain opens
        CurtainTransition.Inst.Open(() => {
        });
        CurrOption = 0;

        var soundOpt = Options.First((x) => x.Type == MainMenuOptionType.Sound);
        if (soundOpt != null) {
            SetSoundText(soundOpt);
        }

        VersionText.text = $"Roboxing v{Application.version} ({ (InputManager.UsingController ? "Release Build" : "Cabinet Build") })";
    }

    
    void Update()
    {
        if (!CurtainTransition.Inst.InProgress) {
            if (InputManager.GetKeyDown(true, InputType.Confirm)) {
                AudioManager.Inst.PlayOneShot("Menu_Accept");
                Confirm();
            }
            if (InputManager.GetKeyDown(true, InputType.Up)) {
                AudioManager.Inst.PlayOneShot("Menu_Up");
                CurrOption--;
            }
            if (InputManager.GetKeyDown(true, InputType.Down)) {
                AudioManager.Inst.PlayOneShot("Menu_Down");
                CurrOption++;
            }
        }
    }

    private void Confirm() {
        
            switch (CurrOptionValue.Type) {
                case MainMenuOptionType.Play:
                    AudioManager.Inst.MMSToPreGame();
                    CurtainTransition.Inst.Close(() => {
                        SceneManager.LoadScene("PreGame");
                    });
                    break;
                case MainMenuOptionType.Sound:

                    AudioManager.Inst.ToggleMute();
                    SetSoundText(CurrOptionValue);

                    break;
                case MainMenuOptionType.HowToPlay:

                    CurtainTransition.Inst.Close(() => {
                        SceneManager.LoadScene("HowToPlay");
                    });
                    break;
                case MainMenuOptionType.Quit:
                    CurtainTransition.Inst.Close(() => {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
                    });
                    break;
            }
    }

    private void SetSoundText(MainMenuOption option) {
        option.MenuText.text = $"Sound: {(AudioManager.Inst.SoundOn ? "ON" : "OFF" )}";
    }
}