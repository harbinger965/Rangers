using Assets.Scripts.Timers;
using System;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    public class ZeroGravityAttack : GravityAttack
    {
        protected override void HandleTriggerEnter(Collider col)
        {
            Rigidbody rigidbody = col.transform.root.GetComponent<Rigidbody>();
            if(rigidbody)
            {
                rigidbody.useGravity = false;
            }
        }

        protected override void HandleTriggerStay(Collider col)
        {
            Rigidbody rigidbody = col.transform.root.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.useGravity = false;
            }
        }

        protected override void HandleTriggerExit(Collider col)
        {
            Rigidbody rigidbody = col.transform.root.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.useGravity = true;
            }
        }

        protected override void Final(Timer t)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
            foreach (Collider col in cols)
            {
                Rigidbody rigidbody = col.transform.root.GetComponent<Rigidbody>();
                if (rigidbody)
                {
                    rigidbody.useGravity = true;
                }
                Destroy(gameObject);
            }
        }
    }
}
