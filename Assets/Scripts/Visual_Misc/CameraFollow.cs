using UnityEngine;
using System.Collections;
using Assets.Scripts.Data;

public class CameraFollow : MonoBehaviour 
{

	public float speed;

	private Vector3 offset;
	private int numPlayers;
	private float greatestDistance;

	// Use this for initialization
	void Start ()
	{
		numPlayers = GameManager.instance.AllPlayers.Count;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		Vector3 averagePosition = Vector3.zero;
		for(int i = 0; i < numPlayers; i++) 
		{
			averagePosition += GameManager.instance.AllPlayers[i].transform.position + (Vector3.up*2f);
		}

		averagePosition /= numPlayers;
		greatestDistance = 0;

		for(int i = 0; i < numPlayers; i++) 
		{
			float tempDist = Vector3.Distance(GameManager.instance.AllPlayers[i].transform.position, averagePosition);
			if(tempDist > greatestDistance) 
			{
				greatestDistance = tempDist;
			}
		}
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(averagePosition.x, averagePosition.y, (-1.1f)*(greatestDistance+4)), Time.deltaTime*speed);

	}
}