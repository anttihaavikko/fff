using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class LeaderBoard
{
	public LeaderBoardScore[] scores;
}

[System.Serializable]
public class LeaderBoardScore {
	public string name;
	public string score;
	public int level;
	public int position;
	public string pid;
	public string locale;

	public static LeaderBoardScore CreateFromJSON(string jsonString) {
		return JsonUtility.FromJson<LeaderBoardScore>(jsonString);
	}
}

public class ScoreManager : MonoBehaviour {

    public string gameName;
    public Action onUploaded;

    private string playerName = "";
	private int check = 0;
	private long score = 0;
	private int wave = 0;

	private int localRank = -1;

    const string webURL = "https://games.sahaqiel.com/leaderboards/save-score.php?str=";

	private bool enteringName = false;
	private bool writingEnabled = false;
	private bool uploading = false;

	public string leaderBoardString = "";
	public string localScoreString = "";

    public string leaderBoardPositionsString = "";
    public string leaderBoardScoresString = "";

	public bool endReached = false;

    public string personalBestPos, personalBestScore;

	private TouchScreenKeyboard keyboard = null;

    private CertificateHandler certHandler;

    /******/

    private static ScoreManager instance = null;
	public static ScoreManager Instance {
		get { return instance; }
	}

	public void CancelLeaderboards() {
		StopAllCoroutines ();
	}

	public void LoadLeaderBoards(int p) {
        //leaderBoardPositionsString = "LOADING...";
		StartCoroutine(DoLoadLeaderBoards(p));
	}

	public int GetValidatedScore()
    {
        LoadPrefs();

        if((long)check != Secrets.GetVerificationNumber(playerName, score, 7)) {
            score = 0;
            check = (int)Secrets.GetVerificationNumber(playerName, 0, 7);
            SavePrefs();
        }

        return (int)score;
    }

    IEnumerator DoLoadLeaderBoards(int p) {

		FlagManager.Instance.HideAllFlags ();

        var www = UnityWebRequest.Get("https://games.sahaqiel.com/leaderboards/load-scores.php?p=" + p + "&game=" + gameName);
        www.certificateHandler = certHandler;

        yield return www.SendWebRequest();

		if (string.IsNullOrEmpty (www.error)) {

			LeaderBoard lb = JsonUtility.FromJson<LeaderBoard> (www.downloadHandler.text);

			leaderBoardString = "";
            leaderBoardPositionsString = "";
            leaderBoardScoresString = "";

			if (lb.scores.Length > 0) {
				for (int i = 0; i < lb.scores.Length; i++) {
					leaderBoardString += FormatLeaderboardRow (lb.scores [i].position, lb.scores [i].name, long.Parse (lb.scores [i].score), lb.scores [i].pid);
					FlagManager.Instance.SetPositionFlag (i, lb.scores [i].locale);
                    leaderBoardPositionsString += lb.scores[i].position + ". " + lb.scores[i].name + "\n";
                    leaderBoardScoresString += Flock.FormatScore(long.Parse(lb.scores[i].score)) + "\n";
				}
			}

            if(playerName != "") {
                FindPlayerRank();
            }

			endReached = (lb.scores.Length < 9);

		} else {
			leaderBoardString = www.error;
		}
	}

	public void FindPlayerRank() {
		if (score > 0) {
			StartCoroutine (DoFindPlayerRank ());
		}
	}

	IEnumerator DoFindPlayerRank() {
        var url = "https://games.sahaqiel.com/leaderboards/get-rank.php?score=" + score + "&name=" + playerName + "&pid=" + SystemInfo.deviceUniqueIdentifier + "&game=" + gameName;
        //Debug.Log(url);
        var www = UnityWebRequest.Get(url);
        www.certificateHandler = certHandler;

        yield return www.SendWebRequest();

		if (string.IsNullOrEmpty (www.error)) {
			localRank = int.Parse (www.downloadHandler.text);
			localScoreString = (score > 0) ? FormatLeaderboardRow (localRank, playerName, score, "") : "";

            personalBestPos = localRank + ". " + playerName;
            personalBestScore = score.ToString();
		}
	}

