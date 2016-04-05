using UnityEngine;
using System;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Rushes the opponent to fire at close range.
	/// </summary>
	public class RushEnemy : IPolicy
	{
		/// <summary> The desired horizontal distance between the AI and the target. </summary>
		internal float targetDistance;

		/// <summary> The object that the AI is targeting. </summary>
		internal GameObject target;

		/// <summary> The AI's speed on the previous tick.</summary>
		private float lastSpeed;

		/// <summary> Timer for allowing the AI to turn. </summary>
		private int turnTimer;
		/// <summary> Tick cooldown for the AI turning. </summary>
		private const int TURNCOOLDOWN = 3;

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
			if (target == null) {
				return;
			}

			lastSpeed = controller.runSpeed;

			// Check if the AI is falling to its death.
			RaycastHit under;
			Physics.Raycast(controller.transform.position + Vector3.up * 0.5f, Vector3.down, out under, 30, AIController.LAYERMASK);
			if (under.collider == null)
			{
				controller.SetRunInDirection(-controller.transform.position.x);
				controller.jump = true;
				return;
			}

			controller.jump = false;
			float currentTargetDistance = targetDistance;
			Vector3 opponentOffset = target.transform.position - controller.transform.position;
			Vector3 targetOffset = opponentOffset;
			float distanceTolerance = 1f;


			// Check if there is a platform in the way of shooting.
			RaycastHit hit;
			controller.HasClearShot(opponentOffset, out hit);
			if (hit.collider != null)
			{
				// If an obstacle is in the way, move around it.
				BoxCollider closest = null;
				float closestDistance = Mathf.Infinity;
				if (hit.collider != null) {
					// Find ledges on the obstructing platform.
					BoxCollider[] children = hit.collider.GetComponentsInChildren<BoxCollider>();
					foreach (BoxCollider child in children)
					{
						if (child.tag == "Ledge")
						{
							Vector3 ledgeOffset = child.transform.position - controller.transform.position;
							if (Mathf.Sign(ledgeOffset.y) == Mathf.Sign(targetOffset.y)) {
								// Look for the closest ledge to grab or fall from.
								float currentDistance = Vector3.Distance(child.transform.position, controller.transform.position);
								if (currentDistance < closestDistance)
								{
									if (Physics.Raycast(child.transform.position + Vector3.down * 0.5f, Vector3.down, 30, AIController.LAYERMASK)) {
										// Only use ledges that aren't hanging off the edge of the stage.
										closest = child;
										closestDistance = currentDistance;
									}
								}
							}
						}
					}
				}
				if (closest != null)
				{
					// Move towards the nearest ledge, jumping if needed.
					Vector3 closestVector = closest.transform.position - controller.transform.position;
					float ledgeOffset = 0.6f;
					if (hit.collider != null)
					{
						if (closest.transform.position.x - hit.collider.transform.position.x > 0)
						{
							closestVector.x += ledgeOffset;
						}
						else
						{
							closestVector.x -= ledgeOffset;
						}
					}
					if (Math.Abs(closestVector.x) < 1f)
					{
						controller.jump = opponentOffset.y > 0;
					}
					else
					{
						controller.jump = false;
					}
					currentTargetDistance = 0;
					distanceTolerance = 0.1f;
					targetOffset = closestVector;
				}
			}

			if (currentTargetDistance > 0 && targetOffset.y < 0)
			{
				// Move onto a platform if a ledge was just negotiated.
				if (!Physics.Raycast(controller.transform.position, Vector3.down, out hit, -targetOffset.y + 0.5f, AIController.LAYERMASK))
				{
					currentTargetDistance = 0;
				}
			}

			// Move towards the opponent.
			float horizontalDistance = Mathf.Abs(targetOffset.x);
			if (horizontalDistance > currentTargetDistance)
			{
				controller.SetRunInDirection(targetOffset.x);
			}
			else if (horizontalDistance < currentTargetDistance - distanceTolerance)
			{
				controller.SetRunInDirection(-targetOffset.x);
			}
			else
			{
				controller.runSpeed = 0;
			}
			if (controller.runSpeed != 0)
			{
				// Don't chase an opponent off the map.
				Vector3 offsetPosition = controller.transform.position;
				offsetPosition.x += controller.runSpeed;
				offsetPosition.y += 0.5f;
				if (!Physics.Raycast(offsetPosition, Vector3.down, out hit, 30, AIController.LAYERMASK))
				{
					if (controller.ParkourComponent.Sliding)
					{
						controller.SetRunInDirection(-opponentOffset.x);
					}
					else
					{
						controller.runSpeed = 0;
					}
					controller.slide = false;
				}
				else
				{
					// Slide if the opponent is far enough away for sliding to be useful.
					controller.slide = horizontalDistance > targetDistance * 2;
				}
			}

			if (controller.runSpeed > 0 && lastSpeed < 0 || controller.runSpeed < 0 && lastSpeed > 0)
			{
				// Check if the AI turned very recently to avoid thrashing.
				if (turnTimer-- <= 0) {
					turnTimer = TURNCOOLDOWN;
				} else {
					controller.runSpeed = 0;
				}
			}

			// Jump to reach some tokens.
			if (targetDistance == 0 && controller.runSpeed == 0) {
				controller.jump = true;
			}
		}
	}
}