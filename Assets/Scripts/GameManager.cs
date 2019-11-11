using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Waiting is used as an aux state until a coroutine finishes
public enum GameState { Start, Prefight, ReadyUp, Waiting, Fighting, Down, GameDone, BetweenRounds, PreDecision }

public enum GameOverResult { P1Win, P2Win, Tie }
public enum GameOverReason { KO, TKO, Decision }

public class RoundPunchStat {
    public int Hits;
    public int StunHits;
}
public class RoundStats {
    public RoundPunchStat Jabs = new RoundPunchStat();
    public RoundPunchStat Hooks = new RoundPunchStat();
    public int Blocks;
    public int Downs;
}

[Serializable]
public class PunchStat {
    public int Damage;
    public int Points;
    public float Shake;
}

[Serializable]
public class PunchStats {
    public PunchStat Standard;
    public PunchStat Stunned;
}

[Serializable]
public class HealthAmount {
    public int MinHealth;
    public int MaxHealth;
    public int CalculateHealth(int upNumber) {
        int difference = MaxHealth - MinHealth;
        float upPercent = (9 - upNumber) / 9.0f;
        return MinHealth + (int)(upPercent * difference);
    }
}

public class GameManager : MonoBehaviour
{
    // Inspector Values

    [Header("Players")]
    public Player Player1;
    public Player Player2;


    [Header("Punches")]
    public PunchStats HookStats;
    public PunchStats JabStats;


    [Header("Health")]
    public int InitialHealth;
    public HealthAmount[] HealthAmounts;

    [Header("Down Game")]
    public int[] NumWires;

    [Header("Points")]
    public int blockPoints;

    [Header("Screenshake")]
    public float shakeDecay;
    

    [Header("")]
    public int MaxDistance;    


    [Header("Time")]
    public float stunTime;
    public float RoundTimeSpeed;



    // Public Properties


    public Action OnPrefight;
    
    public Action OnReadyUp;
    public Action OnFightReady;
    public Action OnFightStart;

    public Action OnPlayerDown;
    public Action OnTenCountStart;
    public Action<int> OnTenCountNum;
    public Action OnPlayerRecover;

    public Action OnRoundOver;

    public Action OnPreDecision;

    public Action<GameOverResult, GameOverReason> OnGameOver;
    
    

    public static GameManager Inst { get; private set; }

    public GameState State;

    private int round;
    public int Round => round + 1;
    public float RoundTime { get; private set; }


    // Private Values

    private int p1Downs;
    private int p2Downs;

    private Coroutine stunCR;
    private Coroutine tenCountCR;
    private Coroutine knockDownCR;

    private bool fightStarting;

    private RoundStats[] P1Stats = new RoundStats[3];
    private RoundStats[] P2Stats = new RoundStats[3];

    private int countNum;



    // UNITY FUNCTIONS

    void Awake() {
        Inst = this;
        State = GameState.Start;
        for (int i = 0; i < P1Stats.Length; i++) {
            P1Stats[i] = new RoundStats();
            P2Stats[i] = new RoundStats();
        }
    }

    void LateUpdate() {
        
        switch (State) {
            case GameState.Start:
                StartCoroutine(
                    ToState(GameState.Prefight, 1.5f, () => OnPrefight?.Invoke())
                );
                break;
            case GameState.Prefight:
                RoundTime = 180;
                StartCoroutine(
                    ToState(GameState.ReadyUp, 3, () => OnReadyUp?.Invoke())
                );

                break;
            case GameState.ReadyUp:

                if (Player1.IsReady && Player2.IsReady) {
                    StartCoroutine(StartFightAfterDelay(1));
                }

                break;
            case GameState.Fighting:

                RoundTime -= Time.deltaTime * RoundTimeSpeed;
                if (RoundTime <= 0) {
                    RoundTime = 0;
                    State = GameState.BetweenRounds;
                    OnRoundOver?.Invoke();
                }

                break;

            case GameState.Down:

                if (!Player1.IsDown && !Player2.IsDown) {
                    OnPlayerRecover?.Invoke();
                    StopCoroutine(tenCountCR);
                    StartCoroutine(StartFightAfterDelay(2));
                }

                break;
            case GameState.BetweenRounds:
                round++;
                if (round == 3) {
                    StartCoroutine(
                        ToState(GameState.PreDecision, 5, () => OnPreDecision?.Invoke())
                    );
                }
                else {
                    StartCoroutine(
                        ToState(GameState.Prefight, 5, () => OnPrefight?.Invoke())
                    );
                }
                break;
            case GameState.PreDecision:
                StartCoroutine(
                    ToState(GameState.GameDone, 3, () => {
                        State = GameState.GameDone;

                        int p1Score = CalculateScore(P1Stats);
                        int p2Score = CalculateScore(P2Stats);

                        int cmp = p1Score.CompareTo(p2Score);

                        GameOverResult r;
                        if (cmp > 0) r = GameOverResult.P1Win;
                        else if (cmp < 0) r = GameOverResult.P2Win;
                        else r = GameOverResult.Tie;

                        OnGameOver?.Invoke(r, GameOverReason.Decision);
                    })
                );
                break;
        }

    }

    int CalculateScore(RoundStats[] stats) {
        int score = 0;
        foreach(var stat in stats) {
            score += stat.Hooks.Hits * HookStats.Standard.Points;
            score += stat.Hooks.StunHits * HookStats.Stunned.Points;
            score += stat.Jabs.Hits * HookStats.Standard.Points;
            score += stat.Jabs.StunHits * HookStats.Stunned.Points;
            score += stat.Blocks * blockPoints;
        }
        return score;
    }

