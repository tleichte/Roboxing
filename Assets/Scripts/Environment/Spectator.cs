using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spectator : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator.Play($"Cheer1", -1, Random.Range(0, 1f));
    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
