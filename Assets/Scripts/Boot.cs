using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{

    void Start()
    {
        Cursor.visible = false;
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        QualitySettings.vSyncCount = 1;
        
    }


    public void A_StartMusic() {
        AudioManager.Inst.PlayStartingSound(() => {
            CurtainTransition.Inst.Close(() => {
                SceneManager.LoadScene("MainMenu");
            });
        });
    }
}
