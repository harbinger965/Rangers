using UnityEngine;
using System.Collections;

public class CameraFlashingScript : MonoBehaviour
{

	public float flashMin, flashMax;
	public int numFlash;

	private int actualNumFlash;
	private float flashTimer;

	// Use this for initialization
	void Start () 
	{
		actualNumFlash = ((int)(Random.value*numFlash));
	}
	
	// Update is called once per frame
	void Update ()
	{
		flashTimer -= Time.deltaTime;
//		Debug.Log();
		if(flashTimer <= 0) {
			for(int i = 0; i < actualNumFlash; i++) 
			{
				transform.GetChild((int)(Random.value*transform.childCount)).GetComponent<LensFlare>().brightness = 8f;
			}
			actualNumFlash = ((int)(Random.value*numFlash));
			flashTimer = Random.Range(flashMin, flashMax);
		}
		for(int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild(i).GetComponent<LensFlare>().brightness -= Time.deltaTime*8f;
		}
	}
}
