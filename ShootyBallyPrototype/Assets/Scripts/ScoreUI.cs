using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreUI : MonoBehaviour {

    public Text playerOneText;
    public Text playerTwoText;

    public PlayerInfo playerOne;
    public PlayerInfo playerTwo;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        UpdateText(playerOneText, playerOne);
        UpdateText(playerTwoText, playerTwo);
    }

    private void UpdateText(Text t, PlayerInfo info)
    {
        if (t != null)
        {
            t.text = info.score.ToString();
            if(info.turret != null)
            {
                t.color = info.turret.playerColor;
            }
        }
    }
}
