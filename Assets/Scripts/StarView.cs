using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarView : MonoBehaviour
{
    public TMPro.TMP_Text names, scores;
	public GameButton moreButton;

	private int page;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManager.Instance.LoadLeaderBoards(page);
    }

    // Update is called once per frame
    void Update()
    {
        names.text = ScoreManager.Instance.leaderBoardPositionsString;
        scores.text = ScoreManager.Instance.leaderBoardScoresString;

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }

        if(Input.anyKeyDown && !moreButton.IsHovered())
        {
            AudioManager.Instance.PlayEffectAt(2, Vector3.zero, 0.713f);
            AudioManager.Instance.PlayEffectAt(1, Vector3.zero, 0.081f);
            AudioManager.Instance.PlayEffectAt(15, Vector3.zero, 1f);
            AudioManager.Instance.PlayEffectAt(20, Vector3.zero, 0.251f);
            AudioManager.Instance.PlayEffectAt(24, Vector3.zero, 0.332f);

            SceneChanger.Instance.ChangeScene("Main");
		}
    }

    public void LoadMore()
	{
		page++;

		if (ScoreManager.Instance.endReached)
			page = 0;

		ScoreManager.Instance.LoadLeaderBoards(page);
	}
}
