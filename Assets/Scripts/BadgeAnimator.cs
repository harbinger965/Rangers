using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BadgeAnimator : MonoBehaviour {

	public Sprite badge1_1,badge1_2,badge1_3,badge2_1,badge2_2,badge2_3,badge3_1,badge3_2,badge3_3;
	public PlayerID id;

	private bool flipped;
	private float startTimer;

	// Use this for initialization
	void Start () {
		if(MatchSummaryManager.playerInfo.ContainsKey(id)) {
			switch(MatchSummaryManager.playerInfo[id]) {
				case 0:
					transform.GetChild(0).GetComponent<Image>().sprite = badge1_3;
					transform.GetChild(1).GetComponent<Image>().sprite = badge1_2;
					transform.GetChild(2).GetComponent<Image>().sprite = badge1_1;
					break;
				case 1:
					transform.GetChild(0).GetComponent<Image>().sprite = badge2_3;
					transform.GetChild(1).GetComponent<Image>().sprite = badge2_2;
					transform.GetChild(2).GetComponent<Image>().sprite = badge2_1;
					transform.localScale = Vector3.one*0.75f;
					break;
				default:
					transform.GetChild(0).GetComponent<Image>().sprite = badge3_3;
					transform.GetChild(1).GetComponent<Image>().sprite = badge3_2;
					transform.GetChild(2).GetComponent<Image>().sprite = badge3_1;
					transform.localScale = Vector3.one*0.5f;
					break;
			}

			transform.GetChild(0).localPosition = new Vector3(0,0,-3f);
			transform.GetChild(1).localPosition = new Vector3(0,0,-3f);
			transform.GetChild(2).localPosition = new Vector3(0,0,-3f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(startTimer > 4f) {
			if(transform.eulerAngles.y < 180)
				transform.Rotate(Vector3.up*Mathf.Abs((transform.eulerAngles.y+2f)*Time.deltaTime));
			else
				transform.Rotate(Vector3.up*Mathf.Abs(((360-transform.eulerAngles.y)+2f)*Time.deltaTime));
			
			if(transform.eulerAngles.y > 90f && transform.eulerAngles.y < 270f && !flipped) {
				Sprite temp = transform.GetChild(0).GetComponent<Image>().sprite;
				transform.GetChild(0).GetComponent<Image>().sprite = transform.GetChild(2).GetComponent<Image>().sprite;
				transform.GetChild(2).GetComponent<Image>().sprite = temp;
				
				transform.GetChild(0).localPosition = new Vector3(0,0,-0.2f);
				transform.GetChild(2).localPosition = new Vector3(0,0,0);
				flipped = true;
			} else if((transform.eulerAngles.y < 90f || transform.eulerAngles.y > 270f) && flipped) {
				Sprite temp = transform.GetChild(0).GetComponent<Image>().sprite;
				transform.GetChild(0).GetComponent<Image>().sprite = transform.GetChild(2).GetComponent<Image>().sprite;
				transform.GetChild(2).GetComponent<Image>().sprite = temp;
				
				transform.GetChild(0).localPosition = new Vector3(0,0,0);
				transform.GetChild(2).localPosition = new Vector3(0,0,-0.2f);
				flipped = false;
			}
		} else {
			startTimer += Time.deltaTime;
			transform.GetChild(0).localPosition = Vector3.MoveTowards(transform.GetChild(0).localPosition, new Vector3(0,0,0),Time.deltaTime*4f);
			transform.GetChild(1).localPosition = Vector3.MoveTowards(transform.GetChild(1).localPosition, new Vector3(0,0,-0.1f),Time.deltaTime*3f);
			transform.GetChild(2).localPosition = Vector3.MoveTowards(transform.GetChild(2).localPosition, new Vector3(0,0,-0.2f),Time.deltaTime*2f);
		}

	}
}
