using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    public CanvasGroup Group;

    [Header("In")]
    public float InTime;
    public AnimationCurve InCurve;

    [Header("Out")]
    public float OutTime;
    public AnimationCurve OutCurve;


    private float totalTime;

    // Start is called before the first frame update
    void Start()
    {
        totalTime = InTime + OutTime;
    }

    // Update is called once per frame
    void Update()
    {
        float modTime = Time.time % totalTime;

        float a; 
        // If in out-time
        if (modTime > InTime) {
            modTime -= InTime; // Set inwards
            a = OutCurve.Evaluate(modTime / OutTime);
        }
        // If in in-time
        else {
            a = InCurve.Evaluate(modTime / InTime);
        }

        Group.alpha = a;
    }
}
