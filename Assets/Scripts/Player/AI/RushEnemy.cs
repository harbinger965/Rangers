using System;
using UnityEngine;

namespace Assets.Scripts.Player.AI
{
	/// <summary> Rushes the opponent to fire at close range. </summary>
	public class RushEnemy : IPolicy
	{
		/// <summary> The desired horizontal distance between the AI and the opponent. </summary>
		private float targetDistance;

		/// <summary>
		/// Initializes a new instance of the <see cref="Assets.Scripts.Player.AI.RushEnemy"/> class.
		/// </summary>
		/// <param name="targetDistance">The desired horizontal distance between the AI and the opponent.</param>
		public RushEnemy(float targetDistance) {
			this.targetDistance = targetDistance;
		}

		public void ChooseAction(AIController controller)
		{
			Vector3 opponentDistance = controller.GetOpponentDistance();
			if (Mathf.Abs(opponentDistance.x) > targetDistance) 
			{
				controller.SetRunInDirection(opponentDistance.x);
				// Don't chase an opponent off the map.
				RaycastHit hit = new RaycastHit();
				Vector3 offsetPosition = controller.transform.position;
				offsetPosition.x += controller.runSpeed;
				if (!Physics.Raycast(offsetPosition, Vector3.down, out hit, 100, 1 << 13)) {
					controller.runSpeed = 0;
				}
			} else {
				controller.runSpeed = 0;
			}
		}
	}
}

