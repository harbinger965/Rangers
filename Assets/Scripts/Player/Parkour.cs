using UnityEngine;
using Assets.Scripts.Tokens;
using System.Collections;

namespace Assets.Scripts.Player
{
	public class Parkour : ControllerObject 
	{

		private bool facingRight = true;
		private bool jumping = false;
		//private bool ledgeGrabbing = false;
		private bool sliding = false;
        private bool grappling = false;

		private GameObject IKThingy;

		private float slidingTime;
		private float lastMotion;
		private float jumpingTimeOffset;

		/// <summary> The animator attached to the player. </summary>
		private Animator animator;
        /// <summary> The rigidbody attached to the player. </summary>
        private new Rigidbody rigidbody;

        private void Start()
		{
			animator = GetComponent<Animator>();
			rigidbody = GetComponent<Rigidbody>();
		}

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
			else
			{
				facingRight = !animator.GetCurrentAnimatorStateInfo(0).IsTag("Left");
			}
			if (animator.GetBool("CanMove"))
			{
				if(facingRight && Physics.Raycast(new Ray(transform.position, transform.forward),0.5f,~(1<<9))) 
				{
					animator.SetFloat("RunSpeed", Mathf.Min(0,motion));
				} 
				else if(!facingRight && Physics.Raycast(new Ray(transform.position, -transform.forward),0.5f,~(1<<9))) 
				{
					animator.SetFloat("RunSpeed", Mathf.Max(0,motion));
				} 
				else 
				{
					if(!jumping) 
					{
						animator.SetFloat("RunSpeed", motion);
						transform.Translate(Vector3.forward*motion*Time.deltaTime*8);
					} 
					else 
					{
						animator.SetFloat("RunSpeed", motion);
						transform.Translate(Vector3.forward*motion*Time.deltaTime*4);
					}
				}
			}
			lastMotion = motion;
			AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
			sliding = state.IsName("Slide") || state.IsName("SlideLeft");
		}

		public void Jump()
		{
            if(grappling)
            {
                GetComponent<Grapple>().Ungrapple();
            }
			else if(!jumping) 
			{
				animator.ResetTrigger("Land");
				if(facingRight)
				{
					animator.SetTrigger("Jump");
				} 
				else if (!facingRight) 
				{
					animator.SetTrigger("JumpLeft");
				}
				jumping = true;
				jumpingTimeOffset = 0.1f;
				SFXManager.instance.PlayJump();
			}
		}

		void Update() 
		{
			jumpingTimeOffset -= Time.deltaTime;
			//			if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slide")) {
			////				transform.Translate(Vector3.right*lastMotion*Time.deltaTime*16f, Space.World);
			//				sliding = true;
			////				if(animator.IsInTransition(0)) {
			////					lastMotion -= Time.deltaTime;
			////				}
			//			} else if(animator.GetCurrentAnimatorStateInfo(0).IsName("SlideLeft")) {
			////				transform.Translate(-Vector3.left*lastMotion*Time.deltaTime*16f, Space.World);
			//				sliding = true;
			////				if(animator.IsInTransition(0)) {
			////					lastMotion += Time.deltaTime;
			////				}
			//			} else {
			//				sliding = false;
			//			}
			//			if((facingRight && !Physics.Raycast(new Ray(transform.position, transform.forward),0.5f,~(1<<9)))
			//				|| (!facingRight && !Physics.Raycast(new Ray(transform.position, -transform.forward),0.5f,~(1<<9)))) {
			//
			//				if((animator.GetAnimatorTransitionInfo(0).IsName("WalkRunRight -> Slide")
			//					|| animator.GetAnimatorTransitionInfo(0).IsName("WalkRunLeft -> SlideLeft"))
			//					&& !animator.IsInTransition(0)) {
			//					sliding = true;
			//					transform.Translate(Vector3.forward*lastMotion*Time.deltaTime*8f);
			//				} else if(animator.GetAnimatorTransitionInfo(0).IsName("Slide -> WalkRunRight")) {
			//					sliding = false;
			//					transform.Translate(Vector3.forward*Time.deltaTime*8);
			//				} else if(animator.GetAnimatorTransitionInfo(0).IsName("SlideLeft -> WalkRunLeft")) {
			//					sliding = false;
			//					transform.Translate(-Vector3.forward*Time.deltaTime*8);
			//				} else if(animator.GetCurrentAnimatorStateInfo(0).IsName("Slide") || animator.GetCurrentAnimatorStateInfo(0).IsName("SlideLeft")) {
			//					if(!animator.IsInTransition(0)) {
			//						transform.Translate(Vector3.forward*lastMotion*Time.deltaTime*10);
			//						rigidbody.AddForce(Vector3.down*10f, ForceMode.Acceleration);
			//					}
			//				}
			//
			//			}


		}

		public void SlideVelocity() 
		{
			rigidbody.velocity = Vector3.right*lastMotion*10f;
		}

		public void SlideOn() 
		{
			animator.SetBool("Slide", true);
		}

		public void SlideOff() 
		{
			animator.SetBool("Slide", false);
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
				animator.SetIKPosition(AvatarIKGoal.RightHand,IKThingy.transform.position + new Vector3(-0.5f,0,0));
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
				animator.SetIKPosition(AvatarIKGoal.LeftHand,IKThingy.transform.position + new Vector3(0.5f,0,0));
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,1);
				AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
				if(!state.IsName("Jump") && !state.IsName("JumpLeft"))
				{
					jumping = false;
				}
				//				if(!ledgeGrabbing) 
				//				{
				//					rigidbody.velocity = Vector3.zero;
				//				}
				//				ledgeGrabbing = true;
			} 
			else 
			{
				//				ledgeGrabbing = false;
				animator.SetIKPositionWeight(AvatarIKGoal.RightHand,0);
				animator.SetIKPositionWeight(AvatarIKGoal.LeftHand,0);
			}
		}


		// Called by animation event, it is time to jump, handles full vs short hop
		public void JumpVelocity() 
		{
			// If the jump button is still being held, full hop
			if (controller.IsHoldingJump())
				rigidbody.velocity = Vector3.up*8f;
			// else short hop
			else
				rigidbody.velocity = Vector3.up*5f;
		}

		void OnCollisionStay(Collision other) 
		{
			if((animator.GetCurrentAnimatorStateInfo(0).IsName("Airtime") || animator.GetCurrentAnimatorStateInfo(0).IsName("AirtimeLeft")) && other.gameObject.tag.Equals("Ground")) 
			{
				animator.SetTrigger("Land");
				jumping = false;
			}
		}

		public bool FacingRight 
		{
			get { return facingRight; }
		}

        public bool Grappling
        {
            get { return grappling; }
            set { grappling = value; }
        }

		/// <summary>
		/// Overriding the collect token method from player controller object
		/// </summary>
		/// <param name="token">The token that was collected</param>
		public override void CollectToken(Token token) { }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Assets.Scripts.Player.Parkour"/> is sliding.
		/// </summary>
		/// <value><c>true</c> if sliding; otherwise, <c>false</c>.</value>
		public bool Sliding
		{
			get { return sliding; }
		}
	}
}