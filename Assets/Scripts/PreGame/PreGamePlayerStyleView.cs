using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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

    [FormerlySerializedAs("light")]
    public SpriteRenderer lightSprite;
    public SpriteRenderer floorSprite;

    private Color spriteColor;
    private bool focused;

    public void Initialize(PreGamePlayer player) {
        this.player = player;
        OnStyleChange();
        OnStyleExit(true);
        
    }

    public void OnStyleEnter() {
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        animator.SetBool("Style", true);
        focused = true;
    }
    public void OnStyleExit(bool toName) {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        if (toName) {
            focused = false;
            animator.SetBool("Style", false);
        }
    }

    public void OnStyleChange() {
        var style = AssetManager.Inst.PlayerStyles[player.CurrentStyle];
        headImage.sprite = style.HeadIdle;
        bodyImage.sprite = style.BodyIdle;
        leftArm.sprite = style.TPArmIdle;
        rightArm.sprite = style.TPArmIdle;
    }


    public void SetSpriteColor(Color spriteColor) {
        headImage.color = spriteColor;
        bodyImage.color = spriteColor;
        leftArm.color = spriteColor;
        rightArm.color = spriteColor;
        floorSprite.color = spriteColor;
    }

    private void Update() {
        Color targetSpriteColor = (focused) ? Color.white : new Color(0.3f, 0.3f, 0.3f);
        Color targetLightColor = (focused) ? Color.white : new Color(1, 1, 1, 0);

        spriteColor = Color.Lerp(spriteColor, targetSpriteColor, 0.2f);

        SetSpriteColor(spriteColor);

        lightSprite.color = Color.Lerp(lightSprite.color, targetLightColor, 0.2f);
    }

}
