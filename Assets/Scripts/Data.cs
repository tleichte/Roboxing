using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