	public string FormatLeaderboardRow(int pos, string nam, long sco, string pid) {

		string row = "";

		if (nam == playerName && pos > 0 && pid == SystemInfo.deviceUniqueIdentifier) {
			row += "<color=#FF9F1C>";
		}
		
		row += (pos > 0) ? pos.ToString() : "?";
		row += ".";

		row += nam;

		for (int k = 0; k < 22 - nam.Length; k++) {
			row += ".";
		}

		row += sco.ToString("D10") + "\n";

		if (nam == playerName && pos > 0 && pid == SystemInfo.deviceUniqueIdentifier) {
			row += "</color>";
		}

		return row;
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
			return;
		} else {
			instance = this;
		}

		LoadPrefs ();

        //DontDestroyOnLoad(instance.gameObject);

        certHandler = new CustomCertificateHandler();
    }

	void Update() {
	}

	public void SubmitScore(string name, int scoreSub, int levelSub) {
        check = (int)Secrets.GetVerificationNumber(name, (long)scoreSub, levelSub);
        playerName = name;
        score = (long)scoreSub;
        wave = levelSub;
        SavePrefs();
        //HudManager.Instance.namePromptLabel.text = "UPLOADING YOUR SCORE...";
		StartCoroutine(DoSubmitScore());
    }

	IEnumerator DoSubmitScore() {
		string data = "";

		data += playerName;
		data += "," + SystemInfo.deviceUniqueIdentifier.ToString();
		data += "," + wave;
		data += "," + score;
		data += "," + check;
        data += "," + gameName;

        //Debug.Log(data);

        var www = UnityWebRequest.Get(webURL + data);
        www.certificateHandler = certHandler;

        yield return www.SendWebRequest();

		Invoke ("UploadingDone", 0.75f);
	}

	public void UploadingDone() {
        onUploaded?.Invoke();

        uploading = false;
		//HudManager.Instance.namePromptLabel.text = "SCORE UPLOADED!";
		//HudManager.Instance.namePromptText.text = "PLAY AGAIN?";
		//AudioManager.Instance.PlayEffectAt (AudioManager.BLING, Vector3.zero, 1f);
	}

	private void LoadPrefs() {
		if (PlayerPrefs.HasKey ("PlayerName")) {
			playerName = PlayerPrefs.GetString ("PlayerName");
			playerName = FixPlayerName (playerName);
		}

		if (PlayerPrefs.HasKey ("HiScore")) {
			int oldScore = PlayerPrefs.GetInt ("HiScore");
            score = long.Parse(PlayerPrefs.GetString("HiScore"));
		}

        if (PlayerPrefs.HasKey("HiScore"))
        {
            check = PlayerPrefs.GetInt("CheckNumber");
        }

        //Debug.Log("loaded: " + playerName + ", " + score + ", " + check);
	}

	private void SavePrefs() {
		PlayerPrefs.SetString ("PlayerName", playerName);
		PlayerPrefs.SetString ("HiScore", score.ToString());
        PlayerPrefs.SetInt("CheckNumber", (int)Secrets.GetVerificationNumber(playerName, score, 7));
	}

	private string FixPlayerName(string str) {
		str = str.ToUpper();
		str = str.Replace (";", "");
		str = str.Replace (",", "");
		str = str.Replace (":", "");
		str = str.Replace (" ", "");

		if (str.Length > 10) {
			str = str.Substring(0, 10);
		}

		return str;
	}

	public void ResetScores() {
		PlayerPrefs.DeleteAll ();
		playerName = "";
		score = 0;
	}

    public string GetRank()
	{
		return localRank > 0 ? "YOU RANKED <color=#FF9F1C>#" + localRank + "</color>" : "LOADING RANK...";
	}
}

public class CustomCertificateHandler : CertificateHandler
{
    // Encoded RSAPublicKey
    private static readonly string PUB_KEY = "";


    /// <summary>
    /// Validate the Certificate Against the Amazon public Cert
    /// </summary>
    /// <param name="certificateData">Certifcate to validate</param>
    /// <returns></returns>
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}