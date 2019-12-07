using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    public Player Player { get; set; }

    [Header("Ready")]
    public GameObject ReadyBG;   
    public TMP_Text NotReadyText;
    public TMP_Text ReadyText;

    [Header("Down Game")]
    public DownGame DownGame;

    [Header("Background")]
    public Image UIBackground;
    public Color sleepColor;
    public Color dangerColor;
    public Color recoverColor;
    public Color hitJabColor;
    public Color hitHookColor;

    private Color targetColor;

    void Start()
    {
        DownGame.gameObject.SetActive(false);

        ReadyBG.SetActive(false);
        
        targetColor = new Color(0, 0, 0, 0);
        UIBackground.color = targetColor;
    }
    void OnDestroy() {
        
    }

    // GameManager Events

    public void PlayerNotReady() {
        ReadyBG.SetActive(true);
        ReadyText.gameObject.SetActive(false);
        NotReadyText.gameObject.SetActive(true);
    }

    public void FightReady() {
        ReadyBG.SetActive(false);
    }

    public void StartDownGame() {
        //if (Player.IsDown) {
            DownGame.gameObject.SetActive(true);
            DownGame.Initialize(Player);
        //}
    }

    public void PlayerReady() {
        NotReadyText.gameObject.SetActive(false);
        ReadyText.gameObject.SetActive(true);
    }
    

    public void PlayerRecovered() {
        DownGame.OnRecover();
        targetColor = recoverColor;
        IEnumerator FinishDownGame() {
            yield return new WaitForSeconds(1.5f);
            targetColor = new Color(0, 0, 0, 0);
            DownGame.gameObject.SetActive(false);
        }
        StartCoroutine(FinishDownGame());
    }

    public void PlayerHit(HitType type) {
        switch (type) {
            case HitType.Hook:
                UIBackground.color = hitHookColor;
                break;
            case HitType.Jab:
                UIBackground.color = hitJabColor;
                break;
        }
    }

    public void PlayerDown() {
        UIBackground.color = Color.white;
        targetColor = dangerColor;
    }

    public void EnterSleepMode() {
        DownGame.OnSleep();
        targetColor = sleepColor;
    }

    // Update is called once per frame
    void Update()
    {
        UIBackground.color = Color.Lerp(UIBackground.color, targetColor, 0.1f);
    }
}
