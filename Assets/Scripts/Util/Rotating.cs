using UnityEngine;
using System.Collections;

public class Rotating : MonoBehaviour 
{

	public enum Axis 
	{
		X = 0,
		Y = 1,
		Z = 2,
	}

	public float speed = 5f;
	public Axis rotationAxis = Axis.X;
	public bool worldSpace = true;
	public float pause = 0f;

	private float pauseTimer = 0f;
	private bool isPaused = false;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(pause > 0f) 
		{
			pauseTimer -= Time.deltaTime;
			if(pauseTimer <= 0 && isPaused) 
			{
				pauseTimer = pause;
				isPaused = !isPaused;
			} 
			else if(pauseTimer <= 0 && !isPaused) 
			{
				pauseTimer = pause;
				isPaused = !isPaused;
			}
		}
		if(!isPaused) 
		{
			if(rotationAxis == Axis.X) 
			{
				if(worldSpace) 
				{
					transform.Rotate(new Vector3(speed, 0f, 0f), Space.World);
				} 
				else 
				{
					transform.Rotate(new Vector3(speed, 0f, 0f), Space.Self);
				}
			} 
			else if(rotationAxis == Axis.Y)
			{
				if(worldSpace)
				{
					transform.Rotate(new Vector3(0f, speed, 0f), Space.World);
				}
				else
				{
					transform.Rotate(new Vector3(0f, speed, 0f), Space.Self);
				}
			} 
			else if(rotationAxis == Axis.Z) 
			{
				if(worldSpace)
				{
					transform.Rotate(new Vector3(0f, 0f, speed), Space.World);
				} 
				else 
				{
					transform.Rotate(new Vector3(0f, 0f, speed), Space.Self);
				}
			}
		}
	}
}
