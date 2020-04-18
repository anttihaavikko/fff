using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flock : MonoBehaviour
{
    public Bird birdPrefab;
    public LayerMask wallMask;

    private Camera cam;
    private List<Bird> birds;
    private List<Vector2> collisions;

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

        if (Input.GetKeyDown(KeyCode.R) && Application.isEditor)
        {
            SceneManager.LoadScene("Main");
        }

        //collisions = all.Select(p => p.point).ToList();

        //Debug.Log(IsInside(transform.position) ? "INSIDE" : "OUTSIDE");
    }

    public bool IsInside(Vector3 pos)
    {
        var all = Physics2D.RaycastAll(pos, Vector2.up, 100f, wallMask);
        return all.Length % 2 != 0;
    }

    //private void OnDrawGizmos()
    //{
    //    foreach (var a in collisions)
    //    {
    //        Gizmos.DrawSphere(a, 0.2f);
    //    }
    //}

    void AddBird()
    {
        AddBird(transform.position);
    }

    public void AddBird(Vector3 pos)
    {
        var bird = Instantiate(birdPrefab);
        bird.flock = this;
        bird.transform.position = pos;
        birds.Add(bird);
    }

    public void RemoveBird(Bird bird)
    {
        birds.Remove(bird);
    }

    public List<Bird> GetBirds()
    {
        return birds;
    }
}
