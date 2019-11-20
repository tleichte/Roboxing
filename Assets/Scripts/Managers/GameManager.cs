﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Waiting is used as an aux state until a coroutine finishes
public enum GameState { Start, Prefight, ReadyUp, Fighting, Down, GameDone, BetweenRounds, PreDecision }

public class GameManager : MonoBehaviour
{
    // Inspector Values

    [Header("Curtain Transition")]
    public CurtainTransition Curtain;

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

    private RoundStats[] p1Stats = new RoundStats[3];
    private RoundStats[] p2Stats = new RoundStats[3];
    public RoundStats P1RoundStats => p1Stats[round];
    public RoundStats P2RoundStats => p2Stats[round];

    private int countNum;

    private bool waitingForTimer;


    // UNITY FUNCTIONS

    void Awake() {
        Inst = this;
        State = GameState.Start;
        RoundTime = 180;
        for (int i = 0; i < p1Stats.Length; i++) {
            p1Stats[i] = new RoundStats();
            p2Stats[i] = new RoundStats();
        }
    }

    void Start() {
        Curtain.Open(() => {
            ToState(GameState.Prefight, 1f, () => OnPrefight?.Invoke());
        });
    }

    void LateUpdate() {
        
        switch (State) {
            case GameState.Start:

                //ToState(GameState.Prefight, 1.5f, () => OnPrefight?.Invoke());

                break;
            case GameState.Prefight:

                if (!waitingForTimer) RoundTime = 180;

                ToState(GameState.ReadyUp, 3, () => OnReadyUp?.Invoke());

                break;
            case GameState.ReadyUp:
                
                if (!waitingForTimer && Player1.IsReady && Player2.IsReady) {
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
                
                if (!waitingForTimer && !Player1.IsDown && !Player2.IsDown) {
                    OnPlayerRecover?.Invoke();
                    StopCoroutine(tenCountCR);
                    StartCoroutine(StartFightAfterDelay(2));
                }

                break;
            case GameState.BetweenRounds:
                if (!waitingForTimer) {
                    round++;
                    if (round == 3)
                        ToState(GameState.PreDecision, 8, () => OnPreDecision?.Invoke());
                    else
                        ToState(GameState.Prefight, 8, () => OnPrefight?.Invoke());
                }
                break;
            case GameState.PreDecision:

                ToState(GameState.GameDone, 3, () => {

                    //int p1Score = CalculateScore(p1Stats);
                    //int p2Score = CalculateScore(p2Stats);

                    //int cmp = p1Score.CompareTo(p2Score);

                    //GameOverResult r;
                    //if (cmp > 0) r = GameOverResult.P1Win;
                    //else if (cmp < 0) r = GameOverResult.P2Win;
                    //else r = GameOverResult.Tie;

                    EndGame(CalculateDecision(), GameOverReason.Decision);
                });
                break;
            case GameState.GameDone:
                
                //ToState(GameState.GameDone, 4, () => {
                //    Application.Quit();

                //});
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

    void ToState(GameState state, float delay, Action firstFrameAction) {
        IEnumerator WaitForTimer() {
            waitingForTimer = true;
            yield return new WaitForSeconds(delay);
            waitingForTimer = false;
            State = state;
            firstFrameAction?.Invoke();
        }
        if (!waitingForTimer) {
            StartCoroutine(WaitForTimer());
        }
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

                if (player1) p2Stats[round].Blocks++;
                else p1Stats[round].Blocks++;

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
                AddPunch(((player1) ? p1Stats : p2Stats)[round]);

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

        waitingForTimer = true;

        yield return new WaitForSeconds(secondsBeforeReady);

        OnFightReady?.Invoke();

        yield return new WaitForSeconds(2.5f);

        waitingForTimer = false;

        State = GameState.Fighting;

        OnFightStart?.Invoke();

        // Signal UI to start
    }

    IEnumerator WaitStun(Player p) {
        yield return new WaitForSeconds(Inst.stunTime);
        p.GM_FinishStun();
    }
    
    IEnumerator CheckKnockdown() {

        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(0.7f);
        Time.timeScale = 1;

        State = GameState.Down;

        // Add down to players
        if (Player1.IsDown) {
            p1Stats[round].Downs++;
            p1Downs++;
        }
        if (Player2.IsDown) {
            p2Stats[round].Downs++;
            p2Downs++;
        }

        OnPlayerDown?.Invoke();

        

        //
        int flag = 0;
        if (p1Stats[round].Downs >= 3) flag += 2;
        if (p2Stats[round].Downs >= 3) flag += 1;

        if (flag > 0) {

            State = GameState.GameDone;

            GameOverResult result;

            switch(flag) {
                case 1: result = GameOverResult.P1Win; break;
                case 2: result = GameOverResult.P2Win; break;
                default: result = GameOverResult.Tie; break;
            }
            yield return new WaitForSeconds(0.75f);
            EndGame(result, GameOverReason.TKO);
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

        GameOverResult result;
        switch (flag) {
            case 1: result = GameOverResult.P1Win; break;
            case 2: result = GameOverResult.P2Win; break;
            default: result = GameOverResult.Tie; break;
        }

        EndGame(result, GameOverReason.KO);
        // Game over

    }

    private GameOverResult CalculateDecision() {
        int p1Score = CalculateScore(p1Stats);
        int p2Score = CalculateScore(p2Stats);

        int cmp = p1Score.CompareTo(p2Score);

        if (cmp > 0) return GameOverResult.P1Win;
        if (cmp < 0) return GameOverResult.P2Win;
        return GameOverResult.Tie;
    }

    void EndGame(GameOverResult result, GameOverReason reason) {


        GameData.P1Stats = new PlayerStats(p1Stats);
        GameData.P2Stats = new PlayerStats(p2Stats);


        IEnumerator ExitAfterDelay() {
            yield return new WaitForSeconds(4);
            Curtain.Close(() => {
                SceneManager.LoadScene("PostGame");
            });
        }
        StartCoroutine(ExitAfterDelay());
        OnGameOver?.Invoke(result, reason);
    }


    // General functions
    public int GetDowns(bool player1) {
        if (player1) return p1Stats[round].Downs;
        else return p2Stats[round].Downs;
    }


}