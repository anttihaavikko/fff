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
    public TMPro.TMP_Text scoreText, multiText, scoreAddText, helpText, nameText;
    public Transform pickup;
    public Slime monsterPrefab;
    public int eatLimit = 25;
    public NameInput nameInput;
    public GameButton sendbutton, againButton, menuButton;
    public EffectCamera effectCamera;
    public GameObject cursor;

    private Camera cam;
    private List<Bird> birds;
    private List<Vector2> collisions;
    private int score, multi;
    private float shownScore;
    private bool warnedMonster, toldBoost, hasBoosted;
    private int eats;
    private int monsterCount = 1;
    private bool canRestart;
    private bool dead;

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

        UseCursor(true);
    }

    void UseCursor(bool state)
    {
        cursor.SetActive(state);
        Cursor.visible = !state;
    }

    void ShowIntro()
    {
        ShowHelp("POINT WITH <color=#FF9F1C>MOUSE</color>\nTO <color=#FF9F1C>MOVE</color> AROUND", 4f);
    }

    void ShowEatHelp()
    {
        ShowHelp("FIND <color=#FF9F1C>FOOD</color>\nTO REPRODUCE", 4f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mp = Input.mousePosition;
        mp.z = 10f;
        Vector3 mouseInWorld = cam.ScreenToWorldPoint(mp);

        transform.position = mouseInWorld;

        if((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !dead)
        {
            UseCursor(true);

            hasBoosted = true;

            var boostCount = birds.Count(b => b.CanBoost());

            effectCamera.BaseEffect(Mathf.Min(boostCount * 0.03f, 0.5f));

            birds.ForEach(b => b.Boost());

            if (toldBoost)
                HideHelp();
        }

        if (Input.GetKeyDown(KeyCode.R) && Application.isEditor)
        {
            SceneManager.LoadScene("Main");
        }

        if(canRestart)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                canRestart = false;
                SceneChanger.Instance.ChangeScene("Start");
                return;
            }

            //if (Input.anyKeyDown)
            //{
            //    canRestart = false;
            //    SceneChanger.Instance.ChangeScene("Main");
            //    return;
            //}

            ShowHelp("<size=25>SCORE UPLOADED!</size>\n<size=15>" + ScoreManager.Instance.GetRank() + "</size>", 0, true);
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
            ShowHelp("WATCH OUT FOR\nTHE <color=#FF9F1C>BIG BADDIE</color>", 3f);
            monster.gameObject.SetActive(true);
            warnedMonster = true;
        }

        if (score > 250 && !toldBoost && !hasBoosted)
        {
            ShowHelp("YOU CAN ALSO <color=#FF9F1C>BOOST</color>\nWITH <color=#FF9F1C>MOUSE BUTTON</color>", 3f);
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
        bird.cam = effectCamera;
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

        CheckForEnd();
    }

    void CheckForEnd()
    {
        CancelInvoke("CheckForEnd");
        CancelInvoke("GameOver");

        if (birds.Count == 0)
        {
            Invoke("GameOver", 1f);
            return;
        }

        Invoke("CheckForEnd", 2f);
    }

    public void Send()
    {
        nameInput.MarkDoneAndGetName();
        sendbutton.appearer.Hide();
    }

    void GameOver()
    {
        UseCursor(false);

        dead = true;

        CancelInvoke("CheckForEnd");
        UpdateInputs("");

        ShowHelp("<size=45>GAME OVER</size>\n<size=15>FINAL SCORE</size>\n<size=30>" + FormatScore(score) + "</size>\n<size=20>PLEASE ENTER NAME...</size>\n<size=60> </size>");

        nameInput.Ask();
        nameInput.onUpdate += UpdateInputs;
        nameInput.onDone += SubmitScore;

        Tweener.Instance.ScaleTo(nameText.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);
        
    }

    void UpdateInputs(string plrName)
    {
        nameText.text = plrName;

        if(plrName.Length > 1)
        {
            sendbutton.appearer.Show();
        }
        else
        {
            sendbutton.appearer.Hide();
        }
    }

    void SubmitScore(string plrName)
    {
        sendbutton.appearer.Hide();

        Tweener.Instance.ScaleTo(nameText.transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);
        ShowHelp("<size=25>UPLOADING SCORE...\n<size=15>PLEASE WAIT</size></size>");

        ScoreManager.Instance.onUploaded += Uploaded;
        ScoreManager.Instance.SubmitScore(plrName, score, eats);
    }

    void Uploaded()
    {
        if (canRestart)
            return;

        againButton.appearer.Show();
        menuButton.appearer.Show();

        ScoreManager.Instance.FindPlayerRank();

        ShowHelp("<size=25>SCORE UPLOADED!</size>\n<size=15>" + ScoreManager.Instance.GetRank() + "</size>");
        canRestart = true;
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

    public static string FormatScore(long score)
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

    void ShowHelp(string help, float hideAfter = 0f, bool silent = false)
    {
        helpText.text = help;
        Tweener.Instance.ScaleTo(helpText.transform, Vector3.one, 0.3f, 0f, TweenEasings.BounceEaseOut);

        AudioManager.Instance.Highpass(true);

        if (hideAfter > 0)
        {
            CancelInvoke("HideHelp");
            Invoke("HideHelp", hideAfter);
        }

        if (!silent)
        {
            AudioManager.Instance.PlayEffectAt(23, Vector3.zero, 1.223f);
            AudioManager.Instance.PlayEffectAt(26, Vector3.zero, 1.296f);
            AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 1.158f);
        }
    }

    void HideHelp()
    {
        AudioManager.Instance.Highpass(false);

        Tweener.Instance.ScaleTo(helpText.transform, Vector3.zero, 0.2f, 0f, TweenEasings.QuadraticEaseOut);

        AudioManager.Instance.PlayEffectAt(23, Vector3.zero, 1.223f);
        AudioManager.Instance.PlayEffectAt(26, Vector3.zero, 1.296f);
        AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 1.158f);
    }
}
