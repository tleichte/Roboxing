using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class exiter : MonoBehaviour
{
    private float time;

    private static exiter Inst;

    public SwooshAnimator animator;
    public Image circleImage;

    private const float ExitTime = 5;

    void Awake() {
        if (Inst != null) {
            Destroy(gameObject);
            return;
        }

        Inst = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            animator.In();
        }

        if (Input.GetKey(KeyCode.Escape)) {
            time += Time.deltaTime;
            circleImage.fillAmount = time / ExitTime;
            if (time > ExitTime) {
                time = 0;
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
        }
        else {
            time = 0;
            animator.Out();
        }      
    }
}
