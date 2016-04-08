using UnityEngine;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// A node in the ledge network.
	/// </summary>
	public class LedgeNode : MonoBehaviour {
		/// <summary> The edges adjacent to this edge. </summary>
		[Tooltip("The edges adjacent to this edge.")]
		public LedgeNode[] adjacentEdges;
		/// <summary> The index of the node. </summary>
		[HideInInspector]
		public int index;
	}
}