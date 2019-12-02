using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostGame : MonoBehaviour
{

    public float PostGameDuration;
    public Image TimeImage;

    private float time;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        CurtainTransition.Inst.Open();

        yield return new WaitForSecondsRealtime(PostGameDuration);

        CurtainTransition.Inst.Close(() => {
            SceneManager.LoadScene("MainMenu");
        });
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        TimeImage.fillAmount = time / PostGameDuration;
    }
}
