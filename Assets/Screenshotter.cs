using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screenshotter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.Return)) {
            ScreenCapture.CaptureScreenshot("Screenshot.png");
            Debug.Log("Screenshotted!");
        }
    }
}
