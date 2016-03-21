using UnityEngine;
using System;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Rushes the opponent to fire at close range.
	/// </summary>
	public class RushEnemy : IPolicy
	{
		/// <summary> The desired horizontal distance between the AI and the opponent. </summary>
		internal float targetDistance;

		/// <summary>
		/// Initializes a new AI.
		/// </summary>
		/// <param name="targetDistance">The desired horizontal distance between the AI and the opponent.</param>
		internal RushEnemy(float targetDistance) {
			this.targetDistance = targetDistance;
		}

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			Vector3 opponentDistance = controller.GetOpponentDistance();
			float horizontalDistance = Mathf.Abs(opponentDistance.x);
			if (horizontalDistance > targetDistance)
			{
				controller.SetRunInDirection(opponentDistance.x);
			}
			else if (horizontalDistance < targetDistance - 1f)
			{
				controller.SetRunInDirection(-opponentDistance.x);
			}
			else
			{
				controller.runSpeed = 0;
			}
			if (controller.runSpeed != 0) {
				// Don't chase an opponent off the map.
				RaycastHit hit = new RaycastHit();
				Vector3 offsetPosition = controller.transform.position;
				offsetPosition.x += controller.runSpeed;
				offsetPosition.y += 0.5f;
				if (!Physics.Raycast(offsetPosition, Vector3.down, out hit, 100, 1 << 13))
				{
					if (controller.ParkourComponent.Sliding)
					{
						controller.SetRunInDirection(-opponentDistance.x);
					}
					else
					{
						controller.runSpeed = 0;
					}
					controller.slide = false;
				}
				else
				{
					controller.slide = horizontalDistance > targetDistance * 2;
				}
			}
		}
	}
}