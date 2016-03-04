using System;
using UnityEngine;
using Assets.Scripts.Util;

/// <summary>
/// Class for saving player profile data
/// </summary>
[Serializable]
public class ProfileData
{
	private string tag;
    private Color primary, secondary;

    public ProfileData()
    {
        tag = "";
        primary = CustomColor.Black;
        secondary = CustomColor.White;
    }
    public ProfileData(string name)
    {
        this.tag = name;
        primary = CustomColor.Black;
        secondary = CustomColor.White;
    }

    public string Name
    {
        get { return tag; }
        set { tag = value; }
    }
}
