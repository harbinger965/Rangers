using UnityEngine;
using System.Collections.Generic;

public class Statistic {

	public int arrowsShot, arrowsHit, suicides;

	private float accuracy;

	public Dictionary<PlayerID, int> killedPlayer, killedByPlayer;

	public Statistic() {
		killedPlayer = new Dictionary<PlayerID, int>();
		killedByPlayer = new Dictionary<PlayerID, int>();
	}

	public float Accuracy {
		get {
			return arrowsShot/(float)arrowsHit;
		}
	}

}
