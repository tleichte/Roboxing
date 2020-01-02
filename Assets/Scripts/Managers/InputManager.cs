//#define USING_CONTROLLER

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public enum InputType { Confirm, Back, Up, Down, Left, Right, UpJab, UpHook, DownJab, DownHook }

public class InputManager : MonoBehaviour
{

    private static InputManager inst;
    private Dictionary<InputType, float> p1Holds, p2Holds;


    private static Dictionary<InputType, float> GetDict(bool player1) => player1 ? inst.p1Holds : inst.p2Holds;

#if USING_CONTROLLER

    private bool Dictionary<>

    private bool IsAxis(InputType input) {
        switch (input) {
            case InputType.Up:
            case InputType.Down:
            case InputType.Left:
            case InputType.Right:
                return true;
            default:
                return false;
        }
    }

    private bool GetAxisValue(string axis) {
        float val = Input.GetAxisRaw(axis);

    }
#else
    private static KeyCode GetCabCode(bool player1, InputType input) {
        var layout = player1 ? HGDCabKeys.P1 : HGDCabKeys.P2;
        switch (input) {
            case InputType.Confirm:
            case InputType.UpJab:
                return layout.Top1;

            case InputType.Back:
            case InputType.UpHook:
                return layout.Top2;

            case InputType.Up:
                return layout.JoyUp;

            case InputType.Down:
                return layout.JoyDown;

            case InputType.Left:
                return layout.JoyLeft;

            case InputType.Right:
                return layout.JoyRight;

            case InputType.DownJab:
                return layout.Bottom1;

            case InputType.DownHook:
                return layout.Bottom2;
        }
        return layout.Player;
    }
#endif

    public static bool GetKeyDown(bool player1, InputType input) {
#if USING_CONTROLLER

#else
        return Input.GetKeyDown(GetCabCode(player1, input));
#endif
    }

    public static bool GetKey(bool player1, InputType input) {
#if USING_CONTROLLER

#else
        return Input.GetKey(GetCabCode(player1, input));
#endif
    }

    public static bool GetKeyDelay(bool player1, InputType input, float delay = 0.3f) {
        if (GetKey(player1, input)) {
            if (GetDict(player1).ContainsKey(input)) {
                return false;
            }
            GetDict(player1)[input] = delay;
            return true;
        }
        return false;
    }

    void Awake() {
        if (inst != null) {
            Destroy(gameObject);
            return;
        }
        inst = this;
        DontDestroyOnLoad(gameObject);

        p1Holds = new Dictionary<InputType, float>();
        p2Holds = new Dictionary<InputType, float>();
    }

    // Update is called once per frame
    void Update()
    {
        ScanDict(p1Holds, true);
        ScanDict(p2Holds, false);
    }

    private void ScanDict(Dictionary<InputType, float> dict, bool p1) {
        InputType[] keys = dict.Keys.ToArray();
        foreach (var k in keys) {
            dict[k] -= Time.deltaTime;
            if (dict[k] <= 0 || !GetKey(p1, k))
                dict.Remove(k);
        }
    }
}
