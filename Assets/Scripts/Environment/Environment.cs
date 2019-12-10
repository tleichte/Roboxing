using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{

    public Transform player1;
    public Transform player2;

    public Transform[] Lights;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = (player1.position + player2.position) / 2;


        foreach(var light in Lights) {
            if (!light.gameObject.activeSelf) continue;
            Vector3 towards = targetPos - light.position;
            Quaternion target = Quaternion.FromToRotation(Vector2.down, (Vector2)towards);
            light.rotation = Quaternion.Lerp(light.rotation, target, 0.2f);
        }

    }
}
