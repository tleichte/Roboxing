using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustForScreenShape : MonoBehaviour
{
    public CanvasScaler Scaler;

    // Start is called before the first frame update
    void Awake()
    {
        Scaler.scaleFactor *= Screen.width / 1280f;
    }
}
