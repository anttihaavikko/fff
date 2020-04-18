using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimSpeed : MonoBehaviour
{
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim.speed = Random.Range(0.9f, 1.1f);
    }
}
