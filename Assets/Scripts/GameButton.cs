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
    public bool hidesOnClick = true;

    private bool done;
    private Vector3 size;
    private bool hovered;

    private void Start()
    {
        size = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (done) return;

        if(hidesOnClick)
        {
            appearer.Hide();
            done = true;
        }

        if (!string.IsNullOrEmpty(activatesScene))
            SceneChanger.Instance.ChangeScene(activatesScene);

        if (action != null) action.Invoke();

        //Invoke("RemoveHover", 0.25f);

        AudioManager.Instance.Lowpass(false);
        AudioManager.Instance.Highpass(false);

        AudioManager.Instance.PlayEffectAt(2, Vector3.zero, 0.713f);
        AudioManager.Instance.PlayEffectAt(1, Vector3.zero, 0.081f);
        AudioManager.Instance.PlayEffectAt(15, Vector3.zero, 1f);
        AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 0.251f);
        AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 0.332f);

    }

    void RemoveHover()
    {
        hovered = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (done) return;

        hovered = true;

        if (Tweener.Instance)
        {
            Tweener.Instance.ScaleTo(transform, size, 0.2f, 0f, TweenEasings.BounceEaseOut);
            Tweener.Instance.ScaleTo(text.transform, Vector3.one * 0.9f, 0.3f, 0f, TweenEasings.BounceEaseOut);

            bg.color = hoverColor;
            text.color = hoverColor;
        }

        AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 0.267f);
        AudioManager.Instance.PlayEffectAt(19, Vector3.zero, 0.534f);
        AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.785f);

        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (done) return;

        hovered = false;

        if (Tweener.Instance)
        {
            Tweener.Instance.ScaleTo(transform, size * 0.9f, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
            Tweener.Instance.ScaleTo(text.transform, Vector3.one, 0.1f, 0f, TweenEasings.QuadraticEaseOut);

            bg.color = normalColor;
            text.color = normalColor;
        }

        transform.rotation = Quaternion.Euler(Vector3.zero);

        AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 0.267f);
        AudioManager.Instance.PlayEffectAt(19, Vector3.zero, 0.534f);
        AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.785f);
    }

    public bool IsHovered()
    {
        return hovered;
    }
}
