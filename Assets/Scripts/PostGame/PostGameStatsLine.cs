using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameStatsLine : MonoBehaviour
{

    public TMP_Text AmountText;
    public TMP_Text ScoreText;

    public void Initialize(int score) {
        AmountText.gameObject.SetActive(false);
        ScoreText.text = $"{score}";
    }


    public void Initialize(PlayerStat stat) {
        AmountText.text = $"{stat.Amount}";
        ScoreText.text = $"{stat.Score}";
    }
}
