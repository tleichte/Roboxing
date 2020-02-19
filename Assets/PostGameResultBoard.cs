using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameResultBoard : MonoBehaviour
{
    public TMP_Text ResultText;
    public TMP_Text ReasonText;

    public void Initialize()
    {
        string preReason = "";
        if (GameData.Result == GameOverResult.Tie) {
            ResultText.text = "Tie!";
            ResultText.color = Color.white;
            preReason = "Double ";
        }
        else {
            string winner = "";
            switch (GameData.Result) {
                case GameOverResult.P1Win:
                    ResultText.color = AssetManager.Inst.PlayerStyles[GameData.P1Data.Style].UIColor;
                    winner = GameData.P1Data.Name;
                    break;
                case GameOverResult.P2Win:
                    ResultText.color = AssetManager.Inst.PlayerStyles[GameData.P2Data.Style].UIColor;
                    winner = GameData.P2Data.Name;
                    break;
            }
            ResultText.text = $"{winner} wins!";
        }
        string reason = "";
        switch (GameData.Reason) {
            case GameOverReason.Decision: reason = "Decision"; break;
            case GameOverReason.KO: reason = $"{preReason}KO"; break;
            case GameOverReason.TKO: reason = $"{preReason}TKO"; break;
        }
        ReasonText.text = $"By {reason}";
    }
}
