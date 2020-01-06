using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGamePlayer : MonoBehaviour {
    public bool Player1;

    public PostGameStats Stats;

    public SwooshAnimator SkipSwoosh;

    private bool ready;

    public PostGame PostGame;

    // Start is called before the first frame update
    void Start()
    {
        SkipSwoosh.Out();
        Stats.Initialize(Player1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready && InputManager.GetKeyDown(Player1, InputType.Confirm)) {
            PostGame.PlayerReady();
            ready = true;
            SkipSwoosh.In();
            AudioManager.Inst.PlayOneShot("PostGame_Skip");
        }
        if (ready && InputManager.GetKeyDown(Player1, InputType.Back) && !PostGame.Returning) {
            PostGame.PlayerUnready();
            ready = false;
            SkipSwoosh.Out();
            AudioManager.Inst.PlayOneShot("PostGame_CancelSkip");
        }
    }
}
