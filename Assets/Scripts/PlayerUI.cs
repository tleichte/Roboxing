using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{

    public Player Player { get; set; }

    [Header("Ready")]
    public GameObject ReadyBG;   
    public TMP_Text NotReadyText;
    public TMP_Text ReadyText;

    [Header("Down Game")]
    public DownGame DownGame;

    void Start()
    {
        DownGame.gameObject.SetActive(false);

        ReadyBG.SetActive(false);

        GameManager.Inst.OnReadyUp += OnPrefight;
        GameManager.Inst.OnFightReady += OnFightReady;
        GameManager.Inst.OnTenCountStart += OnTenCountStart;
        
    }
    void OnDestroy() {
        GameManager.Inst.OnReadyUp -= OnPrefight;
        GameManager.Inst.OnFightReady -= OnFightReady;
        GameManager.Inst.OnTenCountStart -= OnTenCountStart;
    }

    void OnPrefight() {
        ReadyBG.SetActive(true);
        ReadyText.gameObject.SetActive(false);
        NotReadyText.gameObject.SetActive(true);
    }

    void OnFightReady() {
        ReadyBG.SetActive(false);
    }

    void OnTenCountStart() {
        if (Player.IsDown) {
            DownGame.gameObject.SetActive(true);
            DownGame.Initialize(Player);
        }
    }

    public void OnPlayerReady() {
        NotReadyText.gameObject.SetActive(false);
        ReadyText.gameObject.SetActive(true);
    }

    


    public void PlayerRecovered() {
        DownGame.OnRecover();
        IEnumerator FinishDownGame() {
            yield return new WaitForSeconds(1.5f);
            DownGame.gameObject.SetActive(false);
        }
        StartCoroutine(FinishDownGame());
    }

    // Update is called once per frame
    void Update()
    {

    }
}
