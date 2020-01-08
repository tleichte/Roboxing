#define USING_CONTROLLER

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public enum InputType { Confirm, Back, Up, Down, Left, Right, UpJab, UpHook, DownJab, DownHook }

public class InputManager : MonoBehaviour
{

    public const bool UsingController =
#if USING_CONTROLLER
    true;
#else
    false;
#endif

    private static InputManager inst;
    private Dictionary<InputType, float> p1Holds, p2Holds;


    private static Dictionary<InputType, float> GetHoldDict(bool player1) => player1 ? inst.p1Holds : inst.p2Holds;

#if USING_CONTROLLER

    private static GamePadState p1GPState, p1PrevGPState, p2PrevGPState, p2GPState;

    private static bool CheckButton(ButtonState s) => s == ButtonState.Pressed;

    private static bool GetGPKey(InputType input, GamePadState state) {
        switch (input) {
            case InputType.Confirm:
                return CheckButton(state.Buttons.A);
            case InputType.Back:
                return CheckButton(state.Buttons.B);
            case InputType.UpJab:
                return CheckButton(state.Buttons.LeftShoulder);
            case InputType.UpHook:
                return CheckButton(state.Buttons.RightShoulder);
            case InputType.DownJab:
                return CheckButton(state.Buttons.X);
            case InputType.DownHook:
                return CheckButton(state.Buttons.B);
            case InputType.Up:
                return CheckButton(state.DPad.Up) || state.ThumbSticks.Left.Y > 0.5f;
            case InputType.Down:
                return CheckButton(state.DPad.Down) || state.ThumbSticks.Left.Y < -0.5f;
            case InputType.Left:
                return CheckButton(state.DPad.Left) || state.ThumbSticks.Left.X < -0.5f;
            case InputType.Right:
                return CheckButton(state.DPad.Right) || state.ThumbSticks.Left.X > 0.5f;
            default:
                return false;
        }
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
        return GetKey(player1, input) && !GetGPKey(input, player1 ? p1PrevGPState : p2PrevGPState);
#else
        return Input.GetKeyDown(GetCabCode(player1, input));
#endif
    }

    public static bool GetKey(bool player1, InputType input) {
#if USING_CONTROLLER
        return GetGPKey(input, player1 ? p1GPState : p2GPState);
#else
        return Input.GetKey(GetCabCode(player1, input));
#endif
    }    

    public static bool GetKeyDelay(bool player1, InputType input, float delay = 0.3f) {
        if (GetKey(player1, input)) {
            if (GetHoldDict(player1).ContainsKey(input)) {
                return false;
            }
            GetHoldDict(player1)[input] = delay;
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
#if USING_CONTROLLER
        p1GPState = GamePad.GetState(PlayerIndex.One);
        p2GPState = GamePad.GetState(PlayerIndex.Two);
#endif
    }

    // Update is called once per frame
    void Update()
    {
        ScanHoldDict(p1Holds, true);
        ScanHoldDict(p2Holds, false);

#if USING_CONTROLLER
        p1PrevGPState = p1GPState;
        p1GPState = GamePad.GetState(PlayerIndex.One);
        p2PrevGPState = p2GPState;
        p2GPState = GamePad.GetState(PlayerIndex.Two);
#endif

    }

    private void ScanHoldDict(Dictionary<InputType, float> dict, bool p1) {
        InputType[] keys = dict.Keys.ToArray();
        foreach (var k in keys) {
            dict[k] -= Time.deltaTime;
            if (dict[k] <= 0 || !GetKey(p1, k))
                dict.Remove(k);
        }
    }
}
