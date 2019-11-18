using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats {

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
    public static PlayerData P1Data = new PlayerData("P1L", 0);
    public static PlayerData P2Data = new PlayerData("P2R", 1);

    public static PlayerStats P1Stats;
    public static PlayerStats P2Stats;

    public static void SetData(bool player1, PlayerData data) {
        if (player1)
            P1Data = data;
        else
            P2Data = data;
    }
}
