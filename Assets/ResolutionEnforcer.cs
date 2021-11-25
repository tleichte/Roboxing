using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionEnforcer : MonoBehaviour
{
    private const string InitialResolution = "InitialResolution";

    private const int FULLSCREEN = 1;
    private const int WINDOWED = 2;
    
    private void Start()
    {
        switch (PlayerPrefs.GetInt(InitialResolution, 0))
        {
            case FULLSCREEN:
                Set(true);
                break;
            case WINDOWED:
                Set(false);
                break;
            default:
                Set(true);
                break;
        }
    }

    private const float targetAspect = 16f / 9f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Set(!Screen.fullScreen);
        }
    }

    public static void Set(bool fullscreen)
    {
        if (fullscreen)
        {
            float height = Display.main.systemHeight;
            float width = Display.main.systemWidth;

            // Too large width
            if (width / height > targetAspect)
            {
                width = height * targetAspect;
            }
            else
            {
                height = width / targetAspect;
            }

            Screen.SetResolution(Mathf.RoundToInt(width), Mathf.RoundToInt(height), true, 60);
            PlayerPrefs.SetInt(InitialResolution, FULLSCREEN);
        }
        else
        {
            Screen.SetResolution(1280, 720, false, 60);
            PlayerPrefs.SetInt(InitialResolution, WINDOWED);
        }
    }
}