    IEnumerator ToState(GameState state, float delay, Action firstFrameAction) {
        State = GameState.Waiting;
        yield return new WaitForSeconds(delay);
        State = state;
        firstFrameAction?.Invoke();
    }

    // CALLED BY PLAYERS

    public int P_GetHealth(bool player1) {
        return HealthAmounts[((player1) ? p1Downs : p2Downs)-1].CalculateHealth(countNum);
    }

    public int GetNumWires(bool player1) {
        return NumWires[((player1) ? p1Downs : p2Downs)-1];
    }

    public void P_CheckPunch(bool player1, Punch p) {
        Player aPlayer = null;
        Player dPlayer = null;


        aPlayer = (player1) ? Player1 : Player2;
        dPlayer = (player1) ? Player2 : Player1;



        PunchResult pr = dPlayer.GM_CheckPunch(aPlayer.Position, p);

        if (stunCR != null) StopCoroutine(stunCR);

        switch (pr) {
            case PunchResult.Blocked:

                if (player1) P2Stats[round].Blocks++;
                else P1Stats[round].Blocks++;

                aPlayer.GM_Stunned();
                stunCR = StartCoroutine(WaitStun(aPlayer));
                break;
            case PunchResult.Hit:

                void AddPunch(RoundStats stats) {
                    if (p.Type == HitType.Hook) {
                        if (dPlayer.IsStunned) stats.Hooks.StunHits++;
                        else stats.Hooks.Hits++;
                    }
                    else {
                        if (dPlayer.IsStunned) stats.Jabs.StunHits++;
                        else stats.Jabs.Hits++;
                    }
                }
                AddPunch(((player1) ? P1Stats : P2Stats)[round]);

                dPlayer.GM_Hit(p);

                //IEnumerator TimeFreeze() {
                //    Time.timeScale = 0;
                //    yield return new WaitForSecondsRealtime(p.Type == HitType.Hook ? 0.5f : 0.1f);
                //    Time.timeScale = 1;
                //}
                //StartCoroutine(TimeFreeze());

                break;
            default:
                break;
        }
    }

    public float P_GetShake(HitType ht, bool stunned) {
        switch (ht) {
            case HitType.Hook:
                return (stunned ? HookStats.Stunned : HookStats.Standard).Shake;
            case HitType.Jab:
                return (stunned ? JabStats.Stunned : JabStats.Standard).Shake;
            default:
                return 0;
        }
    }

    public int P_GetDamage(HitType ht, bool stunned) {
        switch (ht) {
            case HitType.Hook:
                return (stunned ? HookStats.Stunned : HookStats.Standard).Damage;
            case HitType.Jab:
                return (stunned ? JabStats.Stunned : JabStats.Standard).Damage;
            default:
                return 0;
        }
    }

    public void P_KnockedDown() {
        if (knockDownCR != null) StopCoroutine(knockDownCR);
        knockDownCR = StartCoroutine(CheckKnockdown());
    }


    // COROUTINES

    IEnumerator StartFightAfterDelay(float secondsBeforeReady) {

        State = GameState.Waiting;

        yield return new WaitForSeconds(secondsBeforeReady);

        OnFightReady?.Invoke();

        yield return new WaitForSeconds(2.5f);
        
        State = GameState.Fighting;

        OnFightStart?.Invoke();

        // Signal UI to start
    }

    IEnumerator WaitStun(Player p) {
        yield return new WaitForSeconds(Inst.stunTime);
        p.GM_FinishStun();
    }
    
    IEnumerator CheckKnockdown() {
        yield return null;

        State = GameState.Down;

        // Add down to players
        if (Player1.IsDown) {
            P1Stats[round].Downs++;
            p1Downs++;
        }
        if (Player2.IsDown) {
            P2Stats[round].Downs++;
            p2Downs++;
        }

        OnPlayerDown?.Invoke();

        

        //
        int flag = 0;
        if (P1Stats[round].Downs >= 3) flag += 2;
        if (P2Stats[round].Downs >= 3) flag += 1;

        if (flag > 0) {

            State = GameState.GameDone;

            GameOverResult r;
            switch(flag) {
                case 1: r = GameOverResult.P1Win; break;
                case 2: r = GameOverResult.P2Win; break;
                default: r = GameOverResult.Tie; break;
            }
            yield return new WaitForSeconds(0.75f);
            OnGameOver?.Invoke(r, GameOverReason.TKO);
        }
        else {         
            tenCountCR = StartCoroutine(TenCount());
        }
    }
    
    IEnumerator TenCount() {
        yield return new WaitForSeconds(0.75f);
        OnTenCountStart?.Invoke();
        yield return new WaitForSeconds(0.75f);
        for (countNum = 1; countNum <= 9; countNum++) {
            OnTenCountNum?.Invoke(countNum);
            yield return new WaitForSeconds(1);
            //Signal counter
        }




        State = GameState.GameDone;

        int flag = 0;
        if (Player1.IsDown) flag += 2;
        if (Player2.IsDown) flag += 1;

        GameOverResult r;
        switch (flag) {
            case 1: r = GameOverResult.P1Win; break;
            case 2: r = GameOverResult.P2Win; break;
            default: r = GameOverResult.Tie; break;
        }

        OnGameOver?.Invoke(r, GameOverReason.KO);
        // Game over

    }


    // General functions
    public int GetDowns(bool player1) {
        if (player1) return P1Stats[round].Downs;
        else return P2Stats[round].Downs;
    }


}
