using UnityEngine;
using UnityEngine.SceneManagement;
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

		/// <summary> The distance away from a ledge that the AI will tolerate. </summary>
		private const float LEDGEGRABDISTANCE = 0.6f;

		/// <summary> The ledges in the scene. </summary>
		private static GameObject[] ledges;
		/// <summary> The name of the level that ledges are currently cached for. </summary>
		private static string levelName;

		/// <summary>
		/// Initializes a new AI.
		/// </summary>
		/// <param name="targetDistance">The desired horizontal distance between the AI and the opponent.</param>
		internal RushEnemy(float targetDistance) {
			this.targetDistance = targetDistance;
			string sceneName = SceneManager.GetActiveScene().name;
			if (ledges == null || levelName != sceneName)
			{
				// Load the level's edges if not already cached.
				ledges = GameObject.FindGameObjectsWithTag("Ledge");
				levelName = sceneName;
			}
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

			controller.jump = false;
			float currentTargetDistance = targetDistance;
			Vector3 opponentOffset = target.transform.position - controller.transform.position;
			Vector3 targetOffset = opponentOffset;
			float distanceTolerance = 1f;


			// Check if there is a platform in the way of shooting.
			RaycastHit hit;
			controller.HasClearShot(opponentOffset, out hit);

			Transform blockingLedge = null;
			if (hit.collider != null)
			{
				// If an obstacle is in the way, move around it.
				float closestDistance = Mathf.Infinity;
				// Find ledges on the obstructing platform.
				BoxCollider[] children = hit.collider.GetComponentsInChildren<BoxCollider>();
				bool foundBetween = false;
				foreach (BoxCollider child in children)
				{
					if (child.tag == "Ledge")
					{
						Vector3 ledgeOffset = child.transform.position - controller.transform.position;
						if (Mathf.Sign(ledgeOffset.y) == Mathf.Sign(targetOffset.y)) {
							// Look for the closest ledge to grab or fall from.
							float currentDistance = Mathf.Abs(child.transform.position.x - controller.transform.position.x);
							bool between = BetweenX(child.transform, controller.transform, target.transform);
							if (!(foundBetween && !between) && currentDistance < closestDistance || !foundBetween && between)
							{
								foundBetween = foundBetween || between;
								// Make sure the edge isn't off the side of the map.
								float edgeMultiplier = 3;
								if (Physics.Raycast(child.transform.position + Vector3.down * 0.5f + Vector3.left * edgeMultiplier, Vector3.down, 30, AIController.LAYERMASK) ||
									Physics.Raycast(child.transform.position + Vector3.down * 0.5f + Vector3.right * edgeMultiplier, Vector3.down, 30, AIController.LAYERMASK))
								{
									// Don't target ledges that have already been jumped over.
									if (currentDistance > LEDGEGRABDISTANCE || child.transform.position.y >= controller.transform.position.y)
									{
										blockingLedge = child.transform;
										closestDistance = currentDistance;
									}
								}
							}
						}
					}
				}
			}

			Transform gapLedge = null;
			RaycastHit under;
			Physics.Raycast(controller.transform.position + Vector3.up * 0.5f, Vector3.down, out under, 30, AIController.LAYERMASK);
			if (hit.collider == null)
			{
				// If the ranger and its target are not on the same platform, go to a nearby ledge.
				RaycastHit underTarget;
				Physics.Raycast(target.transform.position + Vector3.up * 0.5f, Vector3.down, out underTarget, 30, AIController.LAYERMASK);
				if (under.collider != null && underTarget.collider != null && under.collider.gameObject != underTarget.collider.gameObject)
				{
					float closestLedgeDistance = Mathf.Infinity;
					foreach (GameObject ledge in ledges)
					{
						float currentDistance = Mathf.Abs(ledge.transform.position.x - controller.transform.position.x);
						if (currentDistance < closestLedgeDistance && ledge.transform.position.y > controller.transform.position.y + 1 &&
							BetweenX(ledge.transform, controller.transform, target.transform))
						{
							gapLedge = ledge.transform;
							closestLedgeDistance = currentDistance;
						}
					}
				}
			}

			Transform closestLedge;
			if (blockingLedge == null)
			{
				closestLedge = gapLedge;
			}
			else if (gapLedge == null)
			{
				closestLedge = blockingLedge;
			}
			else
			{
				if (Vector3.Distance(blockingLedge.position, controller.transform.position) <= Vector3.Distance(gapLedge.position, controller.transform.position))
				{
					closestLedge = blockingLedge;
				}
				else
				{
					closestLedge = gapLedge;
				}
			}

			if (closestLedge != null)
			{
				// Move towards the nearest ledge, jumping if needed.
				Vector3 closestVector = closestLedge.position - controller.transform.position;
				if (closestLedge.position.x - closestLedge.parent.position.x > 0)
				{
					closestVector.x += LEDGEGRABDISTANCE;
				}
				else
				{
					closestVector.x -= LEDGEGRABDISTANCE;
				}
				if (Math.Abs(closestVector.x) < 1f)
				{
					controller.jump = opponentOffset.y > 0 || gapLedge != null;
				}
				else
				{
					controller.jump = false;
				}
				currentTargetDistance = 0;
				distanceTolerance = 0.1f;
				targetOffset = closestVector;
			}
			Debug.DrawRay(controller.transform.position, targetOffset);

			// Check if the AI is falling to its death.
			if (under.collider == null)
			{
				// Find the closest ledge to go to.
				closestLedge = null;
				float closestLedgeDistance = Mathf.Infinity;
				foreach (GameObject ledge in ledges)
				{
					float currentDistance = Mathf.Abs(ledge.transform.position.x - controller.transform.position.x);
					if (currentDistance < closestLedgeDistance && ledge.transform.position.y < controller.transform.position.y + 1)
					{
						closestLedge = ledge.transform;
						closestLedgeDistance = currentDistance;
					}
				}
				bool awayFromLedge = false;
				if (closestLedge == null)
				{
					controller.SetRunInDirection(-controller.transform.position.x);
				}
				else {
					float ledgeOffsetX = closestLedge.position.x - controller.transform.position.x;
					if (Mathf.Abs(ledgeOffsetX) > LEDGEGRABDISTANCE)
					{
						controller.SetRunInDirection(ledgeOffsetX);
						awayFromLedge = true;
					}
				}
				controller.jump = true;
				if (awayFromLedge)
				{
					return;
				}
			}

			if (currentTargetDistance > 0 && targetOffset.y < -1 && (closestLedge != null || Mathf.Abs(opponentOffset.x) > 1))
			{
				// Move onto a platform if a ledge was just negotiated.
				currentTargetDistance = 0;
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
				Vector3 offsetPosition3 = offsetPosition;
				offsetPosition3.x += controller.runSpeed * 2;
				if (!Physics.Raycast(offsetPosition, Vector3.down, out hit, 30, AIController.LAYERMASK) &&
					!Physics.Raycast(offsetPosition3, Vector3.down, out hit, 30, AIController.LAYERMASK))
				{
					if (controller.ParkourComponent.Sliding)
					{
						controller.SetRunInDirection(-opponentOffset.x);
					}
					else if (closestLedge == null)
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

			if (controller.runSpeed == 0 && Mathf.Abs(opponentOffset.x) < 1 && opponentOffset.y < 0 && target.GetComponent<Controller>() && controller.GetComponent<Rigidbody>().velocity.y <= Mathf.Epsilon)
			{
				// Don't sit on top of the opponent.
				controller.SetRunInDirection(-controller.transform.position.x);
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

		/// <summary>
		/// Checks whether an object is between two other objects in the x direction.
		/// </summary>
		/// <returns>Whether the object is between two other objects in the x direction.</returns>
		/// <param name="middle">The object to check for being between two others.</param>
		/// <param name="limit1">One object to be between.</param>
		/// <param name="limit2">The other object to be between.</param>
		private bool BetweenX(Transform middle, Transform limit1, Transform limit2)
		{
			return Mathf.Sign(middle.position.x - limit1.position.x) != Mathf.Sign(middle.position.x - limit2.position.x);
		}
	}
}