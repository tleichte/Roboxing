using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exiter : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) Application.Quit();      
    }
}
