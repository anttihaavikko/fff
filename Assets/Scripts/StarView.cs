using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarView : MonoBehaviour
{
    public TMPro.TMP_Text names, scores;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.Instance.LoadLeaderBoards(0);
    }

    // Update is called once per frame
    void Update()
    {
        names.text = ScoreManager.Instance.leaderBoardPositionsString;
        scores.text = ScoreManager.Instance.leaderBoardScoresString;

        if(Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync("Main");
        }
    }
}
