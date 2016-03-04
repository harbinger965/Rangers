using Assets.Scripts.Timers;
using UnityEngine;

namespace Assets.Scripts.Attacks
{
    public abstract class GravityAttack : SpawnAttack
    {
        [SerializeField]
        protected bool world;
        [SerializeField]
        private float aliveTime = 7f;

        void Start()
        {
            Timer t = gameObject.AddComponent<Timer>();
            t.TimeOut += new Timer.TimerEvent(Final);
            t.Initialize(aliveTime, "Gravity Effect");
        }

        protected abstract void Final(Timer t);

        void OnTriggerEnter(Collider col)
        {
            HandleTriggerEnter(col);
        }

        void OnTriggerStay(Collider col)
        {
            HandleTriggerStay(col);
        }

        void OnTriggerExit(Collider col)
        {
            HandleTriggerExit(col);
        }

        protected abstract void HandleTriggerEnter(Collider col);
        protected abstract void HandleTriggerStay(Collider col);
        protected abstract void HandleTriggerExit(Collider col);
    }
}
