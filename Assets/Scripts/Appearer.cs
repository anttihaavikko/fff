using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearer : MonoBehaviour
{
	public float appearAfter = -1f;
	public float hideDelay;
	public bool notShownOnWeb;

    private Vector3 size;

    private bool shown;

    // Start is called before the first frame update
    void Start()
    {
        size = transform.localScale;
        transform.localScale = Vector3.zero;

        if (appearAfter >= 0)
			Invoke("Show", appearAfter);
    }

    public void Show()
    {
		if (notShownOnWeb && Application.platform == RuntimePlatform.WebGLPlayer)
			return;

        Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);

        if(!shown)
        {
            AudioManager.Instance.PlayEffectAt(23, transform.position, 1.223f);
            AudioManager.Instance.PlayEffectAt(26, transform.position, 1.296f);
            AudioManager.Instance.PlayEffectAt(24, transform.position, 1.158f);
        }

        shown = true;
    }

    public void Hide()
	{
        Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);

        if(shown)
        {
            AudioManager.Instance.PlayEffectAt(23, transform.position, 1.223f);
            AudioManager.Instance.PlayEffectAt(26, transform.position, 1.296f);
            AudioManager.Instance.PlayEffectAt(24, transform.position, 1.158f);
        }

        shown = false;
    }

    public void HideWithDelay()
	{
		Invoke("Hide", hideDelay);
	}
}
