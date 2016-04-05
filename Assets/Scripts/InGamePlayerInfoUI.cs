using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Data;

public class InGamePlayerInfoUI : MonoBehaviour {

	public PlayerID id;

	private GameObject tagText, indicatorsContainer, crownIcon;

	private Text livesText, deathsText, killsText;

	// Use this for initialization
	void Start () {
		tagText = transform.FindChild("TagBox").GetChild(0).gameObject;
		indicatorsContainer = transform.FindChild("Player1Indicator").gameObject;

		for (int i = 0; i < indicatorsContainer.transform.childCount; i++) {
			if (i == (int)(id-1))
				indicatorsContainer.transform.GetChild(i).GetComponent<Image>().color = Color.white;
			else
				indicatorsContainer.transform.GetChild(i).GetComponent<Image>().color = Color.black;
		}

		tagText.GetComponent<Text>().text = ProfileManager.instance.GetProfile(id).Name;
		tagText.GetComponent<Text>().color = ProfileManager.instance.GetProfile(id).SecondaryColor;
		tagText.transform.parent.GetComponent<Image>().color = ProfileManager.instance.GetProfile(id).PrimaryColor;

		crownIcon = transform.FindChild("CrownIcon").gameObject;

		if(GameManager.instance.CurrentGameSettings.Type == Assets.Scripts.Util.Enums.GameType.Deathmatch) {
			transform.FindChild("DeathMatchInfo").gameObject.SetActive(true);
			deathsText = transform.FindChild("DeathMatchInfo").FindChild("DeathValue").GetComponent<Text>();
			killsText = transform.FindChild("DeathMatchInfo").FindChild("KillsValue").GetComponent<Text>();
		} else if(GameManager.instance.CurrentGameSettings.Type == Assets.Scripts.Util.Enums.GameType.Stock) {
			transform.FindChild("StockInfo").gameObject.SetActive(true);
			livesText = transform.FindChild("StockInfo").FindChild("LivesValue").GetComponent<Text>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(crownIcon != null) {
			if(livesText) livesText.text = GameManager.instance.GetPlayer(id).LifeComponent.Lives.ToString();
			if(deathsText) deathsText.text = GameManager.instance.GetPlayer(id).LifeComponent.Deaths.ToString();
			if(killsText) killsText.text = GameManager.instance.GetPlayer(id).LifeComponent.kills.ToString();
			if(GameManager.instance.CurrentWinner == id) crownIcon.SetActive(true);
			else crownIcon.SetActive(false);
		}
	}
}
