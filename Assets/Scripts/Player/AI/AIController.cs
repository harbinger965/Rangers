using UnityEngine;
using Assets.Scripts.Data;
using Assets.Scripts.Util;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Player.AI
{
    /// <summary>
	/// Interface for AI to control a ranger.
	/// </summary>
    public class AIController : Controller
    {
        /// <summary> The policy used to decide the AI's actions. </summary>
		private IPolicy policy;

		/// <summary> The speed that the ranger is moving at. </summary>
        [SerializeField]
		[Tooltip("The speed that the ranger is moving at.")]
        internal float runSpeed = 0;

		/// <summary> Whether the ranger is jumping. </summary>
        [SerializeField]
		[Tooltip("Whether the ranger is jumping.")]
        internal bool jump = false;
		/// <summary> Whether the ranger is sliding. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is sliding.")]
        internal bool slide = false;

        /// <summary> Whether the ranger is aiming a shot. </summary>
		[SerializeField]
		[Tooltip("Whether the ranger is aiming a shot.")]
        internal bool aiming = false;
        /// <summary> Whether the ranger was aiming its shot on the last tick. </summary>
        private bool wasAiming = false;
        /// <summary> Vector in the direction that the ranger is aiming. </summary>
		[SerializeField]
		[Tooltip("Vector in the direction that the ranger is aiming.")]
        internal Vector3 aim = Vector3.zero;

        /// <summary> The default movement speed of the ranger. </summmary>
		[SerializeField]
		[Tooltip("The default movement speed of the ranger.")]
        private float defaultMoveSpeed = 1;

        /// <summary> The opponent of this ranger. </summary>
		[Tooltip("The opponent of this ranger.")]
        public Controller opponent;

		/// <summary> The AI mode that this ranger will use. </summary>
		[Tooltip("The AI mode that this ranger will use.")]
		public Enums.AIModes mode;

        /// <summary>
		/// Initializes the AI policy to use.
		/// </summary>
        private void Start()
        {
			switch(mode)
			{
			case Enums.AIModes.ApproachShoot: policy = new ApproachShoot(); break;
			case Enums.AIModes.RangerBot: policy = new RangerBot(); break;
			}
            foreach (Controller controller in GameManager.instance.AllPlayers)
            {
                if (controller != this)
                {
                    opponent = controller;
                    break;
                }
            }
            base.Start();
        }

        /// <summary>
		/// Moves the ranger every tick.
		/// </summary>
        private void Update()
        {
            if (opponent == null || life.Health <= 0)
            {
                aiming = false;
                return;
            }
            else
			{
				policy.ChooseAction(this);
            }

            if (jump)
            {
                parkour.Jump();
            }

            if (slide)
            {
                parkour.SlideOn();
            }
            else
            {
                parkour.SlideOff();
            }

            if (aiming)
            {
                archery.UpdateFirePoint(Vector3.Normalize(aim));
                wasAiming = true;
            }
            else if (wasAiming)
            {
                wasAiming = false;
                archery.Fire();
            }
            else
            {
                archery.AimUpperBodyWithLegs();
            }
        }

        /// <summary>
		/// Updates the ranger's running movement.
		/// </summary>
        private void FixedUpdate()
        {
            if (life.Health > 0)
            {
                parkour.Locomote(runSpeed);
            }
        }

        /// <summary>
		/// Sets the ranger's run speed in the direction of the given number.
        /// </summary>
        /// <param name="direction">The direction to set the run speed to.</param>
        internal void SetRunInDirection(float direction)
        {
            if (direction > 0)
            {
                runSpeed = defaultMoveSpeed;
            }
            else if (direction < 0)
            {
                runSpeed = -defaultMoveSpeed;
            }
            else {
                runSpeed = 0;
            }
        }

        /// <summary>
        /// Gets a vector of the difference between the opponent position and the AI position.
        /// </summary>
        /// <returns>A vector of the difference between the opponent position and the AI position.</returns>
        internal Vector3 GetOpponentDistance()
        {
            return opponent.transform.position - transform.position;
        }
    }
}