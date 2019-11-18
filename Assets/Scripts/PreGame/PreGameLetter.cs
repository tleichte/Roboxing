using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreGameLetter : MonoBehaviour
{
    public int LetterNum;
    public GameObject TopArrow;
    public GameObject BottomArrow;
    public TMP_Text LetterText;

    private PreGamePlayer player;

    public void Initialize(PreGamePlayer player) {
        this.player = player;
        OnLetterChange();
        OnCurrentLetterChanged();
    }

    public void OnLetterChange() {
        LetterText.text = $"{player.GetLetter(LetterNum)}";
    }

    public void OnCurrentLetterChanged() {
        bool isCurrent = player.CurrentLetter == LetterNum;
        TopArrow.SetActive(isCurrent);
        BottomArrow.SetActive(isCurrent);
    }
}
