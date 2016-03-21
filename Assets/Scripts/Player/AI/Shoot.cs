using UnityEngine;
using System;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// AI that shoots at the player.
	/// </summary>
	public class Shoot : IPolicy
	{
		/// <summary> Timer for delay between firing. </summary>
		private float fireCooldown = 0;
		/// <summary> The delay between firing. </summary>
		private float cooldownTime = 5;
		/// <summary> The minimum power that the AI will shoot arrows at. </summary>
		private float power;

		/// <summary> Square root of 2. </summary>
		private float ROOT2 = Mathf.Sqrt(2);
		/// <summary> Square root of 2 over 2 (cos45, sin45) </summary>
		private float ROOT22 = Mathf.Sqrt(2) / 2;

		/// <summary>
		/// Initializes a new instance of the <see cref="Assets.Scripts.Player.AI.StandShoot"/> class.
		/// </summary>
		/// <param name="cooldownTime">The delay between firing.</param>
		/// <param name="power">The minimum power that the AI will shoot arrows at.</param>
		internal Shoot(float cooldownTime, float power) {
			this.cooldownTime = cooldownTime;
			this.power = power;
		}

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			if (controller.opponent.LifeComponent.Health <= 0) {
				controller.aiming = false;
				return;
			}

			// Calculate the needed angle and strength to hit the opponent.
			Vector3 distance = controller.opponent.transform.position - controller.transform.position;
			float x = distance.x;
			float y = distance.y;
			float g = Physics.gravity.y;

			// Minimum speed at 45 degrees.
			float v = Mathf.Sqrt(x * x * g / (x + 2 * y * ROOT22 * ROOT22));
			//Debug.Log(v);
			float minStrength = (-4 + Mathf.Sqrt(8 + 160 * v)) / 80;
			minStrength = Mathf.Sqrt(4 * minStrength * minStrength - minStrength * ROOT2 / 5 + 0.01f) / 2;
			float strength = Mathf.Max(power, minStrength);
			float angle = Mathf.PI / 4;

			// Change angle if the required power is greater than the minimum.
			/*
			if (strength != minStrength)
			{
				v = strength * 40 * (strength + 0.1f);
				float v2 = v * v;

				float discriminant = v2 * v2 - g * (g * x * x - 2 * y * v2);
				if (discriminant < 0)
				{
					discriminant = 0;
				}
				discriminant = 0;
				angle = Mathf.Atan((v2 - Mathf.Sqrt(discriminant)) / (g * x));
				Debug.Log(angle + " " + v + " " + x + " " + y);
			}*/

			controller.aim = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI) * Vector3.up;

			//controller.aim = Vector3.Normalize(controller.opponent.transform.position - controller.transform.position);

			if (controller.aiming)
			{
				if (controller.ArcheryComponent.StrengthPercentage > strength - Time.deltaTime / 2f)
				{
					Debug.Log(v);
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