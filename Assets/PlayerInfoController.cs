using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour 
{

	private float startingAnimationSpeed = 2f;
	private float startingAnimGoalValue;

	private GameObject BGLine, PlayerInfoBlocksContainer;

	private GameObject PlayerInfoBlock1, PlayerInfoBlock2, PlayerInfoBlock3, PlayerInfoBlock4;

	// Use this for initialization
	void Start () 
	{

		BGLine = transform.FindChild("BGLine").gameObject;
		PlayerInfoBlocksContainer = transform.FindChild("PlayerInfoBlocksContainer").gameObject;
		PlayerInfoBlock1 = PlayerInfoBlocksContainer.transform.GetChild(0).gameObject;
	}
	
	// Update is called once per frame
	void Update () 
	{
		startingAnimGoalValue = 150*ControllerManager.instance.NumPlayers;
		if(BGLine.GetComponent<RectTransform>().sizeDelta.x < startingAnimGoalValue) 
		{
			BGLine.GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(
				BGLine.GetComponent<RectTransform>().sizeDelta,
				new Vector2(startingAnimGoalValue,BGLine.GetComponent<RectTransform>().sizeDelta.y),
				startingAnimationSpeed*Time.deltaTime*(startingAnimGoalValue-BGLine.GetComponent<RectTransform>().sizeDelta.y));
			PlayerInfoBlocksContainer.GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(
				PlayerInfoBlocksContainer.GetComponent<RectTransform>().sizeDelta,
				new Vector2(startingAnimGoalValue,PlayerInfoBlocksContainer.GetComponent<RectTransform>().sizeDelta.y),
				startingAnimationSpeed*Time.deltaTime*(startingAnimGoalValue-BGLine.GetComponent<RectTransform>().sizeDelta.y));
		}

		if(ControllerManager.instance.NumPlayers > 1 && PlayerInfoBlock2 == null) 
		{
			PlayerInfoBlock2 = (GameObject)(GameObject.Instantiate(PlayerInfoBlock1,Vector3.zero,Quaternion.identity));
			PlayerInfoBlock2.GetComponent<RectTransform>().SetParent(PlayerInfoBlock1.transform.parent,false);
			PlayerInfoBlock2.transform.FindChild("PlayerIndicator").GetChild(0).GetComponent<Image>().color = Color.black;
			PlayerInfoBlock2.transform.FindChild("PlayerIndicator").GetChild(1).GetComponent<Image>().color = Color.white;
			PlayerInfoBlock2.GetComponent<RectTransform>().anchoredPosition = new Vector2(PlayerInfoBlock1.GetComponent<RectTransform>().sizeDelta.x/2f,0f);
			PlayerInfoBlock1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-PlayerInfoBlock1.GetComponent<RectTransform>().sizeDelta.x/2f,0f);
		}
		if(ControllerManager.instance.NumPlayers > 2 && PlayerInfoBlock3 == null) 
		{
			PlayerInfoBlock3 = (GameObject)(GameObject.Instantiate(PlayerInfoBlock1,Vector3.zero,Quaternion.identity));
			PlayerInfoBlock3.GetComponent<RectTransform>().SetParent(PlayerInfoBlock1.transform.parent,false);
			PlayerInfoBlock3.transform.FindChild("PlayerIndicator").GetChild(0).GetComponent<Image>().color = Color.black;
			PlayerInfoBlock3.transform.FindChild("PlayerIndicator").GetChild(2).GetComponent<Image>().color = Color.white;
			PlayerInfoBlock3.GetComponent<RectTransform>().anchoredPosition = new Vector2(PlayerInfoBlock1.GetComponent<RectTransform>().sizeDelta.x,0f);
			PlayerInfoBlock2.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f,0f);
			PlayerInfoBlock1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-PlayerInfoBlock1.GetComponent<RectTransform>().sizeDelta.x,0f);
		}

	}
}
