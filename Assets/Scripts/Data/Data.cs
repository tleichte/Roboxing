using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public enum GameOverResult { P1Win, P2Win, Tie }
public enum GameOverReason { KO, TKO, Decision }

public enum PunchResult { Missed, Blocked, Hit }
public enum HitType { Hook, Jab }

public struct Direction {
    public bool Left;
    public bool Up;
    public Direction(bool left, bool up) {
        Left = left;
        Up = up;
    }
}

public class Punch {
    public Direction Direction;
    public HitType Type;
    public Punch(Direction d, HitType type) {
        Direction = d;
        Type = type;
    }
}

public class RoundPunchStat {
    public int Hits;
    public int StunHits;
    public int Total => (Hits + StunHits);
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
    public int MaxHealth;
    public int MinHealth;
    public int CalculateHealth(int upNumber) {
        int difference = MaxHealth - MinHealth;
        float upPercent = (9 - upNumber) / 9.0f;
        //Debug.Log(MinHealth + (int)(upPercent * difference));
        return MinHealth + (int)(upPercent * difference);
    }
}


[Serializable]
public class SwooshAnimationProps {
    public float Time = 0.16f;
    [FormerlySerializedAs("XPos")]
    public float Position = 0;
    public AnimationCurve Curve;
}


public class PlayerStat {
    public int Amount;
    public int Score;
}

public class PlayerStats {

    public PlayerStat Jabs = new PlayerStat();
    public PlayerStat StunJabs = new PlayerStat();
    public PlayerStat Hooks = new PlayerStat();
    public PlayerStat StunHooks = new PlayerStat();
    public PlayerStat Blocks = new PlayerStat();
    public int TotalScore;

    public PlayerStats() { }

    public PlayerStats(RoundStats[] rounds) {

        void AddToStat(PlayerStat stat, int amount, int points) {
            stat.Amount += amount;
            stat.Score += amount * points;
        }

        var JabStats = GameManager.Inst.JabStats;
        var HookStats = GameManager.Inst.HookStats;

        foreach(var round in rounds) {
            AddToStat(Jabs,         round.Jabs.Hits,        JabStats.Standard.Points);
            AddToStat(StunJabs,     round.Jabs.StunHits,    JabStats.Stunned.Points);
            AddToStat(Hooks,        round.Hooks.Hits,       HookStats.Standard.Points);
            AddToStat(StunHooks,    round.Hooks.StunHits,   HookStats.Stunned.Points);
            AddToStat(Blocks,       round.Blocks,           GameManager.Inst.blockPoints);
        }

        TotalScore =
            Jabs.Score +
            StunJabs.Score +
            Hooks.Score +
            StunHooks.Score +
            Blocks.Score;

    }
}

public class PlayerData {
    public string Name;
    public int Style;
    public PlayerData(string name, int style) {
        Name = name;
        Style = style;
    }
    public PlayerData(char[] name, int style) : this(new string(name), style) { }
}

public static class GameData {
    public static PlayerData P1Data = new PlayerData("P1L", 1);
    public static PlayerData P2Data = new PlayerData("P2R", 0);

    public static PlayerStats P1Stats = new PlayerStats();
    public static PlayerStats P2Stats = new PlayerStats();

    public static GameOverResult Result = GameOverResult.Tie;
    public static GameOverReason Reason = GameOverReason.KO;

    public static void SetData(bool player1, PlayerData data) {
        if (player1)
            P1Data = data;
        else
            P2Data = data;
    }
}