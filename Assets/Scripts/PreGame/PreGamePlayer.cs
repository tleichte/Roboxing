using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PreGamePlayerState { Letters, Style, NotReady, Ready }

public class PreGamePlayer : MonoBehaviour
{

    public bool Player1;
    
    public PreGameLetter[] LetterTexts;

    public PreGamePlayerStyleView StyleView;

    public GameObject StyleTakenIndicator;

    public PreGameReady Ready;

    public bool IsReady => state == PreGamePlayerState.Ready;

    public char[] Letters { get; private set; } = new char[] { 'A', 'A', 'A' };

    public int CurrentStyle { get; private set; }
    public int CurrentLetter { get; private set; }
    private HGDCabPlayer player;

    private PreGamePlayerState state;

    private PreGame preGame;

    public void Initialize(PreGame preGame) {
        this.preGame = preGame;
        CurrentLetter = 0;

        CurrentStyle = (Player1) ? 0 : 1;

        player = (Player1) ? HGDCabPlayer.P1 : HGDCabPlayer.P2;

        foreach (var letter in LetterTexts)
            letter.Initialize(this);

        Debug.Log(StyleView);

        StyleView.Initialize(this);
    }


    void Update() {

        switch (state) {
            case PreGamePlayerState.Letters:
                
                void ChangeLetter(int delta) {

                    if (Letters[CurrentLetter] == ' ') {
                        Letters[CurrentLetter] = (delta > 0) ? 'A' : 'Z';
                    }
                    else {
                        Letters[CurrentLetter] += (char) delta;
                        if (Letters[CurrentLetter] < 'A' || Letters[CurrentLetter] > 'Z') {
                            Letters[CurrentLetter] = ' ';
                        }
                    }
                    LetterTexts[CurrentLetter].OnLetterChange();
                }

                if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyDown)) {
                    ChangeLetter(-1);
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyUp)) {
                    ChangeLetter(1); 
                }

                void CurrentLetterChanged() {
                    CurrentLetter = Mathf.Max(0, CurrentLetter);
                    foreach (var letter in LetterTexts) letter.OnCurrentLetterChanged();
                    if (CurrentLetter >= Letters.Length) {
                        state = PreGamePlayerState.Style;
                        StyleView.OnStyleEnter();
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

                if (Input.GetKeyDown(HGDCabKeys.Of(player).Top1) || Input.GetKeyDown(HGDCabKeys.Of(player).JoyDown)) {
                    state = PreGamePlayerState.NotReady;
                    StyleView.OnStyleExit();
                    Ready.OnNotReady();
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2) || Input.GetKeyDown(HGDCabKeys.Of(player).JoyUp)) {
                    state = PreGamePlayerState.Letters;
                    CurrentLetter = LetterTexts.Length - 1;
                    StyleView.OnStyleExit();
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
            case PreGamePlayerState.NotReady:
                if (Input.GetKeyDown(HGDCabKeys.Of(player).Top1) && !preGame.IsStyleTaken(CurrentStyle)) {
                    state = PreGamePlayerState.Ready;
                    Ready.OnReady();
                    preGame.OnReady(Player1);
                }
                else if (Input.GetKeyDown(HGDCabKeys.Of(player).JoyUp) || Input.GetKeyDown(HGDCabKeys.Of(player).Top2)) {
                    state = PreGamePlayerState.Style;
                    StyleView.OnStyleEnter();
                    Ready.OnEdit();
                }
                break;
            case PreGamePlayerState.Ready:
                if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2) && !preGame.Starting) {
                    state = PreGamePlayerState.NotReady;
                    Ready.OnNotReady();
                    preGame.CancelReady(Player1);
                }
                break;
        }

        StyleTakenIndicator.SetActive(!IsReady && preGame.IsStyleTaken(CurrentStyle));
    }

    public char GetLetter(int i) => Letters[i];
}
