using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HGDCabinetKeyVisual : MonoBehaviour
{
    public Sprite ButtonUp;
    public Sprite ButtonDown;

    public RectTransform Joystick;
    public int JoyStickIntensity;

    public Image Top1;
    public Image Top2;
    public Image Top3;
    public Image Top4;
    public Image Bottom1;
    public Image Bottom2;
    public Image Bottom3;
    public Image Bottom4;
    public Image Coin;
    public Image PlayerButton;

    public HGDCabPlayer Player;

    // Update is called once per frame
    void Update()
    {
        Joystick.anchoredPosition = HGDCabKeys.JoystickPosition(Player) * JoyStickIntensity;
        CheckKey(HGDCabKeys.Of(Player).Top1, Top1);
        CheckKey(HGDCabKeys.Of(Player).Top2, Top2);
        CheckKey(HGDCabKeys.Of(Player).Top3, Top3);
        CheckKey(HGDCabKeys.Of(Player).Top4, Top4);
        CheckKey(HGDCabKeys.Of(Player).Bottom1, Bottom1);
        CheckKey(HGDCabKeys.Of(Player).Bottom2, Bottom2);
        CheckKey(HGDCabKeys.Of(Player).Bottom3, Bottom3);
        CheckKey(HGDCabKeys.Of(Player).Bottom4, Bottom4);
        CheckKey(HGDCabKeys.Of(Player).Coin, Coin);
        CheckKey(HGDCabKeys.Of(Player).Player, PlayerButton);
    }

    void CheckKey(KeyCode c, Image sr) {
        sr.sprite = Input.GetKey(c) ? ButtonDown : ButtonUp;
    }
}
