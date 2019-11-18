using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreGame : MonoBehaviour
{
    public PreGamePlayer Player1;
    public PreGamePlayer Player2;

    private int playersReady;

    public bool Starting { get; private set; }


    private void Start() {
        Player1.Initialize(this);
        Player2.Initialize(this);
    }


    public void GoBack() {
        if (!Starting) {
            Starting = true;
            IEnumerator GoBackAfterDelay() {
                yield return new WaitForSecondsRealtime(0.5f);
                SceneManager.LoadScene("MainMenu");
            }
            StartCoroutine(GoBackAfterDelay());
        }
    }

    public void OnReady(bool player1) {
        playersReady++;

        GameData.SetData(player1, new PlayerData(Player1.Letters, Player1.CurrentStyle));

        if (playersReady == 2) {
            // Start game
            IEnumerator StartGameAfterDelay() {
                Starting = true;
                yield return new WaitForSecondsRealtime(2);
                SceneManager.LoadScene("InGame");
            }
            StartCoroutine(StartGameAfterDelay());
        }
    }

    public void CancelReady(bool player1) {
        playersReady--;
    }
}
