using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Bird : MonoBehaviour
{
    public Flock flock;
    public Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var diff = flock.transform.position - transform.position;
        var speed = Mathf.Max(Time.deltaTime * 2f, Time.deltaTime * 1f * diff.magnitude);
        //transform.position = Vector3.MoveTowards(transform.position, flock.transform.position, speed);

        var others = flock.GetBirds().Where(b => b != this).ToList();

        Vector2 home = diff.normalized * speed * 100f;
        var separate = Separate(others) * 25;

        body.AddForce(home + separate);

        var angle = Mathf.Atan2(body.velocity.y, body.velocity.x) / Mathf.PI * 180f - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        LimitVelocity();
    }

    void LimitVelocity()
    {
        if(body.velocity.magnitude > 10f)
        {
            body.velocity = body.velocity.normalized * 10f;
        }
    }

    private Vector2 Separate(List<Bird> birds)
    {
        float desiredseparation = 0.6f;
        Vector2 steer = new Vector2(0, 0);
        int count = 0;
        // For every boid in the system, check if it's too close
        foreach(var b in birds)
        {
            float d = Vector2.Distance(transform.position, b.transform.position);
            // If the distance is greater than 0 and less than an arbitrary amount (0 when you are yourself)
            if ((d > 0) && (d < desiredseparation))
            {
                // Calculate vector pointing away from neighbor
                Vector2 diff = transform.position - b.transform.position;
                diff.Normalize();
                diff /= d;        // Weight by distance
                steer += diff;
                count++;            // Keep track of how many
            }
        }
        // Average -- divide by how many
        if (count > 0)
        {
            steer /= count;
        }

        // As long as the vector is greater than 0
        if (steer.magnitude > 0)
        {
            // First two lines of code below could be condensed with new PVector setMag() method
            // Not using this method until Processing.js catches up
            // steer.setMag(maxspeed);

            // Implement Reynolds: Steering = Desired - Velocity
            steer.Normalize();
            //steer.mult(maxspeed);
            //steer.sub(velocity);
            //steer.limit(maxforce);
        }
        return steer;
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
