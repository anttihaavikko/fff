using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Appearer : MonoBehaviour
{
	public float appearAfter = -1f;
	public float hideDelay;
    private Vector3 size;

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
		//AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
		//AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);

		Tweener.Instance.ScaleTo(transform, size, 0.3f, 0f, TweenEasings.BounceEaseOut);
    }

    public void Hide()
	{
		//AudioManager.Instance.PlayEffectAt(16, Vector3.zero, 0.336f);
		//AudioManager.Instance.PlayEffectAt(17, Vector3.zero, 0.329f);

		Tweener.Instance.ScaleTo(transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
	}

    public void HideWithDelay()
	{
		Invoke("Hide", hideDelay);
	}
}
