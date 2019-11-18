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

    public void OnReady(bool player1) {
        playersReady++;

        GameData.SetData(player1, new PlayerData(Player1.Letters, Player1.CurrentStyle));

        if (playersReady == 2) {
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
