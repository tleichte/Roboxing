using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownGameWire : MonoBehaviour
{
    public float WireWidth;

    public int Position { get; set; }

    private int target;
    public bool IsAligned => Position == target;
    public bool Confirmed {
        set {
            if (value) {
                WireImage.color = Color.green;
                LeftArrow.SetActive(false);
                RightArrow.SetActive(false);
                PressButton.SetActive(false);
                active = false;
            }
        }
    }

    private bool active;

    public void Initialize(int correctNum, int maxSpaces) {
        target = correctNum;
        do {
            Position = Random.Range(-maxSpaces, maxSpaces);
        }
        while (Position == correctNum);
        LeftArrow.SetActive(false);
        RightArrow.SetActive(false);
        PressButton.SetActive(false);
        active = false;
    }

    public RectTransform MovableRT;
    public Image WireImage;

    public GameObject PressButton;
    public GameObject LeftArrow;
    public GameObject RightArrow;

    // Update is called once per frame
    void Update()
    {
        if (active) PressButton.SetActive(IsAligned);
        MovableRT.anchoredPosition = Vector2.Lerp(Vector2.right * WireWidth * Position, MovableRT.anchoredPosition, 0.2f);
    }

    public void SetActive() {
        active = true;
        LeftArrow.SetActive(true);
        RightArrow.SetActive(true);
    }


}
