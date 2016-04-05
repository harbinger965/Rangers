﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class InputTester : MonoBehaviour {

	public static InputTester instance;

	public Sprite XBOX_A, PS4_A, KEY_A, XBOX_B, PS4_B, KEY_B, XBOX_START, PS4_START, KEY_START, JOYLEFT, DPAD_LEFTRIGHT, KEY_UPDOWNLEFTRIGHT,KEY_LEFTRIGHT;
	public Sprite L_JOY_CLICK, KEY_L_JOY_CLICK, R_JOY_CLICK, KEY_R_JOY_CLICK, XBOX_X, PS4_X, KEY_X, XBOX_Y, PS4_Y, KEY_Y, XBOX_BACK, PS4_BACK, KEY_BACK;

	private ControllerManager cm;

    void Awake()
    {
        if (instance == null)
        {
			DontDestroyOnLoad(this);
            instance = this;
            cm = new ControllerManager();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
}
