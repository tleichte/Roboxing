using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    

    public float fadeTime;
    public AnimationCurve fadeCurve;

    public float VisibleTime;

    private float time;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        Screen.fullScreen = true;
        Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        QualitySettings.vSyncCount = 1;

        AudioManager.Inst.PlayOneShot("BootMusic");

        CanvasGroup.alpha = 0;
        time = 0;
        while (time < fadeTime) {
            yield return null;
            time += Time.deltaTime;

            float t = time / fadeTime;
            CanvasGroup.alpha = fadeCurve.Evaluate(t);
        }

        CanvasGroup.alpha = 1;

        yield return new WaitForSeconds(VisibleTime);

        CurtainTransition.Inst.Close(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    
}
