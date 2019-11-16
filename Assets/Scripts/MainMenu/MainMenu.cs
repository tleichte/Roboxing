using System.Collections;
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
            _option = Mathf.Clamp(value, 0, Options.Length);
            Options[_option].Highlight();
        }
    }
    private MainMenuOption CurrOptionValue => Options[_option]; 

    private int selectedOption;

    // Start is called before the first frame update
    void Start()
    {
        CurrOption = 0;
    }

    // Update is called once per frame
    void Update()
    {
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

    private void Confirm() {
        switch (CurrOptionValue.Type) {
            case MainMenuOptionType.Play:
                SceneManager.LoadScene("PreGame");
                break;
            case MainMenuOptionType.Quit:
                Application.Quit();
                break;
        }
    }
}
