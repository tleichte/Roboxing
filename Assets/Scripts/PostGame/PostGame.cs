using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{

    public float PostGameDuration;
    public Image TimeImage;

    private float time;

    public TMP_Text ResultText;
    public TMP_Text ReasonText;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        
        string preReason = "";
        if (GameData.Result == GameOverResult.Tie) {
            ResultText.text = "Tie!";
            preReason = "Double ";
        }
        else {
            string winner = "";
            switch (GameData.Result) {
                case GameOverResult.P1Win: winner = GameData.P1Data.Name; break;
                case GameOverResult.P2Win: winner = GameData.P2Data.Name; break;
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

        yield return new WaitForSecondsRealtime(PostGameDuration);

        CurtainTransition.Inst.Close(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        TimeImage.fillAmount = time / PostGameDuration;
    }
}
