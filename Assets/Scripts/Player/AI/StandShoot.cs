using System;
using UnityEngine;

namespace Assets.Scripts.Player.AI
{
	/// <summary> AI that always stands still and shoots at the player. </summary>
	public class StandShoot : IPolicy
	{
		/// <summary> Timer for delay between firing. </summary>
		private float fireCooldown = 0;
		/// <summary> The delay between firing. </summary>
		private float cooldownTime = 5;
		/// <summary> The power that the AI will shoot arrows at. </summary>
		private float power;

		/// <summary>
		/// Initializes a new instance of the <see cref="Assets.Scripts.Player.AI.StandShoot"/> class.
		/// </summary>
		/// <param name="cooldownTime">The delay between firing.</param>
		/// <param name="power">The power that the AI will shoot arrows at.</param>
		public StandShoot(float cooldownTime, float power) {
			this.cooldownTime = cooldownTime;
			this.power = power;
		}

		public void ChooseAction(AIController controller)
		{
			if (controller.opponent.LifeComponent.Health <= 0) {
				controller.aiming = false;
				return;
			}
			controller.aim = Vector3.Normalize(controller.opponent.transform.position - controller.transform.position);
			if (controller.aiming)
			{
				if (controller.ArcheryComponent.StrengthPercentage > power)
				{
					controller.aiming = false;
				}
			}
			else
			{
				if (fireCooldown++ > cooldownTime)
				{
					controller.aiming = true;
					fireCooldown = 0;
				}
			}
		}
	}
}

