using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinders : MonoBehaviour
{
    public Transform left, right;
    public bool startsOpen, openAtStart = true;

    private float duration = 0.3f;
    private bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        isOpen = startsOpen;

        if (startsOpen) return;

        left.transform.localScale = new Vector3(1f, 1f, 1f);
        right.transform.localScale = new Vector3(1f, 1f, 1f);

        if(openAtStart)
            Invoke("Open", 0.5f);
    }

    public void Close()
    {
        if (!isOpen) return;

        Tweener.Instance.ScaleTo(left, Vector3.one, duration, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(right, Vector3.one, duration, 0f, TweenEasings.BounceEaseOut);

        //AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
        //AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);

        Invoke("Clang", duration * 0.6f);

        isOpen = false;
    }

    public void Open()
    {
        Tweener.Instance.ScaleTo(left, new Vector3(0f, 1f, 1f), duration, 0f, TweenEasings.BounceEaseOut);
        Tweener.Instance.ScaleTo(right, new Vector3(0f, 1f, 1f), duration, 0f, TweenEasings.BounceEaseOut);

        //AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
        //AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);

        isOpen = true;
    }

    public float GetDuration()
    {
        return duration;
    }

    void Clang()
    {
        //AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
        //AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);
    }
}
