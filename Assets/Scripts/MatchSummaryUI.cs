using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MatchSummaryUI : MonoBehaviour {

	public Text congratsText;

	// Use this for initialization
	void Start () {
		if (MatchSummaryManager.winner != PlayerID.None)
			congratsText.text = ProfileManager.instance.GetProfile(MatchSummaryManager.winner).Name + " Wins!";
		else
			congratsText.text = "Tied!";
	}
}
