using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameStats : MonoBehaviour
{

    public TMP_Text NameLine;

    public PostGameStatsLine JabsLine;
    public PostGameStatsLine StunJabsLine;
    public PostGameStatsLine HooksLine;
    public PostGameStatsLine StunHooksLine;
    public PostGameStatsLine BlocksLine;
    public PostGameStatsLine ScoreLine;
    
    public void Initialize(bool player1) {

        PlayerData data = (player1) ? GameData.P1Data : GameData.P2Data;

        NameLine.text = $"{data.Name}'s Stats";
        NameLine.color = AssetManager.Inst.PlayerStyles[data.Style].UIColor;

        PlayerStats stats = (player1) ? GameData.P1Stats : GameData.P2Stats;
        JabsLine.Initialize(stats.Jabs);
        StunJabsLine.Initialize(stats.StunJabs);
        HooksLine.Initialize(stats.Hooks);
        StunHooksLine.Initialize(stats.StunHooks);
        BlocksLine.Initialize(stats.Blocks);
        ScoreLine.Initialize(stats.TotalScore);
    }
}
