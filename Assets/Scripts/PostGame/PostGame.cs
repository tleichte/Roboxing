using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{
    
    public TMP_Text ResultText;
    public TMP_Text ReasonText;

    public PostGameStats winnerStats;
    public PostGameStats loserStats;
    public PostGameStats tieStatsP1;
    public PostGameStats tieStatsP2;


    public bool Returning { get; private set; }

    private int numReady;

    // Start is called before the first frame update
    void Start()
    {
        
        string preReason = "";
        if (GameData.Result == GameOverResult.Tie) {
            ResultText.text = "Tie!";
            ResultText.color = Color.white;
            preReason = "Double ";

            tieStatsP1.Initialize(true);
            tieStatsP2.Initialize(false);
            winnerStats.gameObject.SetActive(false);
            loserStats.gameObject.SetActive(false);
        }
        else {

            tieStatsP1.gameObject.SetActive(false);
            tieStatsP2.gameObject.SetActive(false);
            winnerStats.Initialize(GameData.Result == GameOverResult.P1Win);
            loserStats.Initialize(GameData.Result != GameOverResult.P1Win);


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

        CurtainTransition.Inst.Open();
        AudioManager.Inst.PlayLoop("PostGame");
    }


    public void PlayerReady() {
        numReady++;
        if (numReady > 1) {
            StartCoroutine(GoToMenuAfterDelay(0.5f));
        }
    }

    public void PlayerUnready() {
        numReady--;
    }

    IEnumerator GoToMenuAfterDelay(float delay) {
        Returning = true;

        yield return new WaitForSecondsRealtime(delay);

        AudioManager.Inst.StopLoop("PostGame");
        CurtainTransition.Inst.Close(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }
}
