using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenuPlayerInfoBlock : MonoBehaviour {

	public PlayerID playerID;

	private Text tagText;
	private Image tagBG;
	private GameObject pressToJoin;
	private GameObject pressToOpen;
	private GameObject nameCreator;
	private Image playerNumIndicator;


	// Use this for initialization
	void Start () {
		tagText = transform.FindChild("TagBox").FindChild("PlayerTag").GetComponent<Text>();
		tagBG = transform.FindChild("TagBox").GetComponent<Image>();
		pressToJoin = transform.FindChild("PressToJoin").gameObject;
		pressToOpen = transform.FindChild("StartToOpen").gameObject;
		nameCreator = transform.FindChild("NameCreator").gameObject;
		playerNumIndicator = transform.FindChild("Player" + (int)playerID + "Indicator").GetChild((int)playerID - 1).GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		if(nameCreator.activeInHierarchy) {
			if(ControllerManager.instance.GetButtonDown(ControllerInputWrapper.Buttons.Start,playerID)) {
				ProfileManager.instance.AddProfile(new ProfileData(tagText.text), playerID);
				HideNameCreator();
				playerNumIndicator.color = Color.white;
			}
		}
	}

	public void PlayerAdded() {
		if(ProfileManager.instance.ProfileExists(playerID)) {
			SetTag(ProfileManager.instance.GetProfile(playerID).Name);
			HidePressToJoinGraphic();
			playerNumIndicator.color = Color.white;
		} else {
			ShowNameCreator();
		}
	}

	public void HidePressToJoinGraphic() {
		pressToJoin.SetActive(false);
		pressToOpen.SetActive(true);
	}

	public void SetTag(string text) {
		tagText.text = text;
	}

	public void ShowNameCreator() {
		nameCreator.SetActive(true);
		pressToJoin.SetActive(false);
		SetTag("");
	}

	public void HideNameCreator() {
		nameCreator.SetActive(false);
		pressToOpen.SetActive(true);
	}
}
