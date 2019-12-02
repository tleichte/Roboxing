using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    [Header("Names")]
    public TMP_Text P1NameText;
    public TMP_Text P2NameText;

    [Header("Stats")]
    public GameObject statsGO;
    public TMP_Text statsHeader;
    public TMP_Text statsP1Hooks;
    public TMP_Text statsP2Hooks;
    public TMP_Text statsP1Jabs;
    public TMP_Text statsP2Jabs;
    public TMP_Text statsP1Blocks;
    public TMP_Text statsP2Blocks;
    public TMP_Text statsP1Downs;
    public TMP_Text statsP2Downs;


    [Header("Timer")]
    public TMP_Text timerMin;
    public TMP_Text timerSec1;
    public TMP_Text timerSec2;

    [Header("Round")]
    public TMP_Text round1;
    public TMP_Text round2;
    public TMP_Text round3;

    [Header("Downs")]
    public GameObject p1Down1;
    public GameObject p1Down2;
    public GameObject p1Down3;
    public GameObject p2Down1;
    public GameObject p2Down2;
    public GameObject p2Down3;

    [Header("Health")]
    public Image p1HealthBar;
    public Image p2HealthBar;

    [Header("Text")]

    public GameObject KOTextGO;
    public TMP_Text KOText;

    public GameObject StatusTextGO;
    public TMP_Text StatusText;

    public GameObject NumTextGO;
    public TMP_Text NumText;


    [Header("Decision")]
    public GameObject DecisionTextGO;
    public TMP_Text DecisionText;

    [Header("Round Card")]
    public GameObject RoundCardGO;
    public TMP_Text RoundCardNum;

    // Start is called before the first frame update
    private void Start() {
        statsGO.SetActive(false);
        KOTextGO.SetActive(false);
        StatusTextGO.SetActive(false);
        NumTextGO.SetActive(false);
        RoundCardGO.SetActive(false);
        DecisionTextGO.SetActive(false);
        DecisionText.gameObject.SetActive(false);

        GameManager.Inst.OnReadyUp += OnReadyUp;
        GameManager.Inst.OnPrefight += OnPrefight;
        GameManager.Inst.OnFightReady += OnFightReady;
        GameManager.Inst.OnFightStart += OnFightStart;
        GameManager.Inst.OnPlayerDown += OnPlayerDown;
        GameManager.Inst.OnTenCountStart += OnTenCountStart;
        GameManager.Inst.OnTenCountNum += OnTenCountNum;
        GameManager.Inst.OnGameOver += OnGameOver;
        GameManager.Inst.OnRoundOver += OnRoundOver;
        GameManager.Inst.OnPreDecision += OnPreDecision;



        p1Down1.SetActive(false);
        p1Down2.SetActive(false);
        p1Down3.SetActive(false);
        p2Down1.SetActive(false);
        p2Down2.SetActive(false);
        p2Down3.SetActive(false);

        SetRoundText(round1, -1);
        SetRoundText(round2, -1);
        SetRoundText(round3, -1);

        P1NameText.text = GameData.P1Data.Name;
        P2NameText.text = GameData.P2Data.Name;

        int p1i = GameData.P1Data.Style;
        int p2i = GameData.P2Data.Style;
        p1HealthBar.color = AssetManager.Inst.PlayerStyles[p1i].UIColor;
        p2HealthBar.color = AssetManager.Inst.PlayerStyles[p2i].UIColor;
    }

    private void OnDestroy() {
        GameManager.Inst.OnPrefight -= OnPrefight;
        GameManager.Inst.OnReadyUp -= OnReadyUp;
        GameManager.Inst.OnFightReady -= OnFightReady;
        GameManager.Inst.OnFightStart -= OnFightStart;
        GameManager.Inst.OnPlayerDown -= OnPlayerDown;
        GameManager.Inst.OnTenCountStart -= OnTenCountStart;
        GameManager.Inst.OnTenCountNum -= OnTenCountNum;
        GameManager.Inst.OnGameOver -= OnGameOver;
        GameManager.Inst.OnRoundOver -= OnRoundOver;
        GameManager.Inst.OnPreDecision -= OnPreDecision;
    }


    // ACTIONS 

    void OnPrefight() {
        statsGO.SetActive(false);
        RoundCardGO.SetActive(true);
        RoundCardNum.text = $"{GameManager.Inst.Round}";
        SetRoundText(round1, 1);
        SetRoundText(round2, 2);
        SetRoundText(round3, 3);
        RefreshDowns();
    }

    void OnReadyUp() {
        RoundCardGO.SetActive(false);
    }

    void OnFightReady() {
        NumTextGO.SetActive(false);
        StatusText.text = "Ready...";
        Show(StatusTextGO);
    }
    
    void OnFightStart() {
        StatusText.text = "FIGHT!";
        ShowThenHide(StatusTextGO, hideDelay: 0.5f);
    }
    
    void OnPlayerDown() {
        RefreshDowns();
        StatusText.text = "DOWN";
        ShowThenHide(StatusTextGO);
    }

    void OnTenCountStart() {
        //Show(NumTextGO);
    }

    void OnTenCountNum(int num) {
        NumText.text = $"{num}";
        ShowThenHide(NumTextGO, hideDelay: 0.8f);
        //if (num == 10) {
        //    Hide(NumTextGO, 1);
        //}
    }

    void OnRoundOver() {

        statsGO.SetActive(true);

        statsHeader.text = $"Round {GameManager.Inst.Round} Done";

        statsP1Hooks.text = $"{GameManager.Inst.P1RoundStats.Hooks.Total}";
        statsP2Hooks.text = $"{GameManager.Inst.P2RoundStats.Hooks.Total}";
        statsP1Jabs.text = $"{GameManager.Inst.P1RoundStats.Jabs.Total}";
        statsP2Jabs.text = $"{GameManager.Inst.P2RoundStats.Jabs.Total}";
        statsP1Blocks.text = $"{GameManager.Inst.P1RoundStats.Blocks}";
        statsP2Blocks.text = $"{GameManager.Inst.P2RoundStats.Blocks}";
        statsP1Downs.text = $"{GameManager.Inst.P1RoundStats.Downs}";
        statsP2Downs.text = $"{GameManager.Inst.P2RoundStats.Downs}";

        // Bell sound
        // TODO Show Bell?
    }
    
    void OnPreDecision() {
        statsGO.SetActive(false);
        DecisionTextGO.SetActive(true);
        // show the winner is text
    }

    void OnGameOver(GameOverResult result, GameOverReason reason) {

        void ShowKOText(string text, float showDelay = 0) {
            KOText.text = text;
            Show(KOTextGO, showDelay);
        }

        bool isTie = result == GameOverResult.Tie;

        switch (reason) {
            case GameOverReason.KO:
                OnTenCountNum(10);
                ShowKOText(isTie ? "Tie!" : "KO!", 1.25f);
                break;
            case GameOverReason.TKO:
                ShowKOText(isTie ? "Tie!" : "TKO!");
                break;
            case GameOverReason.Decision:
                
                switch (result) {
                    case GameOverResult.P1Win: DecisionText.text = "P1 Wins!"; break;
                    case GameOverResult.P2Win: DecisionText.text = "P2 Wins!"; break;
                    case GameOverResult.Tie: DecisionText.text = "Tie!"; break;
                }
                DecisionText.gameObject.SetActive(true);

                break;
        }

    }

    // HELPERS

    void SetRoundText(TMP_Text text, int round) {
        Color c = text.color;
        c.a = round == GameManager.Inst.Round ? 1f : 0.3f;
        text.color = c;
    }

    void ShowThenHide(GameObject go, float showDelay = 0, float hideDelay = 1f) {
        IEnumerator Delay() {
            yield return new WaitForSeconds(showDelay);
            go.SetActive(true);
            yield return new WaitForSeconds(hideDelay);
            go.SetActive(false);
        }
        StartCoroutine(Delay());
    }

    void Show(GameObject go, float showDelay = 0) {
        IEnumerator Delay() {
            yield return new WaitForSeconds(showDelay);
            go.SetActive(true);
        }
        StartCoroutine(Delay());
    }

    void Hide(GameObject go, float hideDelay = 1) {
        IEnumerator Delay() {
            yield return new WaitForSeconds(hideDelay);
            go.SetActive(false);
        }
        StartCoroutine(Delay());
    }

    void RefreshDowns() {
        int p1Downs = GameManager.Inst.GetDowns(true);
        int p2Downs = GameManager.Inst.GetDowns(false);
        
        p1Down1.SetActive(p1Downs >= 1);
        p1Down2.SetActive(p1Downs >= 2);
        p1Down3.SetActive(p1Downs >= 3);
        p2Down1.SetActive(p2Downs >= 1);
        p2Down2.SetActive(p2Downs >= 2);
        p2Down3.SetActive(p2Downs >= 3);
    }

    // Update is called once per frame
    void Update()
    {
        float time = GameManager.Inst.RoundTime;
        int mins = (int)time / 60;
        int secs = (int)time % 60;
        timerMin.text = $"{mins}";
        timerSec1.text = $"{secs / 10}";
        timerSec2.text = $"{secs % 10}";

        p1HealthBar.fillAmount = Mathf.Lerp(p1HealthBar.fillAmount, GameManager.Inst.Player1.HealthPercent, 0.4f);
        p2HealthBar.fillAmount = Mathf.Lerp(p2HealthBar.fillAmount, GameManager.Inst.Player2.HealthPercent, 0.4f);
    }




}
