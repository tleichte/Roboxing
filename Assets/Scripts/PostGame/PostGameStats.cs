using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostGameStats : MonoBehaviour
{

    public PostGameStatsLine JabsLine;
    public PostGameStatsLine StunJabsLine;
    public PostGameStatsLine HooksLine;
    public PostGameStatsLine StunHooksLine;
    public PostGameStatsLine BlocksLine;
    public PostGameStatsLine ScoreLine;
    
    public void Initialize(bool player1) {
        PlayerStats stats = (player1) ? GameData.P1Stats : GameData.P2Stats;
        JabsLine.Initialize(stats.Jabs);
        StunJabsLine.Initialize(stats.StunJabs);
        HooksLine.Initialize(stats.Hooks);
        StunHooksLine.Initialize(stats.StunHooks);
        BlocksLine.Initialize(stats.Blocks);
        ScoreLine.Initialize(stats.TotalScore);
    }
}
