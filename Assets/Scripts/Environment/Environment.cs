using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnvironmentState { Focused, Relaxed }

public class Environment : MonoBehaviour
{
    public Transform player1;
    public Transform player2;

    public Transform[] Lights;

    private EnvironmentState state;

    // Start is called before the first frame update
    void Start()
    {
        state = EnvironmentState.Relaxed;

        GameManager.Inst.OnFightReady += ToFocused;
        GameManager.Inst.OnRoundOver += ToRelaxed;
        GameManager.Inst.OnPlayerDown += ToRelaxed;
    }

    private void OnDestroy() {
        GameManager.Inst.OnFightReady -= ToFocused;
        GameManager.Inst.OnRoundOver -= ToRelaxed;
        GameManager.Inst.OnPlayerDown -= ToRelaxed;
    }

    void ToFocused() {
        state = EnvironmentState.Focused;
    }

    void ToRelaxed() {
        state = EnvironmentState.Relaxed;
    }
    

    // Update is called once per frame
    void Update()
    {


        // Adjust lights

        Vector3 lightTargetPos = (player1.position + player2.position) / 2;

        foreach (var light in Lights) {
            if (!light.gameObject.activeSelf) continue;
            Vector3 towards = lightTargetPos - light.position;
            Quaternion target = Quaternion.FromToRotation(Vector2.down, (Vector2)towards);

            if (state == EnvironmentState.Relaxed)
                target = Quaternion.Lerp(Quaternion.identity, target, 0.7f);

            light.rotation = Quaternion.Lerp(light.rotation, target, 0.2f);
        }
    }
}
