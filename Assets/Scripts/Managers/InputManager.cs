//Edited by Logan, 11-17-2021, to try and combine XInput and Keyboard systems
//Gives priority to controller in any given input type

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XInputDotNetPure;

public enum InputType { Confirm, Back, Up, Down, Left, Right, UpJab, UpHook, DownJab, DownHook }

public class InputManager : MonoBehaviour
{

    private static InputManager inst;
    private Dictionary<InputType, float> p1Holds, p2Holds;

    private static Dictionary<InputType, float> GetHoldDict(bool player1) => player1 ? inst.p1Holds : inst.p2Holds;


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

    //In order to make XInput and keyboard input play nice, both GetKeyDown and GetKey function the same way
    //Sorry Tyler, hope I didn't make a mess of things here
    public static bool GetKeyDown(bool player1, InputType input) {
        bool controllerCurrent = GetGPKey(input, player1 ? p1GPState : p2GPState);
        bool controllerPrev = GetGPKey(input, player1 ? p1PrevGPState : p2PrevGPState);

        bool keyDown = Input.GetKeyDown(GetCabCode(player1, input));
        bool keyCurr = Input.GetKey(GetCabCode(player1, input));
        bool keyUp = Input.GetKeyUp(GetCabCode(player1, input));

        bool keyPrev = keyUp || (!keyDown && keyCurr);

        return (keyDown || controllerCurrent) && !controllerPrev && !keyPrev;
    }

    public static bool GetKey(bool player1, InputType input) {
        return GetGPKey(input, player1 ? p1GPState : p2GPState) 
                || Input.GetKey(GetCabCode(player1, input));
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
        p1GPState = GamePad.GetState(PlayerIndex.One);
        p2GPState = GamePad.GetState(PlayerIndex.Two);
    }

    // Update is called once per frame
    void Update()
    {
        ScanHoldDict(p1Holds, true);
        ScanHoldDict(p2Holds, false);

        p1PrevGPState = p1GPState;
        p1GPState = GamePad.GetState(PlayerIndex.One);
        p2PrevGPState = p2GPState;
        p2GPState = GamePad.GetState(PlayerIndex.Two);

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
