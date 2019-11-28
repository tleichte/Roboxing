using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreGame : MonoBehaviour
{
    public PreGamePlayer Player1;
    public PreGamePlayer Player2;

    public CurtainTransition Curtain;

    private int playersReady;

    public bool Starting => Curtain.InProgress;


    private void Start() {
        Player1.Initialize(this);
        Player2.Initialize(this);
        Curtain.Open();
    }


    public void GoBack() {
        if (!Curtain.InProgress) {
            Curtain.Close(() => {
                SceneManager.LoadScene("MainMenu");
            });
        }
    }

    public bool IsStyleTaken(int style) {
        return (Player1.IsReady && Player1.CurrentStyle == style) || (Player2.IsReady && Player2.CurrentStyle == style);
    }

    public void OnReady(bool player1) {
        playersReady++;

        if (player1)
            GameData.P1Data = new PlayerData(Player1.Letters, Player1.CurrentStyle);
        else
            GameData.P2Data = new PlayerData(Player2.Letters, Player2.CurrentStyle);

        if (playersReady == 2) {
            AudioManager.Inst.StopSound("MainMenuSong");
            // Start game
            Curtain.Close(() => {
                SceneManager.LoadScene("InGame");
            });
        }
    }

    public void CancelReady(bool player1) {
        playersReady--;
    }
}
