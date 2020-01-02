using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PreGamePlayerState { Name, Style, Ready }

public class PreGamePlayer : MonoBehaviour
{

    public SwooshAnimator SmallNameSwoosh;
    public SwooshAnimator ReadySwoosh;
    public SwooshAnimator LargeNameSwoosh;

    public TMP_Text SmallNameText;

    public bool Player1;
    
    public PreGameLetter[] LetterTexts;

    public PreGamePlayerStyleView StyleView;

    public GameObject StyleTakenIndicator;

    public bool IsReady => state == PreGamePlayerState.Ready;

    public char[] Letters { get; private set; } = new char[] { 'A', 'A', 'A' };

    public int CurrentStyle { get; private set; }
    public int CurrentLetter { get; private set; }
    private HGDCabPlayer player;

    private PreGamePlayerState state;

    private PreGame preGame;

    private float beforeHoldBuffer = 0.5f;
    private float afterHoldBuffer = 0.1f;
    //private float heldTime;

    private bool upHolding;
    private bool downHolding;

    public void Initialize(PreGame preGame) {
        this.preGame = preGame;
        CurrentLetter = 0;

        //heldTime = 0;

        CurrentStyle = (Player1) ? 0 : 1;

        player = (Player1) ? HGDCabPlayer.P1 : HGDCabPlayer.P2;

        foreach (var letter in LetterTexts)
            letter.Initialize(this);

        //Debug.Log(StyleView);

        StyleView.Initialize(this);


        LargeNameSwoosh.In();
        SmallNameSwoosh.Out();
        ReadySwoosh.Out();

    }


    void Update() {

        switch (state) {
            case PreGamePlayerState.Name:
                
                void ChangeLetter(int delta) {

                    AudioManager.Inst.PlayOneShot("PreGame_LetterChange");

                    if (Letters[CurrentLetter] == '_') {
                        Letters[CurrentLetter] = (delta > 0) ? 'A' : 'Z';
                    }
                    else {
                        Letters[CurrentLetter] += (char) delta;
                        if (Letters[CurrentLetter] < 'A' || Letters[CurrentLetter] > 'Z') {
                            Letters[CurrentLetter] = '_';
                        }
                    }
                    LetterTexts[CurrentLetter].OnLetterChange();
                }



                // Make letter changes go faster if held
                void CheckInput(InputType input, int delta, ref bool holding) {

                    if (!InputManager.GetKey(Player1, input)) {
                        holding = false;
                    }
                    else {
                        if (!holding) {
                            if (InputManager.GetKeyDelay(Player1, input, beforeHoldBuffer)) {
                                ChangeLetter(delta);
                                holding = true;
                            }
                        }
                        else {
                            if (InputManager.GetKeyDelay(Player1, input, afterHoldBuffer)) {
                                ChangeLetter(delta);
                            }
                        }
                    }
                }

                CheckInput(InputType.Down, -1, ref downHolding);
                CheckInput(InputType.Up, 1, ref upHolding);



                void CurrentLetterChanged() {
                    CurrentLetter = Mathf.Max(0, CurrentLetter);
                    foreach (var letter in LetterTexts) {
                        letter.OnCurrentLetterChanged();

                        AudioManager.Inst.PlayOneShot("PreGame_NextLetter");
                    }
                    if (CurrentLetter >= Letters.Length) {
                        state = PreGamePlayerState.Style;
                        StyleView.OnStyleEnter();

                        SmallNameText.text = new string(Letters);
                        
                        LargeNameSwoosh.Out(() => SmallNameSwoosh.In());
                        AudioManager.Inst.PlayOneShot("PreGame_NameAccept");
                    }
                }

                if (InputManager.GetKeyDown(Player1, InputType.Back) && CurrentLetter == 0) {
                    //Exit
                    preGame.GoBack();
                }
                else if (InputManager.GetKeyDown(Player1, InputType.Left) || InputManager.GetKeyDown(Player1, InputType.Back)) {
                    CurrentLetter--;
                    CurrentLetterChanged();
                }
                else if (InputManager.GetKeyDown(Player1, InputType.Right) || InputManager.GetKeyDown(Player1, InputType.Confirm)) {
                    CurrentLetter++;
                    CurrentLetterChanged();
                }
                break;
            case PreGamePlayerState.Style:

                void StyleChanged() {
                    AudioManager.Inst.PlayOneShot("PreGame_StyleChange");
                    int styleLength = AssetManager.Inst.PlayerStyles.Length;
                    if (CurrentStyle < 0) CurrentStyle = styleLength - 1;
                    if (CurrentStyle >= styleLength) CurrentStyle = 0;
                    StyleView.OnStyleChange();
                }

                if (InputManager.GetKeyDown(Player1, InputType.Confirm)) {

                    if (preGame.IsStyleTaken(CurrentStyle)) {
                        AudioManager.Inst.PlayOneShot("PreGame_StyleTaken");
                    }
                    else {
                        AudioManager.Inst.PlayOneShot("PreGame_Ready");
                        state = PreGamePlayerState.Ready;
                        preGame.OnReady(Player1);
                        ReadySwoosh.In();
                        StyleView.OnStyleExit(false);
                    }
                }
                else if (InputManager.GetKeyDown(Player1, InputType.Back)) {
                    state = PreGamePlayerState.Name;
                    AudioManager.Inst.PlayOneShot("PreGame_CancelStyle");
                    SmallNameSwoosh.Out(() => LargeNameSwoosh.In());
                    CurrentLetter = LetterTexts.Length - 1;
                    StyleView.OnStyleExit(true);
                    foreach(var letter in LetterTexts) letter.OnCurrentLetterChanged();
                }

                else if (InputManager.GetKeyDown(Player1, InputType.Left)) {
                    CurrentStyle--;
                    StyleChanged();
                }
                else if (InputManager.GetKeyDown(Player1, InputType.Right)) {
                    CurrentStyle++;
                    StyleChanged();
                }
                break;
            case PreGamePlayerState.Ready:
                if (InputManager.GetKeyDown(Player1, InputType.Back) && !preGame.Starting) {
                    state = PreGamePlayerState.Style;

                    AudioManager.Inst.PlayOneShot("PreGame_CancelReady");

                    ReadySwoosh.Out();

                    StyleView.OnStyleEnter();

                    preGame.CancelReady(Player1);
                }
                break;
        }

        StyleTakenIndicator.SetActive(!IsReady && preGame.IsStyleTaken(CurrentStyle));
    }

    public char GetLetter(int i) => Letters[i];
}
