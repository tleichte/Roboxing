using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGamePlayer : MonoBehaviour {
    public bool Player1;

    public PostGameStats Stats;

    public SwooshAnimator SkipSwoosh;

    private HGDCabPlayer player;

    public bool WantsSkip { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        SkipSwoosh.Out();
        player = Player1 ? HGDCabPlayer.P1 : HGDCabPlayer.P2;
        Stats.Initialize(Player1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(HGDCabKeys.Of(player).Top1) && !WantsSkip) {
            WantsSkip = true;
            SkipSwoosh.In();
            AudioManager.Inst.PlayOneShot("PostGame_Skip");
        }
        if (Input.GetKeyDown(HGDCabKeys.Of(player).Top2) && WantsSkip) {
            WantsSkip = false;
            SkipSwoosh.Out();
            AudioManager.Inst.PlayOneShot("PostGame_CancelSkip");
        }
    }
}
