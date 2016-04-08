using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Scripts.Data;
using Assets.Scripts.UI;

public class PauseUI : MonoBehaviour {

	public Text pause1,pause2,pause3;

	public bool pauseMenuOpen;


	public Selectable startingButton;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(pauseMenuOpen || GameManager.instance.IsPaused) {
			transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(871,275f), Time.deltaTime*(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*10f+10f));
			if(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y > 120) {
				pause1.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause1.rectTransform.anchoredPosition3D,new Vector3(-1.5f,0,0),Mathf.Abs(pause1.rectTransform.anchoredPosition3D.x)*Time.deltaTime);
				pause2.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause2.rectTransform.anchoredPosition3D,new Vector3(1.5f,0,0),pause2.rectTransform.anchoredPosition3D.x*Time.deltaTime);
				pause3.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause3.rectTransform.anchoredPosition3D,Vector3.zero,pause3.rectTransform.anchoredPosition3D.y*Time.deltaTime);
			}
			Navigator.defaultGameObject = 
		} else {
			transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = Vector2.MoveTowards(transform.GetChild(0).GetComponent<RectTransform>().sizeDelta, new Vector2(871,0f), Time.deltaTime*(650-transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y*2f));
			pause1.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause1.rectTransform.anchoredPosition3D,new Vector3(-500f,0,0),Mathf.Abs(pause1.rectTransform.anchoredPosition3D.x*10f-1f)*Time.deltaTime);
			pause2.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause2.rectTransform.anchoredPosition3D,new Vector3(500f,0,0),(pause2.rectTransform.anchoredPosition3D.x*10f+1f)*Time.deltaTime);
			pause3.rectTransform.anchoredPosition3D = Vector3.MoveTowards(pause3.rectTransform.anchoredPosition3D,new Vector3(0,200f,0),(pause3.rectTransform.anchoredPosition3D.y*10f+1f)*Time.deltaTime);
		}
	}
}
