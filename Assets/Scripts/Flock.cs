using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public Bird birdPrefab;

    private Camera cam;
    private List<Bird> birds;

    // Start is called before the first frame update
    void Start()
    {
        birds = new List<Bird>();
        cam = Camera.main;

        AddBird();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = 10f;
        Vector3 mouseInWorld = cam.ScreenToWorldPoint(mp);

        transform.position = mouseInWorld;

        if(Input.GetKeyDown(KeyCode.A) && Application.isEditor)
        {
            AddBird();
        }
    }

    void AddBird()
    {
        AddBird(transform.position);
    }

    void AddBird(Vector3 pos)
    {
        var bird = Instantiate(birdPrefab);
        bird.flock = this;
        bird.transform.position = pos;
        birds.Add(bird);
    }
}
