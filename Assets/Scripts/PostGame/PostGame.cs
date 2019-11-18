using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGame : MonoBehaviour
{

    public CurtainTransition Curtain;

    // Start is called before the first frame update
    void Start()
    {
        Curtain.Open();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
