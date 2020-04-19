using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TMPro.TMP_Text text;
    public Appearer appearer;
    public Image bg;
    public Color normalColor, hoverColor;
    public string activatesScene;
    public UnityEvent action;
    private bool done;
    private Vector3 size;

    private void Start()
    {
        size = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (done) return;

        appearer.Hide();

        if (!string.IsNullOrEmpty(activatesScene))
            SceneChanger.Instance.ChangeScene(activatesScene);

        if (action != null) action.Invoke();

        done = true;

        AudioManager.Instance.Lowpass(false);
        AudioManager.Instance.Highpass(false);

        //AudioManager.Instance.PlayEffectAt(2, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(3, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(5, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(0, transform.position, 1f);
        //AudioManager.Instance.PlayEffectAt(14, transform.position, 1f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (done) return;

        if (Tweener.Instance)
        {
            Tweener.Instance.ScaleTo(transform, size, 0.2f, 0f, TweenEasings.BounceEaseOut);
            Tweener.Instance.ScaleTo(text.transform, Vector3.one * 0.9f, 0.3f, 0f, TweenEasings.BounceEaseOut);

            bg.color = hoverColor;
            text.color = hoverColor;
        }

        //AudioManager.Instance.PlayEffectAt(3, transform.position, 0.495f);
        //AudioManager.Instance.PlayEffectAt(9, transform.position, 1.229f);
        //AudioManager.Instance.PlayEffectAt(14, transform.position, 0.271f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (done) return;

        if (Tweener.Instance)
        {
            Tweener.Instance.ScaleTo(transform, size * 0.9f, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
            Tweener.Instance.ScaleTo(text.transform, Vector3.one, 0.1f, 0f, TweenEasings.QuadraticEaseOut);

            bg.color = normalColor;
            text.color = normalColor;
        }

        //AudioManager.Instance.PlayEffectAt(3, transform.position, 0.495f);
        //AudioManager.Instance.PlayEffectAt(9, transform.position, 1.229f);
        //AudioManager.Instance.PlayEffectAt(17, transform.position, 0.502f);
    }
}
