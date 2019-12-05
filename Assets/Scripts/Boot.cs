using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{
    public CanvasGroup CanvasGroup;
    

    public float fadeTime;
    public AnimationCurve fadeCurve;

    private float time;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        CanvasGroup.alpha = 0;
        time = 0;
        while (time < fadeTime) {
            yield return null;
            time += Time.deltaTime;

            float t = time / fadeTime;
            CanvasGroup.alpha = fadeCurve.Evaluate(t);
        }

        CanvasGroup.alpha = 1;

        yield return new WaitForSeconds(3.5f);

        CurtainTransition.Inst.Close(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    
}
