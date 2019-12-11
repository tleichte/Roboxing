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
    private float heldTime;

    public void Initialize(PreGame preGame) {
        this.preGame = preGame;
        CurrentLetter = 0;

        heldTime = 0;

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
                void CheckKey(KeyCode key, int delta) {
                    if (Input.GetKeyDown(key)) {
                        heldTime = beforeHoldBuffer;
                        ChangeLetter(delta);
                    }
                    else if (Input.GetKey(key)) {
                        heldTime -= Time.deltaTime;
                        if (heldTime <= 0) {
                            heldTime = afterHoldBuffer;
                            ChangeLetter(delta);
                        }
                    }
                }

                KeyCode down = HGDCabKeys.Of(player).JoyDown;
                KeyCode up = HGDCabKeys.Of(player).JoyUp;

                CheckKey(down, -1);
                CheckKey(up, 1);



                void CurrentLetterChanged() {
                    CurrentLetter = Mathf.Max(0, CurrentLetter);
                    foreach (var letter in LetterTexts) {
                        letter.OnCurrentLetterChanged();
                    }
                    if (CurrentLetter >= Letters.Length) {
                        state = PreGamePlayerState.Style;
                        StyleView.OnStyleEnter();

                        SmallNameText.text = new string(Letters);
                        
                        LargeNameSwoosh.Out(() => SmallNameSwoosh.In());
                    }
                }

                if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2) && CurrentLetter == 0) {
                    //Exit
                    preGame.GoBack();
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyLeft) || Input.GetKeyDown(HGDCabKeys.Of(player).Top2)) {
                    CurrentLetter--;
                    CurrentLetterChanged();
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyRight) || Input.GetKeyDown(HGDCabKeys.Of(player).Top1)) {
                    CurrentLetter++;
                    CurrentLetterChanged();
                }
                break;
            case PreGamePlayerState.Style:

                void StyleChanged() {

                    int styleLength = AssetManager.Inst.PlayerStyles.Length;
                    if (CurrentStyle < 0) CurrentStyle = styleLength - 1;
                    if (CurrentStyle >= styleLength) CurrentStyle = 0;
                    StyleView.OnStyleChange();
                }

                if ((Input.GetKeyDown(HGDCabKeys.Of(player).Top1))
                    && !preGame.IsStyleTaken(CurrentStyle)) {
                    state = PreGamePlayerState.Ready;
                    preGame.OnReady(Player1);
                    ReadySwoosh.In();
                    StyleView.OnStyleExit(false);
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2)) {
                    state = PreGamePlayerState.Name;
                    SmallNameSwoosh.Out(() => LargeNameSwoosh.In());
                    CurrentLetter = LetterTexts.Length - 1;
                    StyleView.OnStyleExit(true);
                    foreach(var letter in LetterTexts) letter.OnCurrentLetterChanged();
                }

                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyLeft)) {
                    CurrentStyle--;
                    StyleChanged();
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyRight)) {
                    CurrentStyle++;
                    StyleChanged();
                }
                break;
            case PreGamePlayerState.Ready:
                if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2) && !preGame.Starting) {
                    state = PreGamePlayerState.Style;

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
