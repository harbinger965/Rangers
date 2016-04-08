using UnityEngine;
using System;
using Assets.Scripts.Data;
using Assets.Scripts.Player;

namespace Assets.Scripts.Levels
{
    public class Teleporter : MonoBehaviour
{ 
        public Transform portal;

        void OnTriggerEnter(Collider other)
        {
			if(!other.transform.root.gameObject.name.Equals("Level")) {
				other.transform.root.position = portal.position + transform.forward*2f + Vector3.up;
			}
        }
    }
}