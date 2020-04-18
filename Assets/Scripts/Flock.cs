using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Flock : MonoBehaviour
{
    public Bird birdPrefab;
    public LayerMask wallMask;
    public Slime monster;
    public TMPro.TMP_Text scoreText, multiText, scoreAddText, helpText;
    public Transform pickup;
    public Slime monsterPrefab;
    public int eatLimit = 25;

    private Camera cam;
    private List<Bird> birds;
    private List<Vector2> collisions;
    private int score, multi;
    private float shownScore;
    private bool warnedMonster, toldBoost, hasBoosted;
    private int eats;
    private int monsterCount = 1;

    // Start is called before the first frame update
    void Start()
    {
        multi = 1;
        birds = new List<Bird>();
        cam = Camera.main;

        var p = IsInside(transform.position) ? transform.position : GetPointInLevel();
        AddBird(p);

        pickup.position = GetPointInLevel();

        Invoke("ShowIntro", 3f);
        Invoke("ShowEatHelp", 8f);
    }

    void ShowIntro()
    {
        ShowHelp("POINT WITH MOUSE\nTO MOVE AROUND", 4f);
    }

    void ShowEatHelp()
    {
        ShowHelp("FIND FOOD\nTO REPRODUCE", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = 10f;
        Vector3 mouseInWorld = cam.ScreenToWorldPoint(mp);

        transform.position = mouseInWorld;

        if(Input.GetMouseButtonDown(0))
        {
            hasBoosted = true;
            birds.ForEach(b => b.Boost());

            if (toldBoost)
                HideHelp();
        }

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
        scoreText.text = FormatScore(Mathf.RoundToInt(shownScore));

        //collisions = all.Select(p => p.point).ToList();

        //Debug.Log(IsInside(transform.position) ? "INSIDE" : "OUTSIDE");
    }

    public bool IsInside(Vector3 pos)
    {
        var all = Physics2D.RaycastAll(pos, Vector2.up, 100f, wallMask);
        var occupied = Physics2D.OverlapCircle(pos, 1f);
        return all.Length % 2 != 0 && !occupied;
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
        CancelInvoke("ShowIntro");
        CancelInvoke("ShowEatHelp");

        eats++;

        if (score == 0)
            HideHelp();

        var amount = birds.Count * multi * eats * monsterCount * monsterCount;
        score += amount;

        scoreAddText.text = "+" + amount;
        multiText.text = "x" + multi;

        Tweener.Instance.ScaleTo(scoreAddText.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);

        multi++;

        if(score > 10 && !warnedMonster)
        {
            ShowHelp("WATCH OUT FOR\nTHE BIG BADDIE!", 3f);
            monster.gameObject.SetActive(true);
            warnedMonster = true;
        }

        if (score > 250 && !toldBoost && !hasBoosted)
        {
            ShowHelp("YOU CAN ALSO BOOST\nWITH MOUSE BUTTON", 3f);
            toldBoost = true;
        }

        // speed up monster
        if (score > 500)
        {
            monster.speed = 1f;
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
        bird.transform.localScale *= Random.Range(0.9f, 1.1f);
        birds.Add(bird);

        EffectManager.Instance.AddEffect(4, pos);
    }

    public void RemoveBird(Bird bird)
    {
        birds.Remove(bird);
        multi = 1;
        multiText.text = "x" + multi;

        if(birds.Count == 0)
        {
            Invoke("GameOver", 1f);
        }
    }

    void GameOver()
    {
        ShowHelp("<size=45>GAME OVER</size>\n<size=15>FINAL SCORE</size>\n<size=30>" + FormatScore(score) + "</size>");
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

    public static string FormatScore(int score)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = " ";
        return score.ToString("#,0", nfi);
    }

    private Vector3 GetRandomPoint()
    {
        return new Vector3(Random.Range(-18f, 18f), Random.Range(-8f, 8f), 0);
    }

    public void SpawnMonster()
    {
        CancelInvoke("DoSpawn");
        Invoke("DoSpawn", 1f);
    }

    void DoSpawn()
    {
        var newMonster = Instantiate(monsterPrefab);
        newMonster.RandomizeSpawn();
        newMonster.target = transform;
        monsterCount++;
    }

    void ShowHelp(string help, float hideAfter = 0f)
    {
        helpText.text = help;
        Tweener.Instance.ScaleTo(helpText.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);

        if (hideAfter > 0)
        {
            CancelInvoke("HideHelp");
            Invoke("HideHelp", hideAfter);
        }
    }

    void HideHelp()
    {
        Tweener.Instance.ScaleTo(helpText.transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
    }
}
