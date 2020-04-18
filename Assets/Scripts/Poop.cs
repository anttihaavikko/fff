using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poop : MonoBehaviour
{
    private bool willBeRemoved;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!willBeRemoved)
        {
            Invoke("Fade", 2.5f);
            Invoke("Remove", 3f);
        }

        willBeRemoved = true;
    }

    void Fade()
    {
        Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.4f, 0, TweenEasings.QuadraticEaseOut);
    }

    void Remove()
    {
        Destroy(gameObject);
    }
}
