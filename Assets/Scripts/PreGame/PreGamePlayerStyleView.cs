using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PreGamePlayerStyleView : MonoBehaviour
{

    public Animator animator;

    public SpriteRenderer headImage;
    public SpriteRenderer bodyImage;
    public SpriteRenderer leftArm;
    public SpriteRenderer rightArm;

    public GameObject leftArrow;
    public GameObject rightArrow;

    private PreGamePlayer player;

    public void Initialize(PreGamePlayer player) {
        this.player = player;
        OnStyleChange();
        OnStyleExit(true);
    }

    public void OnStyleEnter() {
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        animator.SetBool("Style", true);
    }
    public void OnStyleExit(bool toName) {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        if (toName) animator.SetBool("Style", false);
    }

    public void OnStyleChange() {
        var style = AssetManager.Inst.PlayerStyles[player.CurrentStyle];
        headImage.sprite = style.HeadIdle;
        bodyImage.sprite = style.BodyIdle;
        leftArm.sprite = style.TPArmIdle;
        rightArm.sprite = style.TPArmIdle;
    }
}
