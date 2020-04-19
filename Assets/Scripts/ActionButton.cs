using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public Image icon, border, fill;
    public TMPro.TMP_Text action, desc;

    public void HoverIn()
    {
        Tweener.Instance.ScaleTo(transform, Vector3.one * 1.05f, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        border.color = Color.yellow;
    }

    public void HoverOut()
    {
        Tweener.Instance.ScaleTo(transform, Vector3.one, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        border.color = Color.white;
    }

    public void Click()
    {
    }
}
