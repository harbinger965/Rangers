using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PlayerID {None, One, Two, Three, Four};

public class ControllerManager  {
   
    public enum ControlType { Xbox, PS3, PS4 };
    public enum OperatingSystem { Win, OSX, Linux };

	public OperatingSystem currentOS;
	public Dictionary<PlayerID, ControllerInputWrapper> playerControls;

	public static ControllerManager instance;

    public ControllerManager()
    {
		setUpPlatform();
		playerControls = new Dictionary<PlayerID, ControllerInputWrapper>();
		if(instance != this) 
		{
			instance = this;
		}
    }

	public int NumPlayers 
	{
		get { return playerControls.Count; }
	}

	public bool AddPlayer(ControllerInputWrapper.Buttons connectCode) {
		KeyboardWrapper kw = new KeyboardWrapper(-1);
		if(!playerControls.ContainsValue(kw) && kw.GetButton(connectCode)) 
		{
			for(int j = 1; j < 5; j++) 
			{
				if(!playerControls.ContainsKey((PlayerID)j)) 
				{
					playerControls.Add((PlayerID)(j), kw);
					Debug.Log((PlayerID)(j) + ": " + kw + " added");
					return true;
				}
			}
		}
		if(playerControls.Count < 4) 
		{
			string[] controllerNames = Input.GetJoystickNames();
			for (int i = 0; i < controllerNames.Length; i++) 
			{
				ControllerInputWrapper ciw = getControllerType(i);
				if(ciw != null && !playerControls.ContainsValue(ciw) && ciw.GetButton(connectCode)) 
				{
					for(int j = 1; j < 5; j++) 
					{
						if(!playerControls.ContainsKey((PlayerID)j)) 
						{
							playerControls.Add((PlayerID)(j), ciw);
							Debug.Log((PlayerID)(j) + ": " + ciw + " added");
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public void AllowPlayerRemoval(ControllerInputWrapper.Buttons removalButton) 
	{
		PlayerID playerToRemove = PlayerID.None;
		foreach(KeyValuePair<PlayerID, ControllerInputWrapper> kvp in ControllerManager.instance.playerControls) 
		{
			if(kvp.Value.GetButton(removalButton)) 
			{
				playerToRemove = kvp.Key;
				break;
			}
		}
		if(playerToRemove != PlayerID.None) {
//			Debug.Log(playerToRemove + " removed");
			playerControls.Remove(playerToRemove);
		}
	}

    public void setUpControls()
    {
        setUpPlatform();
        string[] controllerNames = Input.GetJoystickNames();
        //Debug.Log("Controllers connected: " + controllerNames.Length);
        for (int i = 0; i < controllerNames.Length; i++)
        {
//            playerControls[i] = getControllerType(i + 1);
        }
    }

    public ControllerInputWrapper getControllerType(int joyNum)
    {
        //Debug.Log(joyNum);
        string[] controllerNames = Input.GetJoystickNames();
        if (joyNum < 0 || joyNum > controllerNames.Length)
        {
            return null;
        }
//        joyNum--;
        string name = controllerNames[joyNum];

        //Debug.Log("Controllers connected: " + controllerNames.Length);

        if (name.Contains("Wireless"))
        {
			return new PS4ControllerWrapper(joyNum);
        }
        else if (name.Contains("Logitech"))
        {
			return new LogitechControllerWrapper(joyNum);
        }
        else
        {
			return new XboxControllerWrapper(joyNum);
        }
           

    }

	private void setUpPlatform()
    {
		//Debug.Log ("platform: " + Application.platform);
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXWebPlayer 
			|| Application.platform == RuntimePlatform.OSXEditor)
        {
            currentOS = OperatingSystem.OSX;
        }
        else
        {
            currentOS = OperatingSystem.Win;
        }
    }

	public float GetAxis(ControllerInputWrapper.Axis axis, PlayerID id, bool isRaw = false)
    {
		if(!playerControls.ContainsKey(id)) return 0;
		if (playerControls[id] == null)
        {
            return 0;
        }
		return playerControls[id].GetAxis(axis, isRaw);
    }

	public float GetTrigger(ControllerInputWrapper.Triggers trigger, PlayerID id, bool isRaw = false)
    {
		if(!playerControls.ContainsKey(id)) return 0;
		if (playerControls[id] == null)
		{
			return 0;
		}
		return playerControls[id].GetTrigger(trigger, isRaw);
    }

	public bool GetButton(ControllerInputWrapper.Buttons button, PlayerID id)
    {
		if(!playerControls.ContainsKey(id)) return false;
		if (playerControls[id] == null)
		{
			return false;
		}
		return playerControls[id].GetButton(button);
    }

	public bool GetButtonDown(ControllerInputWrapper.Buttons button, PlayerID id) 
	{
		if(!playerControls.ContainsKey(id)) return false;
		if (playerControls[id] == null)
		{
			return false;
		}
		return playerControls[id].GetButtonDown(button);
	}

    public bool GetButtonUp(ControllerInputWrapper.Buttons button, int joyNum)
    {
//        joyNum--;
//        if (joyNum < 0 || playerControls[joyNum] == null)
//        {
//            return false;
//        }
//        return playerControls[joyNum].GetButtonUp(button, joyNum + 1);
		return false;
    }

    public bool GetButtonAll(ControllerInputWrapper.Buttons button, bool isDown)
    {
        //This definitely needs an update........... Like seriously..
//        int i = 0;
//        foreach (ControllerInputWrapper cW in playerControls)
//        {
//            if (!cW.GetButton(button, i, isDown)){
//                return false;
//            }
//        }
//        return true;
		return false;
    }

	public override string ToString ()
	{
		return currentOS.ToString(); 
	}
}
