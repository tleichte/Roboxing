using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshAnimator : MonoBehaviour
{
    [Header("In")]
    public SwooshAnimationProps InProps;

    [Header("Out")]
    public SwooshAnimationProps OutProps;

    private RectTransform rt;
    private Coroutine currRoutine;

    public bool StartsIn;

    public bool IsIn { get; private set; }

    private void Awake() {
        rt = GetComponent<RectTransform>();
        IsIn = !StartsIn;
        if (StartsIn) In();
        else Out();
    }

    private void SetX(float pos) {
        rt.anchoredPosition = new Vector2(pos, rt.anchoredPosition.y);
    }

    private void DoAnim(bool isIn, Action onDone) {

        if (currRoutine != null) StopCoroutine(currRoutine);

        IsIn = isIn;

        SwooshAnimationProps prevProps = isIn ? OutProps : InProps;
        SwooshAnimationProps nextProps = isIn ? InProps : OutProps;

        IEnumerator Animate() {
            // Do curved animation
            for (float time = 0; time < nextProps.Time; time += Time.deltaTime) {
                float t = nextProps.Curve.Evaluate(time / nextProps.Time);
                float nextPos = Mathf.Lerp(prevProps.XPos, nextProps.XPos, t);
                SetX(nextPos);
                yield return null;
            }
            SetX(nextProps.XPos);
            onDone?.Invoke();
        }
        currRoutine = StartCoroutine(Animate());
    }

    public void In(Action onDone = null) {
        if (!IsIn) DoAnim(true, onDone);
    }
    public void Out(Action onDone = null) {
        if (IsIn) DoAnim(false, onDone);
    }
}
