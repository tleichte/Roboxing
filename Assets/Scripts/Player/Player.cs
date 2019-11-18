using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum PlayerState { PreFight, NotReady, Recover, Ready, Hit, Down, Idle, UpBlock, DownBlock, Punching, Stunned }

public class Player : MonoBehaviour {

    public int Health { get; private set; }

    public bool Player1;
    public HGDCabPlayer PlayerPos { get; private set; }
    
    [Header("First Person")]
    public Animator FPAnim;

    [Header("Third Person (SET OUTSIDE PREFAB)")]
    public Animator TPAnim;
    public Transform TPTransform;

    [Header("Camera")]
    public Transform CameraContainer;

    [Header("Variable Values")]
    public float positionDistance;
    public float movementLerp;

    [Header("UI")]
    public PlayerUI ui;

    [Header("PREVIEW")]
    [SerializeField] private PlayerState state;



    public int Position { get; private set; }
    public bool IsReady => state == PlayerState.Ready;
    public bool IsDown => state == PlayerState.Down;
    public bool IsStunned => state == PlayerState.Stunned;

    //Starting Positions
    private Vector3 fpStartingPos;
    private Vector3 tpStartingPos;

    //
    private Vector3 currMoveOffset;

    private bool punchSide; // true for left, false for right

    Punch currentPunch;

    float shakeAmount;

    int maxHealth;
    public float HealthPercent => (float)Health / maxHealth;


    private bool CanMove { get {
            switch (state) {
                case PlayerState.Idle:
                case PlayerState.UpBlock:
                case PlayerState.DownBlock:
                    return true;
                default:
                    return false;
            }
        } }

    void Awake() {
        ui.Player = this;
    }

    void Start() {
        state = PlayerState.PreFight;
        PlayerPos = Player1 ? HGDCabPlayer.P1 : HGDCabPlayer.P2;
        fpStartingPos = transform.position;
        tpStartingPos = TPTransform.position;

        Health = GameManager.Inst.InitialHealth;
        maxHealth = Health;

        GameManager.Inst.OnPrefight += OnPrefight;
        GameManager.Inst.OnReadyUp += OnReadyUp;
        GameManager.Inst.OnPlayerDown += PlayerDown;
        GameManager.Inst.OnFightStart += FightStart;
        GameManager.Inst.OnRoundOver += RoundOver;
        GameManager.Inst.OnGameOver += OnGameOver;
        GameManager.Inst.OnTenCountStart += OnTenCountStart;
    }

    private void OnDestroy() {
        GameManager.Inst.OnPrefight -= OnPrefight;
        GameManager.Inst.OnReadyUp -= OnReadyUp;
        GameManager.Inst.OnPlayerDown -= PlayerDown;
        GameManager.Inst.OnFightStart -= FightStart;
        GameManager.Inst.OnRoundOver -= RoundOver;
        GameManager.Inst.OnGameOver -= OnGameOver;
        GameManager.Inst.OnTenCountStart -= OnTenCountStart;
    }


