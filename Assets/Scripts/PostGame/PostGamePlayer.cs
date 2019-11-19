using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGamePlayer : MonoBehaviour
{
    public bool Player1;

    public PostGameStats Stats;

    // Start is called before the first frame update
    void Start()
    {
        Stats.Initialize(Player1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
