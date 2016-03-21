using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Approaches the opponent and shoots at it.
	/// </summary>
	public class ApproachShoot : IPolicy {

		/// <summary> The policies to employ with this policy. </summary>
		private IPolicy[] policies;

		/// <summary>
		/// Initializes a new AI.
		/// </summary>
		internal ApproachShoot()
		{
			policies = new IPolicy[]{new Shoot(5, 0.5f), new RushEnemy(10)};
		}

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			foreach (IPolicy policy in policies)
			{
				policy.ChooseAction(controller);
			}
		}
	}
}