    void CalculateCamera() {
        Vector3 targetOffset = Vector3.right * Position * positionDistance;
        currMoveOffset = Vector3.Lerp(currMoveOffset, targetOffset, movementLerp);

        transform.position = fpStartingPos + currMoveOffset;
        TPTransform.position = tpStartingPos - currMoveOffset;
        
        //Screenshake
        if (shakeAmount > 0) {
            CameraContainer.localPosition = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1)) * shakeAmount;
            shakeAmount -= GameManager.Inst.shakeDecay;
            shakeAmount = Mathf.Clamp01(shakeAmount);
        }
        else CameraContainer.localPosition = Vector3.zero;
    }

    void Update() {

        CalculateCamera();

        if (CanMove) {
            if (Input.GetKeyDown(HGDCabKeys.Of(PlayerPos).JoyLeft))         Move(-1);
            else if (Input.GetKeyDown(HGDCabKeys.Of(PlayerPos).JoyRight))   Move(1);
        }

        switch (state) {

            case PlayerState.PreFight:
                
                break;

            case PlayerState.NotReady:
                if (Input.GetKeyDown(HGDCabKeys.Of(PlayerPos).Top1)) {
                    state = PlayerState.Ready;
                    ui.OnPlayerReady();
                }
                break;

            case PlayerState.Hit:
                break;

            case PlayerState.Down:
                break;

            case PlayerState.UpBlock:
                if (!Input.GetKey(HGDCabKeys.Of(PlayerPos).JoyUp)) StopBlock();
                break;

            case PlayerState.DownBlock:
                if (!Input.GetKey(HGDCabKeys.Of(PlayerPos).JoyDown)) StopBlock();
                break;

            case PlayerState.Idle:
                //if(!Player1) {
                //    Debug.Log($"Player 1?({Player1}): Throwing Punch from Idle");
                //    ThrowPunch(true, HitType.Hook);
                //}
                //break;
                
                if (Input.GetKey(HGDCabKeys.Of(PlayerPos).Top1))       ThrowPunch(true, HitType.Jab);
                else if (Input.GetKey(HGDCabKeys.Of(PlayerPos).Top2))       ThrowPunch(true, HitType.Hook);

                else if (Input.GetKey(HGDCabKeys.Of(PlayerPos).Bottom1))    ThrowPunch(false, HitType.Jab);
                else if (Input.GetKey(HGDCabKeys.Of(PlayerPos).Bottom2))    ThrowPunch(false, HitType.Hook);

                else if (Input.GetKey(HGDCabKeys.Of(PlayerPos).JoyUp)) Block(true);
                else if (Input.GetKey(HGDCabKeys.Of(PlayerPos).JoyDown)) Block(false);

                break;

            case PlayerState.Punching:
                break;

            default:
                break;
        }
    }

    // PLAYER HELPER FUNCTIONS

    void Move(int delta) {
        if (Math.Abs(Position + delta) <= GameManager.Inst.MaxDistance)
            Position += delta;
    }

    void Block(bool up) {
        state = up ? PlayerState.UpBlock : PlayerState.DownBlock;
        
        string dirT = up ? "Up" : "Down";
        SetAnimators(anim => {
            anim.SetBool("Block", true);
            anim.SetTrigger(dirT);
        });
    }

    void StopBlock() {
        state = PlayerState.Idle;
        SetAnimators(anim => anim.SetBool("Block", false));
    }

    void ThrowPunch(bool up, HitType ht) {
        state = PlayerState.Punching;

        Direction d = new Direction { Left = punchSide, Up = up };

        currentPunch = new Punch(d, ht);

        SetAnimators(anim => {
            anim.SetTrigger(punchSide ? "Left" : "Right");
            anim.SetTrigger(up ? "Up" : "Down");
            anim.SetTrigger(ht == HitType.Hook ? "Hook" : "Jab");
        });

        punchSide = !punchSide;
    }

    void KnockedDown() {
        state = PlayerState.Down;
        SetAnimators(anim => {
            anim.SetBool("KnockDown", true);
            anim.SetBool("Fighting", false);
        });
        GameManager.Inst.P_KnockedDown();
    }

    public void Recover() {
        state = PlayerState.Recover;
        SetAnimators(anim => anim.SetBool("KnockDown", false));
        Health = GameManager.Inst.P_GetHealth(Player1);

        ui.PlayerRecovered();
    }

    private void SetAnimators(Action<Animator> armsAction) {
        armsAction?.Invoke(FPAnim);
        armsAction?.Invoke(TPAnim);
    }


    // CALLED BY GAMEMANAGER
    

    public PunchResult GM_CheckPunch(int enemyPosition, Punch p) {

        if (state == PlayerState.Down || enemyPosition != -Position) return PunchResult.Missed;

        PlayerState blockState = p.Direction.Up ? PlayerState.UpBlock : PlayerState.DownBlock;

        return state == blockState ? PunchResult.Blocked : PunchResult.Hit;
    }

    public void GM_Hit(Punch p) {
        IEnumerator GetHitAfterUpdate() {
            yield return null;

            shakeAmount = GameManager.Inst.P_GetShake(p.Type, IsStunned);

            SetAnimators(anim => anim.SetBool("Stunned", false));

            if ((Health -= GameManager.Inst.P_GetDamage(p.Type, IsStunned)) <= 0) {
                KnockedDown();
            }
            else {
                state = PlayerState.Hit;
                
                SetAnimators((anim) => {
                    anim.SetTrigger("Hit");
                    anim.SetTrigger(p.Type == HitType.Hook ? "Hook" : "Jab");
                    anim.SetTrigger(p.Direction.Left ? "Left" : "Right");
                    anim.SetTrigger(p.Direction.Up ? "Up" : "Down");
                });
            }
        }
        StartCoroutine(GetHitAfterUpdate());
    }

    public void GM_Stunned() {
        state = PlayerState.Stunned;
        SetAnimators((anim) => anim.SetBool("Stunned", true));
    }

    public void GM_FinishStun() {
        state = PlayerState.Idle;
        SetAnimators((anim) => anim.SetBool("Stunned", false));
    }

    private void OnPrefight() {
        Position = 0;
        state = PlayerState.PreFight;
    }

    private void OnReadyUp() {
        state = PlayerState.NotReady;
    }

    private void PlayerDown() {
        if (state != PlayerState.Down) {
            state = PlayerState.Ready;
            StopFighting();
        }
    }

    private void FightStart() {
        state = PlayerState.Idle;
        SetAnimators(anim => anim.SetBool("Fighting", true));
    }

    private void RoundOver() {
        state = PlayerState.NotReady;
        StopFighting();
    }

    private void StopFighting() {
        SetAnimators(anim => anim.SetBool("Fighting", false));
    }

    private void OnGameOver(GameOverResult result, GameOverReason reason) {
        bool won = false;
        switch (result) {
            case GameOverResult.P1Win:
                won = Player1;
                break;
            case GameOverResult.P2Win:
                won = !Player1;
                break;
        }

        // Set to sleep mode
        if (won) {
            if (IsDown) Recover();
            SetAnimators(anim => anim.SetBool("Victory", true));
        }
    }

    private void OnTenCountStart() {
        //TODO Start down game
    }

    //CALLED BY ANIMATOR

    public void A_Hit() {
        if (state == PlayerState.Punching)
            GameManager.Inst.P_CheckPunch(Player1, currentPunch);
    }

    public void A_ToIdle() {
        if (state != PlayerState.Ready) // Don't go to idle state if the player state is "ready"
            state = PlayerState.Idle;
    }

    public void A_AfterDownReady() {
        state = PlayerState.Ready;
    }
}
