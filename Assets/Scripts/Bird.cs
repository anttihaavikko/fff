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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Pickup")
        {
            var t = collision.gameObject.transform;
            flock.AddBird(t.position);
            t.position = new Vector3(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f), 0);
        }
    }
}
