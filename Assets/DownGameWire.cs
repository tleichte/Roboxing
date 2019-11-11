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
            if (value) WireImage.color = Color.green;
        }
    }

    public void Initialize(int correctNum) {
        target = correctNum;
        do {
            Position = Random.Range(-5, 5);
        }
        while (Position == correctNum);
    }

    public RectTransform MovableRT;
    public Image WireImage;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MovableRT.anchoredPosition = Vector2.Lerp(Vector2.right * WireWidth * Position, MovableRT.anchoredPosition, 0.2f);
    }


}
