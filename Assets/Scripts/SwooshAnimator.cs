using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshAnimator : MonoBehaviour
{
    public bool Vertical;

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
        IsIn = StartsIn;
        SetPos(((StartsIn) ? InProps : OutProps).Position);
    }

    private void SetPos(float pos) {
        Vector2 nextVec = rt.anchoredPosition;
        if (Vertical) nextVec.y = pos;
        else nextVec.x = pos;
        rt.anchoredPosition = nextVec;
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
                float nextPos = Mathf.Lerp(prevProps.Position, nextProps.Position, t);
                SetPos(nextPos);
                yield return null;
            }
            SetPos(nextProps.Position);
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
