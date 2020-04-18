using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoBall : MonoBehaviour {

	private float speed = 0.5f;
	private float offset = 0f;
	private float distance = 1f;
	private bool noNegatives = false;

	private Vector3 originalPosition;

	// Use this for initialization
	void Start () {
		speed = Random.Range (0.5f, 1.5f);
		offset = Random.Range (0f, 10f);
		originalPosition = transform.localPosition;
	}

	// Update is called once per frame
	void Update () {
		float sinVal = Mathf.PerlinNoise (Time.time * speed + offset * Mathf.PI, offset) - distance * 0.5f;
		float xVal = Mathf.PerlinNoise (Time.time * speed + offset * Mathf.PI, offset * 10f) - distance * 0.5f;
        //		sinVal = noNegatives ? Mathf.Abs (sinVal) : sinVal;
        var mod = 1.25f;
		transform.localPosition = originalPosition + Vector3.up * sinVal * distance * mod + Vector3.right * xVal * distance * mod;
	}
}
