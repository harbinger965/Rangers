using UnityEngine;
using System;

namespace Assets.Scripts.Player.AI
{
	/// <summary>
	/// Uses an MDP to decide what to do.
	/// </summary>
	public class RangerBot : IPolicy {

		private int[] spacingReinforcement = { 20, 20, 20 };  // reinforcement for spacing --Chris   (modify based on positive and negative feedback)
		private int[] distanceReinforcement = { 20, 20, 20 }; // dictates what range the bot wants to be at  (modify based on positive and negative feedback)
		private int chanceToShoot = 100;                    // modifier simulating passivity (lower is passive, higher is active)
		private int rangeClass = 0;                         // Range group label that is used to keep track of which range is given Incentive or Disincentive
		private System.Random random = new System.Random();
		private Shoot shotInst = new Shoot(1, 0.6f);
		private RushEnemy rushInst;

		/// <summary>
		/// Picks an action for the character to do every tick.
		/// </summary>
		/// <param name="controller">The controller for the character.</param>
		public void ChooseAction(AIController controller)
		{
			float totalOppDistance = Vector3.Distance(controller.opponent.transform.position, controller.transform.position);
			int shotChanceModifier = 0;

			// Experiment with 3 distance groups (determines if the bot shoots)

			//Choose the distance modifier
			if (totalOppDistance < 5)
			{
				shotChanceModifier = spacingReinforcement[0];
			}
			else if (totalOppDistance < 9)
			{
				shotChanceModifier = spacingReinforcement[1];
			}
			else
			{
				shotChanceModifier = spacingReinforcement[2];
			}


			//Check to see if CPU will shoot given the modifier
			int randShotChance = random.Next(200000);
			if (shotChanceModifier * chanceToShoot < randShotChance)
			{
				shotInst.ChooseAction(controller);
			}

			//Experiment with 3 distance groups (determines favorite distance)
			int randMoveChance = random.Next(100);
			if (randMoveChance < distanceReinforcement[0])
			{
				rushInst.targetDistance = 3;
			}
			else if (randMoveChance < (distanceReinforcement[0] + distanceReinforcement[1]))
			{
				rushInst.targetDistance = 4;
			}
			else if (randMoveChance < (distanceReinforcement[0] + distanceReinforcement[1] + distanceReinforcement[2]))
			{
				rushInst.targetDistance = 5;
			}
			rushInst.ChooseAction(controller);
		}
	}
}