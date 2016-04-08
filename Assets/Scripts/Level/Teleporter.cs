using UnityEngine;
using System;
using Assets.Scripts.Data;
using Assets.Scripts.Player;

namespace Assets.Scripts.Levels
{
    public class Teleporter : MonoBehaviour
{ 
        public Transform portal;

        void OnCollisionEnter(Collision other)
        {
            other.collider.transform.position = portal.position;
        }
    }
}