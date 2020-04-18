using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public Flock flock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var diff = flock.transform.position - transform.position;
        var angle = Mathf.Atan2(diff.y, diff.x) / Mathf.PI * 180f - 90;
        var speed = Mathf.Max(Time.deltaTime * 2f, Time.deltaTime * 1f * diff.magnitude);
        transform.position = Vector3.MoveTowards(transform.position, flock.transform.position, speed);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
