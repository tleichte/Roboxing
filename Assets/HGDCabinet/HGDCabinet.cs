using System;
using UnityEngine;



/* 

    ** How to Use HGDCabKeys **
    
    ____________________________
    HGDCabKeys.{Player}.{Button}
    ____________________________

        {Player} : P1, P2, Left, Right, Of(HGDCabPlayer)
        {Button} : Player, Coin, Joy[Up, Down, Left, Right], Top[1-4], Bottom[1-4]


    ** Functions **
    
    ___________________________________
    HGDCabKeys.Of(HGDCabPlayer player):
    ___________________________________
        
        Returns the Key Layout for the specified player. Useful for HGDCabPlayer variables.

    


*/






public enum HGDCabPlayer { Left, Right, P1, P2 }

public static class HGDCabKeys {
    
    public static KeyLayout P1 => Left;
    public static readonly KeyLayout Left = new KeyLayout(HGDCabPlayer.Left);

    public static KeyLayout P2 => Right;
    public static readonly KeyLayout Right = new KeyLayout(HGDCabPlayer.Right);

    public static KeyLayout Of(HGDCabPlayer p) {
        switch (p) {
            case HGDCabPlayer.Left:
            case HGDCabPlayer.P1:
                return Left;
            case HGDCabPlayer.Right:
            case HGDCabPlayer.P2:
                return Right;
            default:
                return null;
        }
    }

    public class KeyLayout {

        public readonly KeyCode JoyUp, JoyDown, JoyRight, JoyLeft;
        public readonly KeyCode Top1, Top2, Top3, Top4;
        public readonly KeyCode Bottom1, Bottom2, Bottom3, Bottom4;
        public readonly KeyCode Player, Coin;
            
        public KeyLayout(HGDCabPlayer p) {
            switch (p) {
                case HGDCabPlayer.Left:
                case HGDCabPlayer.P1:
                    JoyUp = KeyCode.W;
                    JoyLeft = KeyCode.A;
                    JoyDown = KeyCode.S;
                    JoyRight = KeyCode.D;
                    Top1 = KeyCode.T;
                    Top2 = KeyCode.Y;
                    Top3 = KeyCode.U;
                    Top4 = KeyCode.I;
                    Bottom1 = KeyCode.F;
                    Bottom2 = KeyCode.G;
                    Bottom3 = KeyCode.H;
                    Bottom4 = KeyCode.J;
                    Player = KeyCode.KeypadEnter;
                    Coin = KeyCode.Escape;
                    break;
                case HGDCabPlayer.Right:
                case HGDCabPlayer.P2:
                    JoyUp = KeyCode.UpArrow;
                    JoyLeft = KeyCode.LeftArrow;
                    JoyDown = KeyCode.DownArrow;
                    JoyRight = KeyCode.RightArrow;
                    Top1 = KeyCode.Keypad1;
                    Top2 = KeyCode.Keypad2;
                    Top3 = KeyCode.Keypad3;
                    Top4 = KeyCode.Keypad4;
                    Bottom1 = KeyCode.Keypad5;
                    Bottom2 = KeyCode.Keypad6;
                    Bottom3 = KeyCode.Keypad7;
                    Bottom4 = KeyCode.Keypad8;
                    Player = KeyCode.Keypad0;
                    Coin = KeyCode.Keypad9;
                    break;
            }
        }
    }

    public static Vector2 JoystickPosition(HGDCabPlayer p) {
        KeyLayout l = Of(p);

        Vector2 outVec = Vector2.zero;
        if (Input.GetKey(l.JoyUp)) outVec.y = 1;
        if (Input.GetKey(l.JoyDown)) outVec.y = -1;
        if (Input.GetKey(l.JoyLeft)) outVec.x = -1;
        if (Input.GetKey(l.JoyRight)) outVec.x = 1;

        return outVec;
    }
}