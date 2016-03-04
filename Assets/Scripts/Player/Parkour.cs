using UnityEngine;
using Assets.Scripts.Tokens;
using System.Collections;

namespace Assets.Scripts.Player
{
	public class Parkour : ControllerObject 
	{

		private bool facingRight = true;
		private bool jumping = false;
		private bool ledgeGrabbing = false;
		private bool sliding = false;

		private GameObject IKThingy;

		private float slidingTime;
		private float lastMotion;
		private float jumpingTimeOffset;

		public void Locomote(float motion) 
		{
			transform.rotation = new Quaternion(0, transform.rotation.y, 0, transform.rotation.w);
			if(motion > 0) 
			{
				facingRight = true;
			} 
			else if (motion < 0) 
			{
				facingRight = false;
			}
			if(!sliding) 
			{
				if(facingRight && Physics.Raycast(new Ray(transform.position, transform.forward),0.5f,~(1<<9))) 
				{
					GetComponent<Animator>().SetFloat("RunSpeed", Mathf.Min(0,motion));
				} 
				else if(!facingRight && Physics.Raycast(new Ray(transform.position, -transform.forward),0.5f,~(1<<9))) 
				{
					GetComponent<Animator>().SetFloat("RunSpeed", Mathf.Max(0,motion));
				} 
				else 
				{
					if(!jumping) 
					{
						GetComponent<Animator>().SetFloat("RunSpeed", motion);
						transform.Translate(Vector3.forward*motion*Time.deltaTime*8);
					} 
					else 
					{
						GetComponent<Animator>().SetFloat("RunSpeed", motion);
						transform.Translate(Vector3.forward*motion*Time.deltaTime*4);
					}
				}
				lastMotion = motion;
			} 
			else 
			{
				GetComponent<Animator>().SetFloat("RunSpeed", 0);
			}

		}

		public void Jump() 
		{
			if(!jumping) 
			{
				GetComponent<Animator>().ResetTrigger("Land");
				if(facingRight)
				{
					GetComponent<Animator>().SetTrigger("Jump");
				} 
				else if (!facingRight) 
				{
					GetComponent<Animator>().SetTrigger("JumpLeft");
				}
				jumping = true;
				jumpingTimeOffset = 0.1f;
			}
		}

		void Update() 
		{
			jumpingTimeOffset -= Time.deltaTime;
//			if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
////				transform.Translate(Vector3.right*lastMotion*Time.deltaTime*16f, Space.World);
//				sliding = true;
////				if(GetComponent<Animator>().IsInTransition(0)) {
////					lastMotion -= Time.deltaTime;
////				}
//			} else if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SlideLeft")) {
////				transform.Translate(-Vector3.left*lastMotion*Time.deltaTime*16f, Space.World);
//				sliding = true;
////				if(GetComponent<Animator>().IsInTransition(0)) {
////					lastMotion += Time.deltaTime;
////				}
//			} else {
//				sliding = false;
//			}
//			if((facingRight && !Physics.Raycast(new Ray(transform.position, transform.forward),0.5f,~(1<<9)))
//				|| (!facingRight && !Physics.Raycast(new Ray(transform.position, -transform.forward),0.5f,~(1<<9)))) {
//
//				if((GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("WalkRunRight -> Slide")
//					|| GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("WalkRunLeft -> SlideLeft"))
//					&& !GetComponent<Animator>().IsInTransition(0)) {
//					sliding = true;
//					transform.Translate(Vector3.forward*lastMotion*Time.deltaTime*8f);
//				} else if(GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("Slide -> WalkRunRight")) {
//					sliding = false;
//					transform.Translate(Vector3.forward*Time.deltaTime*8);
//				} else if(GetComponent<Animator>().GetAnimatorTransitionInfo(0).IsName("SlideLeft -> WalkRunLeft")) {
//					sliding = false;
//					transform.Translate(-Vector3.forward*Time.deltaTime*8);
//				} else if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Slide") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SlideLeft")) {
//					if(!GetComponent<Animator>().IsInTransition(0)) {
//						transform.Translate(Vector3.forward*lastMotion*Time.deltaTime*10);
//						GetComponent<Rigidbody>().AddForce(Vector3.down*10f, ForceMode.Acceleration);
//					}
//				}
//
//			}


		}

		public void SlideVelocity() 
		{
			GetComponent<Rigidbody>().velocity = Vector3.right*lastMotion*10f;
		}

		public void SlideOn() 
		{
			GetComponent<Animator>().SetBool("Slide", true);
		}

		public void SlideOff() 
		{
			GetComponent<Animator>().SetBool("Slide", false);
		}

		void OnAnimatorIK() 
		{
			Collider[] overlaps = Physics.OverlapSphere(transform.position + Vector3.up,1f);
			foreach(Collider c in overlaps) 
			{
				if(c.gameObject.tag.Equals("Ledge")) 
				{
					IKThingy = c.gameObject;
					break;
				}
				IKThingy = null;
			}
			if(IKThingy != null) 
			{
				GetComponent<Animator>().SetIKPosition(AvatarIKGoal.RightHand,IKThingy.transform.position + new Vector3(-0.5f,0,0));
				GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				GetComponent<Animator>().SetIKPosition(AvatarIKGoal.LeftHand,IKThingy.transform.position + new Vector3(0.5f,0,0));
				GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
				jumping = false;
//				if(!ledgeGrabbing) 
//				{
//					GetComponent<Rigidbody>().velocity = Vector3.zero;
//				}
//				ledgeGrabbing = true;
			} 
			else 
			{
//				ledgeGrabbing = false;
				GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				GetComponent<Animator>().SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
			}
		}

		public void JumpVelocity() 
		{
			GetComponent<Rigidbody>().velocity = Vector3.up*8f;
		}

		void OnCollisionStay(Collision other) 
		{
			if((GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Airtime") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("AirtimeLeft")) && other.gameObject.tag.Equals("Ground")) 
			{
				GetComponent<Animator>().SetTrigger("Land");
				jumping = false;
			}
		}

		public bool FacingRight 
		{
			get { return facingRight; }
		}

		/// <summary>
		/// Overriding the collect token method from player controller object
		/// </summary>
		/// <param name="token">The token that was collected</param>
		public override void CollectToken(Token token) { }
	}
}