using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Waiting is used as an aux state until a coroutine finishes
public enum GameState { Start, Prefight, ReadyUp, Waiting, Fighting, Down, GameDone, BetweenRounds, PreDecision }

public enum GameOverResult { P1Win, P2Win, Tie }
public enum GameOverReason { KO, TKO, Decision }

public class RoundStats {
    public int Jabs;
    public int Hooks;
    public int Blocks;
    public int Downs;
}

public class GameManager : MonoBehaviour
{
    // Inspector Values

    [Header("Players")]
    public Player Player1;
    public Player Player2;
    

    [Header("Health/Damage")]
    public int jabDamage;
    public int hookDamage;
    public int[] HealthAmounts;

    [Header("Scores")]
    public int jabPoints;
    public int hookPoints;
    public int blockPoints;

    [Header("Screenshake")]
    public float jabShake;
    public float hookShake;
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

    private int p1HealthI;
    private int p2HealthI;

    private Coroutine stunCR;
    private Coroutine tenCountCR;
    private Coroutine knockDownCR;

    private bool fightStarting;

    private RoundStats[] P1Stats = new RoundStats[3];
    private RoundStats[] P2Stats = new RoundStats[3];




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
            score += stat.Hooks * hookPoints;
            score += stat.Jabs * jabPoints;
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
        int health = 0;
        if (player1) {
            health = HealthAmounts[p1HealthI];
            p1HealthI++;
        }
        else {
            health = HealthAmounts[p2HealthI];
            p2HealthI++;
        }
        return health;
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

                void AddPunch(RoundStats stats, HitType ht) {
                    if (ht == HitType.Hook) stats.Hooks++;
                    else stats.Jabs++;
                }
                AddPunch(((player1) ? P1Stats : P2Stats)[round], p.Type);

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

    public float P_GetShake(HitType ht) {
        switch (ht) {
            case HitType.Hook:
                return hookShake;
            case HitType.Jab:
                return jabShake;
            default:
                return 0;
        }
    }

    public int P_GetDamage(HitType ht) {
        switch (ht) {
            case HitType.Hook:
                return hookDamage;
            case HitType.Jab:
                return jabDamage;
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
        if (Player1.IsDown) P1Stats[round].Downs++;
        if (Player2.IsDown) P2Stats[round].Downs++;

        OnPlayerDown?.Invoke();

        yield return new WaitForSeconds(1.5f);

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
            OnGameOver?.Invoke(r, GameOverReason.TKO);
        }
        else {
            tenCountCR = StartCoroutine(TenCount());
        }
    }
    
    IEnumerator TenCount() {
        OnTenCountStart?.Invoke();
        for (int i = 1; i <= 9; i++) {
            OnTenCountNum?.Invoke(i);
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
