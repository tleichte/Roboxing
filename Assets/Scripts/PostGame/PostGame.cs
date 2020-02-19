using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{
    public PostGameResultBoard winnerResultBoard;
    public PostGameResultBoard tieResultBoard;
    public PostGameStats winnerStats;
    public PostGameStats loserStats;
    public PostGameStats tieStatsP1;
    public PostGameStats tieStatsP2;

    public Image winnerImage;
    public Image loserImage;
    public Image p1TiedImage;
    public Image p2TiedImage;


    public GameObject tieGameObject;
    public GameObject winnerGameObject;

    public bool Returning { get; private set; }

    private int numReady;

    // Start is called before the first frame update
    void Start()
    {
        bool tied = GameData.Result == GameOverResult.Tie;

        tieGameObject.SetActive(tied);
        winnerGameObject.SetActive(!tied);

        if (GameData.Result == GameOverResult.Tie) {
            tieResultBoard.Initialize();
            tieStatsP1.Initialize(true);
            tieStatsP2.Initialize(false);
            p1TiedImage.sprite = AssetManager.Inst.PlayerStyles[GameData.P1Data.Style].PostGameP1Tie;
            p2TiedImage.sprite = AssetManager.Inst.PlayerStyles[GameData.P2Data.Style].PostGameP2Tie;
        }
        else {
            int winnerStyleIndex = GameData.Result == GameOverResult.P1Win ? GameData.P1Data.Style : GameData.P2Data.Style;
            int loserStyleIndex = GameData.Result == GameOverResult.P1Win ? GameData.P2Data.Style : GameData.P1Data.Style;
            winnerResultBoard.Initialize();
            winnerStats.Initialize(GameData.Result == GameOverResult.P1Win);
            loserStats.Initialize(GameData.Result != GameOverResult.P1Win);
            winnerImage.sprite = AssetManager.Inst.PlayerStyles[winnerStyleIndex].PostGameWinner;
            loserImage.sprite = AssetManager.Inst.PlayerStyles[loserStyleIndex].PostGameLoser;
        }
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
