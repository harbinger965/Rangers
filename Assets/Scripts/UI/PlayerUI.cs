using UnityEngine;
using System.Collections;
using Assets.Scripts.Player;
using Assets.Scripts.Data;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour 
{

	private Image healthBar, strengthBar;
	private Controller playerRef;

	// Use this for initialization
	void Start () 
	{
		playerRef = transform.root.GetComponent<Controller>();
		healthBar = transform.FindChild("HealthPanel").FindChild("Overlay").GetComponent<Image>();
		strengthBar = transform.FindChild("StrengthPanel").FindChild("Overlay").GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		float health = playerRef.LifeComponent.HealthPercentage;
		float strength = playerRef.ArcheryComponent.StrengthPercentage;

		healthBar.fillAmount = health;
		strengthBar.rectTransform.localScale = new Vector3(-strength, strength, strength);

		if(playerRef.ArcheryComponent.UpperBodyFacingRight && !Mathf.Approximately(transform.localEulerAngles.y,270f))
		{
			GetComponent<RectTransform>().localRotation = Quaternion.RotateTowards(GetComponent<RectTransform>().localRotation,
				Quaternion.Euler(new Vector3(0f,270f,0f)), Time.deltaTime*360f);
		} 
		else if(!playerRef.ArcheryComponent.UpperBodyFacingRight && !Mathf.Approximately(transform.localEulerAngles.y,90f)) 
		{
			GetComponent<RectTransform>().localRotation = Quaternion.RotateTowards(GetComponent<RectTransform>().localRotation,
				Quaternion.Euler(new Vector3(0f,90f,0f)), Time.deltaTime*360f);
		}
	}
}
