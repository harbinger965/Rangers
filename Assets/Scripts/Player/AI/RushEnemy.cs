using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using Assets.Scripts.Tokens;

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
		private float turnTimer;
		/// <summary> Tick cooldown for the AI turning. </summary>
		private const float TURNCOOLDOWN = 1;

		/// <summary> The distance away from a ledge that the AI will tolerate. </summary>
		private const float LEDGEGRABDISTANCE = 0.6f;

		/// <summary> Timer for the AI to replan its ledge path. </summary>
		private float replanTimer;
		/// <summary> The time interval used for the AI to replan its ledge path. </summary>
		private const float REPLANTIME = 2;
		/// <summary> The current ledge path that the AI is taking. </summary>
		private LinkedList<LedgeNode> currentPath;
		/// <summary> The ledge node that the AI is currently headed for. </summary>
		private LedgeNode currentNode;

		/// <summary> The ledges in the scene. </summary>
		private static LedgeNode[] ledges;
		/// <summary> Stores the next node between two nodes. </summary>
		private static LedgeNode[,] next;
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
				// Create the level edge network with Floyd-Warshall.
				ledges = GameObject.FindObjectsOfType<LedgeNode>();
				next = new LedgeNode[ledges.Length, ledges.Length];
				float[,] distance = new float[ledges.Length, ledges.Length];
				for (int i = 0; i < ledges.Length; i++)
				{
					ledges[i].index = i;
				}
				for (int i = 0; i < ledges.Length; i++)
				{
					for (int j = 0; j < ledges.Length; j++)
					{
						distance[i, j] = Mathf.Infinity;
					}
				}
				for (int i = 0; i < ledges.Length; i++)
				{
					foreach (LedgeNode otherLedge in ledges[i].adjacentEdges)
					{
						distance[i, otherLedge.index] = Vector3.Distance(ledges[i].transform.position, otherLedge.transform.position);
						next[i, otherLedge.index] = otherLedge;
					}
				}
				for (int k = 0; k < ledges.Length; k++)
				{
					for (int i = 0; i < ledges.Length; i++)
					{
						for (int j = 0; j < ledges.Length; j++)
						{
							if (distance[i, k] + distance[k, j] < distance[i, j])
							{
								distance[i, j] = distance[i, k] + distance[k, j];
								next[i, j] = next[i, k];
							}
						}
					}
				}
				levelName = sceneName;
			}
		}

		/// <summary>
		/// Constructs a path from a start and end node.
		/// </summary>
		/// <returns>A path from the start to the end node.</returns>
		/// <param name="start">The node to start from.</param>
		/// <param name="end">The node to end at.</param>
		private LinkedList<LedgeNode> GetPath(LedgeNode start, LedgeNode end)
		{
			LinkedList<LedgeNode> path = new LinkedList<LedgeNode>();
			if (start == null || end == null || next[start.index, end.index] == null)
			{
				return path;
			}
			else
			{
				LedgeNode current = start;
				path.AddLast(start);
				while (current != end)
				{
					current = next[current.index, end.index];
					path.AddLast(current);
				}
				return path;
			}
		}


		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			if (target == null) {
				controller.SetRunInDirection(0);
				return;
			}
			Controller targetController = target.GetComponent<Controller>();
			if (targetController != null && targetController.LifeComponent.Health <= 0)
			{
				controller.SetRunInDirection(0);
				return;
			}

			lastSpeed = controller.runSpeed;

			controller.jump = false;
			float currentTargetDistance = targetDistance;
			Vector3 opponentOffset = target.transform.position - controller.transform.position;
			Vector3 targetOffset = opponentOffset;
			float distanceTolerance = targetDistance - 1;
			Vector3 playerCenter = controller.transform.position + Vector3.up * 0.75f;

			// Check if there is a platform in the way of shooting.
			RaycastHit hit;
			// Find a ledge path to get to the target.
			replanTimer -= Time.deltaTime;
			if (replanTimer <= 0)
			{
				replanTimer = REPLANTIME;
				currentPath = GetPath(GetClosestLedgeNode(controller.transform), GetClosestLedgeNode(target.transform));
				if (currentPath.Count > 0)
				{
					LoadNextLedge();
				}
			}

			if (currentNode != null)
			{
				// Move towards the nearest ledge, jumping if needed.
				Vector3 currentOffset = currentNode.transform.position - controller.transform.position;
				Vector3 nextPosition = currentPath.Count > 0 ? currentPath.First.Value.transform.position : target.transform.position;
				Vector3 nextOffset = nextPosition - currentNode.transform.position;

				// Go to the next ledge node if possible.
				if (nextOffset.y >= 0 && Math.Abs(currentOffset.x) <= LEDGEGRABDISTANCE && currentOffset.y <= 0  ||
					nextOffset.y < 0 && currentOffset.y >= 0)
				{
					LoadNextLedge();
				}
				else
				{
					// Smooth the path if possible.
					if (nextPosition == target.transform.position || GetPlatformUnderneath(controller.transform) == GetPlatformUnderneath(target.transform))
					{
						if (controller.HasClearShot(opponentOffset, out hit))
						{
							currentNode = null;
							replanTimer = 0;
						}
					}
					else if (nextOffset.y >= 0 && currentOffset.y <= 0 ||
						     nextOffset.y < 0)
					{
						RaycastHit nextPlatform;
						Vector3 nextOffsetSelf = nextPosition - playerCenter;
						if (Physics.Raycast(playerCenter, nextOffsetSelf, out nextPlatform, Vector3.Magnitude(nextOffsetSelf), AIController.LAYERMASK))
						{
							LedgeNode[] currentLedges = GetPlatformLedges(nextPlatform);
							bool skip = false;
							foreach (LedgeNode ledge in currentLedges)
							{
								if (ledge == currentNode && currentOffset.y > 0)
								{
									skip = false;
									break;
								}
								else if (ledge == currentPath.First.Value)
								{
									skip = true;
								}
							}
							if (skip)
							{
								LoadNextLedge();
							}
						}
					}
				}
				if (currentNode != null)
				{
					currentOffset = currentNode.transform.position - controller.transform.position;
					nextPosition = currentPath.Count > 0 ? currentPath.First.Value.transform.position : target.transform.position;
					nextOffset = nextPosition - currentNode.transform.position;

					currentTargetDistance = 0;
					distanceTolerance = 0.1f;

					if (Mathf.Abs(currentOffset.y) < distanceTolerance)
					{
						currentOffset.y = 0;
					}
					if (Mathf.Abs(nextOffset.y) < distanceTolerance)
					{
						nextOffset.y = 0;
					}

					Vector3 platformOffset = currentNode.transform.position - GetLedgePlatform(currentNode).transform.position;

					if (currentOffset.y > -2)
					{
						if (platformOffset.x > 0)
						{
							currentOffset.x += LEDGEGRABDISTANCE;
						}
						else
						{
							currentOffset.x -= LEDGEGRABDISTANCE;
						}
					}

					controller.jump = currentOffset.y > -0.75 && Math.Abs(currentOffset.x) <= LEDGEGRABDISTANCE / 2;

					targetOffset = currentOffset;
				}
			}

			Debug.DrawRay(controller.transform.position, targetOffset, Color.red);
			if (currentNode != null)
			{
				LedgeNode c = currentNode;
				foreach (LedgeNode l in currentPath)
				{
					Debug.DrawLine(c.transform.position, l.transform.position, Color.red);
					c = l;
				}
				Debug.DrawLine(c.transform.position, target.transform.position, Color.red);
			}

			RaycastHit under;
			Physics.Raycast(playerCenter, Vector3.down, out under, 30, AIController.LAYERMASK);
			LedgeNode closestLedge = null;

			// Check if the AI is falling to its death.
			if (under.collider == null)
			{
				// Find the closest ledge to go to.
				float closestLedgeDistance = Mathf.Infinity;
				foreach (LedgeNode ledge in ledges)
				{
					float currentDistance = Mathf.Abs(ledge.transform.position.x - controller.transform.position.x);
					if (currentDistance < closestLedgeDistance && ledge.transform.position.y < controller.transform.position.y + 1)
					{
						closestLedge = ledge;
						closestLedgeDistance = currentDistance;
					}
				}
				bool awayFromLedge = false;
				if (closestLedge == null)
				{
					controller.SetRunInDirection(-controller.transform.position.x);
				}
				else {
					float ledgeOffsetX = closestLedge.transform.position.x - controller.transform.position.x;
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
			else if (opponentOffset == targetOffset && under.collider != null && (controller.ParkourComponent.FacingRight ^ opponentOffset.x > 0))
			{
				controller.ParkourComponent.FacingRight = opponentOffset.x > 0;
			}
			else
			{
				controller.runSpeed = 0;
			}
			if (controller.runSpeed != 0)
			{
				// Don't chase an opponent off the map.
				Vector3 offsetPosition = controller.transform.position;
				offsetPosition.x += controller.runSpeed / 2;
				offsetPosition.y += 0.5f;
				Vector3 offsetPosition3 = offsetPosition;
				offsetPosition3.x += controller.runSpeed;
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
					controller.ParkourComponent.FacingRight = opponentOffset.x > 0;
					controller.slide = horizontalDistance > this.targetDistance * 2 && currentNode == null;
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
				turnTimer -= Time.deltaTime;
				if (turnTimer <= 0) {
					turnTimer = TURNCOOLDOWN;
				} else {
					controller.runSpeed = 0;
				}
			}

			// Jump to reach some tokens.
			if (targetDistance == 0 && controller.runSpeed == 0 && target.GetComponent<ArrowToken>()) {
				controller.jump = true;
			}
		}

		/// <summary>
		/// Gets the closest ledge node from a target.
		/// </summary>
		/// <returns>The closest node.</returns>
		/// <param name="target">Target.</param>
		private LedgeNode GetClosestLedgeNode(Transform target)
		{
			RaycastHit hit;
			LedgeNode[] currentLedges = ledges;
			if (Physics.Raycast(target.position + Vector3.up * 0.5f, Vector3.down, out hit, 30, AIController.LAYERMASK))
			{
				currentLedges = GetPlatformLedges(hit);
			}
			LedgeNode closestLedge = null;
			float closestDistance = Mathf.Infinity;
			foreach (LedgeNode ledge in currentLedges)
			{
				float currentDistance = Vector3.Distance(target.position, ledge.transform.position);
				if (ledge.transform.position.y < target.position.y + 1.5f &&
					currentDistance < closestDistance)
				{
					closestLedge = ledge;
					closestDistance = currentDistance;
				}
			}
			return closestLedge;
		}

		/// <summary>
		/// Loads the next ledge node into the current node.
		/// </summary>
		private void LoadNextLedge()
		{
			if (currentPath == null || currentPath.Count == 0)
			{
				currentNode = null;
				replanTimer = 0;
			}
			else
			{
				currentNode = currentPath.First.Value;
				currentPath.RemoveFirst();
			}
		}

		/// <summary>
		/// Gets the platform that a ledge is a part of.
		/// </summary>
		/// <returns>The platform that a ledge is a part of.</returns>
		/// <param name="ledge">The ledge to get a platform for.</param>
		private GameObject GetLedgePlatform(LedgeNode ledge)
		{
			return ledge.transform.parent.parent.gameObject;
		}

		/// <summary>
		/// Gets the ledges attached to a platform.
		/// </summary>
		/// <returns>The ledges attached to the platform.</returns>
		/// <param name="platformHit">The raycast that hit the platform.</param>
		private LedgeNode[] GetPlatformLedges(RaycastHit platformHit)
		{
			LedgeNode[] currentLedges;
			if (platformHit.collider.tag == "Ledge")
			{
				currentLedges = platformHit.collider.transform.parent.GetComponentsInChildren<LedgeNode>();
			}
			else
			{
				currentLedges = platformHit.collider.GetComponentsInChildren<LedgeNode>();
			}
			if (currentLedges.Length == 0 && platformHit.collider.transform.parent != null)
			{
				currentLedges = platformHit.collider.transform.parent.GetComponentsInChildren<LedgeNode>();
			}
			return currentLedges;
		}

		/// <summary>
		/// Gets the platform underneath a position.
		/// </summary>
		/// <returns>The platform underneath the position.</returns>
		/// <param name="target">The position to get a platform from.</param>
		private GameObject GetPlatformUnderneath(Transform target)
		{
			RaycastHit hit;
			if (Physics.Raycast(target.position + Vector3.up * 0.5f, Vector3.down, out hit, 30, AIController.LAYERMASK))
			{
				Transform current = hit.collider.transform;
				while (current != null)
				{
					if (current.tag == "Ground")
					{
						return current.gameObject;
					}
					current = current.parent;
				}
			}
			return null;
		}
	}
}