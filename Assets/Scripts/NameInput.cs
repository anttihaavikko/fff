using System;
using UnityEngine;

public class NameInput : MonoBehaviour {
    public Action<string> onUpdate;
    public Action<string> onDone;

    string nameInput = "";
    string playerName = "";
    bool showLine;
    bool asking;

    public void Ask()
    {
        LineToggle();
        asking = true;

        if(PlayerPrefs.HasKey("PlayerName"))
        {
            playerName = PlayerPrefs.GetString("PlayerName").ToUpper();
        }
    }
	
	// Update is called once per frame
	void Update () {

        if (!asking)
            return;

        if (Input.GetKeyUp(KeyCode.Escape))
        {
        }

        foreach (char c in Input.inputString)
        {

            if (c == "\b"[0])
            {

                if (playerName.Length != 0)
                {
                    playerName = playerName.Substring(0, playerName.Length - 1);
                    TriggerUpdate();
                    //AudioManager.Instance.PlayEffectAt(14, Vector3.zero, 0.75f);
                }

            }
            else
            {

                if (c == ';' || c == ',' || c == ':' || c == ' ')
                {
                    return;
                }

                if (c == "\n"[0] || c == "\r"[0])
                {

                    if (playerName != "")
                    {
                        //AudioManager.Instance.PlayEffectAt(0, Vector3.zero, 0.75f);
                        asking = false;
                        onDone?.Invoke(playerName);
                        PlayerPrefs.SetString("PlayerName", playerName);
                    } else {
                        //AudioManager.Instance.PlayEffectAt(1, Vector3.zero, 0.5f);
                    }

                }
                else
                {
                    if (playerName.Length < 9)
                    {
                        playerName += c.ToString().ToUpper();
                        TriggerUpdate();
                        //AudioManager.Instance.PlayEffectAt(15, Vector3.zero, 0.5f);
                    }
                }

            }
        }
    }

    void LineToggle()
    {
        showLine = !showLine;
        TriggerUpdate();
        Invoke("LineToggle", 0.5f);
    }

    void TriggerUpdate()
    {
        nameInput = playerName + (showLine ? "\u25A1" : "\u25A0");
        onUpdate?.Invoke(nameInput);
    }
}
