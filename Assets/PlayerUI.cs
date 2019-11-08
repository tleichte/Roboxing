using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    
    public TMP_Text NotReadyText;
    public TMP_Text ReadyText;
    
    void Start()
    {
        NotReadyText.gameObject.SetActive(false);
        ReadyText.gameObject.SetActive(false);

        GameManager.Inst.OnReadyUp += OnPrefight;
        GameManager.Inst.OnFightReady += OnFightReady;
    }
    void OnDestroy() {
        GameManager.Inst.OnReadyUp -= OnPrefight;
        GameManager.Inst.OnFightReady -= OnFightReady;
    }

    void OnPrefight() {
        NotReadyText.gameObject.SetActive(true);
    }

    void OnFightReady() {
        ReadyText.gameObject.SetActive(false);
    }

    public void OnPlayerReady() {
        NotReadyText.gameObject.SetActive(false);
        ReadyText.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
