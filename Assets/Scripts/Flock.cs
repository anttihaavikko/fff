using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flock : MonoBehaviour
{
    public Bird birdPrefab;
    public LayerMask wallMask;
    public Slime monster;
    public TMPro.TMP_Text scoreText, multiText, scoreAddText;
    public Transform pickup;

    private Camera cam;
    private List<Bird> birds;
    private List<Vector2> collisions;
    private int score, multi;
    private float shownScore;

    // Start is called before the first frame update
    void Start()
    {
        multi = 1;
        birds = new List<Bird>();
        cam = Camera.main;

        var p = IsInside(transform.position) ? transform.position : GetPointInLevel();
        AddBird(p);

        pickup.position = GetPointInLevel();
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

        var scrollSpeed = Mathf.Max(20f, score - shownScore);
        shownScore = Mathf.MoveTowards(shownScore, score, Time.deltaTime * scrollSpeed * 2f);
        scoreText.text = Mathf.RoundToInt(shownScore).ToString();

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

    public void AddScore()
    {
        var amount = birds.Count * multi;
        score += amount;

        scoreAddText.text = "+" + amount;
        multiText.text = "x" + multi;

        Tweener.Instance.ScaleTo(scoreAddText.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);

        multi++;

        // activate monster
        if(score > 10)
        {
            monster.gameObject.SetActive(true);
        }

        CancelInvoke("HideAdditional");
        Invoke("HideAdditional", 1f);
    }

    void HideAdditional()
    {
        Tweener.Instance.ScaleTo(scoreAddText.transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseIn);
    }

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
        multi = 1;
        multiText.text = "x" + multi;
    }

    public List<Bird> GetBirds()
    {
        return birds;
    }

    public Vector3 GetPointInLevel()
    {
        var p = GetRandomPoint();

        while (!IsInside(p))
        {
            p = GetRandomPoint();
        }

        return p;
    }

    private Vector3 GetRandomPoint()
    {
        return new Vector3(Random.Range(-8f, 8f), Random.Range(-4.5f, 4.5f), 0);
    }
}
